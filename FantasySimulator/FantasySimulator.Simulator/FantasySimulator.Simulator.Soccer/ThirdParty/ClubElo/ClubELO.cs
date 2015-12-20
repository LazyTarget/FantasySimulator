using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using FantasySimulator.Core;
using FantasySimulator.Interfaces;

namespace FantasySimulator.Simulator.Soccer.ThirdParty.ClubElo
{
    public class ClubElo : IEloProvider, IXmlConfigurable, IHasProperties
    {
        private readonly IDictionary<DateTime, IList<RatingsRow>> _eloCache; 

        public ClubElo()
        {
            _eloCache = new Dictionary<DateTime, IList<RatingsRow>>();
            Properties = new Dictionary<string, object>();
            BaseUrl = "http://api.clubelo.com/";
        }

        public IDictionary<string, object> Properties { get; private set; }
        
        public string BaseUrl
        {
            get { return Properties["BaseUrl"].SafeConvert<string>(); }
            set { Properties["BaseUrl"] = value; }
        }

        protected virtual HttpClient GetHttpClient()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl),
            };
            return client;
        }


        private async Task UpdateCacheForDate(DateTime time)
        {
            var path = $"/{time.ToString("yyyy-MM-dd")}";
            var request = new HttpRequestMessage(HttpMethod.Get, path);

            try
            {
                var client = GetHttpClient();
                var response = await client.SendAsync(request);
                var csv = await response.Content.ReadAsStringAsync();
                var ratings = ParseRatingsFromCsv(csv);
                _eloCache[time] = ratings;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        protected virtual IList<RatingsRow> ParseRatingsFromCsv(string csv)
        {
            int idxRank = -1, 
                idxClub = -1,
                idxCountry = -1,
                idxLevel = -1,
                idxElo = -1,
                idxFrom = -1,
                idxTo = -1;

            var result = new List<RatingsRow>();
            var rows = csv.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (var rowIndex = 0; rowIndex < rows.Length; rowIndex++)
            {
                var row = rows[rowIndex];
                var parts = row.Split(',');
                
                if (rowIndex == 0)
                {
                    // Header row
                    for (var i = 0; i < parts.Length; i++)
                    {
                        var text = parts[i];
                        if (text == "Rank")
                            idxRank = i;
                        else if (text == "Club")
                            idxClub = i;
                        else if (text == "Country")
                            idxCountry = i;
                        else if (text == "Level")
                            idxLevel = i;
                        else if (text == "Elo")
                            idxElo = i;
                        else if (text == "From")
                            idxFrom = i;
                        else if (text == "To")
                            idxTo = i;
                    }
                    continue;
                }

                var ratingsRow = new RatingsRow();
                if (idxRank >= 0)
                {
                    bool success;
                    var res = parts[idxRank].SafeConvert<int>(-1, out success);
                    if (success)
                        ratingsRow.Rank = res;
                    else
                        ratingsRow.Rank = null;
                }
                if (idxClub >= 0)
                    ratingsRow.Club = parts[idxClub];
                if (idxCountry >= 0)
                    ratingsRow.Country = parts[idxCountry];
                if (idxLevel >= 0)
                    ratingsRow.Level = parts[idxLevel].SafeConvert<int>();
                if (idxElo >= 0)
                    ratingsRow.Elo = parts[idxElo].SafeConvert<float>();
                if (idxFrom >= 0)
                    ratingsRow.From = parts[idxFrom].SafeConvert<DateTime>();
                if (idxTo >= 0)
                    ratingsRow.To = parts[idxTo].SafeConvert<DateTime>();

                var valid = !string.IsNullOrWhiteSpace(ratingsRow.Club);
                if (valid)
                    result.Add(ratingsRow);
            }
            return result;
        }


        public async Task<EloRating> GetRating(DateTime time, Team team)
        {
            var ratings = await GetRatings(time, new[] {team});
            var rating = ratings?.FirstOrDefault();
            return rating;
        }

        public virtual async Task<IEnumerable<EloRating>> GetRatings(DateTime time, IEnumerable<Team> teams)
        {
            time = time.Date;
            if (!_eloCache.ContainsKey(time))
            {
                await UpdateCacheForDate(time);
            }

            var teamsList = teams.ToList();
            var result = new List<EloRating>();
            var ratings = _eloCache[time];
            foreach (var ratingsRow in ratings)
            {
                var team = teamsList.FirstOrDefault(x => x.MatchName(ratingsRow.Club));
                if (team != null)
                {
                    var rating = new EloRating
                    {
                        TeamName = team.Name,
                        Rating = ratingsRow.Elo,
                        Time = time,
                    };
                    result.Add(rating);
                }
            }
            return result;
        }


        protected class RatingsRow
        {
            public int? Rank { get; set; }
            public string Club { get; set; }
            public string Country { get; set; }
            public int Level { get; set; }
            public float Elo { get; set; }
            public DateTime From { get; set; }
            public DateTime To { get; set; }
        }


        public void Configure(XElement element)
        {
            
        }
    }
}
