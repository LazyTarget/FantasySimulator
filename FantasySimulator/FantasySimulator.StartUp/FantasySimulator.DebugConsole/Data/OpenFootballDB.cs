using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FantasySimulator.Simulator.Soccer;

namespace FantasySimulator.DebugConsole.Data
{
    public class OpenFootballDB
    {
        private static readonly string BaseUrl                  = "https://raw.githubusercontent.com";
        private static readonly string GetEnglishClubsUrl       = "/openfootball/eng-england/master/clubs/1-premierleague.txt";
        private static readonly string GetEnglishClubs2Url      = "/openfootball/eng-england/master/clubs/2-championship.txt";
        private static readonly string GetWelshClubsUrl         = "/openfootball/eng-england/master/clubs/wales.txt";


        public async Task<IList<Team>> GetEnglishClubs()
        {
            var result = new List<Team>();
            try
            {
                // Get clubs
                var http = new HttpClient();
                http.BaseAddress = new Uri(BaseUrl);
                var uri = GetEnglishClubsUrl;
                var request = new HttpRequestMessage(HttpMethod.Get, uri);
                var response = await http.SendAsync(request);
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var streamReader = new StreamReader(stream))
                {
                    var prevIsTeamHeader = false;
                    while (!streamReader.EndOfStream)
                    {
                        var line = await streamReader.ReadLineAsync();
                        if (prevIsTeamHeader)
                        {
                            var vals = line.Trim().Split(',');
                            var names = vals[0].Split('|').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                            var year = vals.Length > 1 ? vals[1].Trim() : null;
                            var abbr = vals.Length > 2 ? vals[2].Trim() : null;

                            var team = new Team();
                            team.ID = names.First();
                            team.Name = names.First();
                            team.Aliases = names;
                            team.ShortName = abbr;
                            result.Add(team);
                        }


                        if (line.StartsWith("[") && line.Contains("]"))
                            prevIsTeamHeader = true;
                        else
                            prevIsTeamHeader = false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

        public async Task<IList<Team>> GetEnglishClubs2()
        {
            var result = new List<Team>();
            try
            {
                // Get clubs
                var http = new HttpClient();
                http.BaseAddress = new Uri(BaseUrl);
                var uri = GetEnglishClubs2Url;
                var request = new HttpRequestMessage(HttpMethod.Get, uri);
                var response = await http.SendAsync(request);
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var streamReader = new StreamReader(stream))
                {
                    var prevIsTeamHeader = false;
                    while (!streamReader.EndOfStream)
                    {
                        var line = await streamReader.ReadLineAsync();
                        if (prevIsTeamHeader)
                        {
                            var vals = line.Trim().Split(',');
                            var names = vals[0].Split('|').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                            var year = vals.Length > 1 ? vals[1].Trim() : null;
                            var abbr = vals.Length > 2 ? vals[2].Trim() : null;

                            var team = new Team();
                            team.ID = names.First();
                            team.Name = names.First();
                            team.Aliases = names;
                            team.ShortName = abbr;
                            result.Add(team);
                        }


                        if (line.StartsWith("[") && line.Contains("]"))
                            prevIsTeamHeader = true;
                        else
                            prevIsTeamHeader = false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }


        public async Task<IList<Team>> GetWelshClubs()
        {
            var result = new List<Team>();
            try
            {
                // Get clubs
                var http = new HttpClient();
                http.BaseAddress = new Uri(BaseUrl);
                var uri = GetWelshClubsUrl;
                var request = new HttpRequestMessage(HttpMethod.Get, uri);
                var response = await http.SendAsync(request);
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var streamReader = new StreamReader(stream))
                {
                    var prevIsTeamHeader = false;
                    while (!streamReader.EndOfStream)
                    {
                        var line = await streamReader.ReadLineAsync();
                        if (prevIsTeamHeader)
                        {
                            var vals = line.Trim().Split(',');
                            var names = vals[0].Split('|').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                            var year = vals.Length > 1 ? vals[1].Trim() : null;
                            var abbr = vals.Length > 2 ? vals[2].Trim() : null;

                            var team = new Team();
                            team.ID = names.First();
                            team.Name = names.First();
                            team.Aliases = names;
                            team.ShortName = abbr;
                            result.Add(team);
                        }


                        if (line.StartsWith("[") && line.Contains("]"))
                            prevIsTeamHeader = true;
                        else
                            prevIsTeamHeader = false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

    }
}
