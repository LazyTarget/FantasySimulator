using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FantasySimulator.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FantasySimulator.Simulator.Soccer.ThirdParty.Kambi
{
    public class KambiAPI : IFixtureOddsProvider
    {
        private readonly HttpClient _client;
        private string _fixtureOddsCache;

        
        public KambiAPI()
        {
            BaseUrl = "https://e2-api.kambi.com/";

            _client = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl),
            };
        }

        public readonly string BaseUrl;



        private async Task UpdateFixtureOddsCache()
        {
            var path = "/offering/api/v2/ub/betoffer/group/1000094985.json?cat=1295&market=se&lang=en_GB&range_size=100&range_start=0&suppress_response_codes";
            var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Properties["channel_id"] = 1;

            var response = await _client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            _fixtureOddsCache = json;
        }


        public async Task<FixtureOdds> GetFixtureOdds(Fixture fixture)
        {
            try
            {
                if (_fixtureOddsCache == null)
                {
                    await UpdateFixtureOddsCache();

                    // todo: clear cache when appropriate
                }


                var obj = JsonConvert.DeserializeObject<JObject>(_fixtureOddsCache);
                var events = obj.GetPropertyValue<JArray>("events");

                var eventID = "";
                var expectedEnglishName = string.Format("{0} - {1}", fixture.HomeTeam.Team.Name, fixture.AwayTeam.Team.Name);
                foreach (var evt in events.Select(x => x.ToObjectOrDefault<JObject>()).Where(x => x != null))
                {
                    var match = false;

                    var homeName = evt.GetPropertyValue<string>("homeName");
                    var awayName = evt.GetPropertyValue<string>("awayName");
                    if (!string.IsNullOrWhiteSpace(homeName) &&
                        !string.IsNullOrWhiteSpace(awayName))
                    {
                        var a = fixture.HomeTeam.Team.MatchName(homeName);
                        var b = fixture.AwayTeam.Team.MatchName(awayName);
                        if (a && b)
                            match = true;
                    }

                    var name = (evt.GetPropertyValue<string>("name") ?? "");
                    var englishName = (evt.GetPropertyValue<string>("englishName") ?? "");
                    if (expectedEnglishName.Equals(name, StringComparison.OrdinalIgnoreCase) ||
                        expectedEnglishName.Equals(englishName, StringComparison.OrdinalIgnoreCase))
                    {
                        match = true;
                    }

                    if (match)
                    {
                        eventID = evt.GetPropertyValue<string>("id");
                        break;
                    }
                }


                if (!string.IsNullOrWhiteSpace(eventID))
                {
                    var betoffers = obj.GetPropertyValue<JArray>("betoffers");
                    foreach (var betoffer in betoffers.Select(x => x.ToObjectOrDefault<JObject>()).Where(x => x != null))
                    {
                        var evtID = betoffer.GetPropertyValue<string>("eventId");
                        if (evtID != eventID)
                            continue;
                        var criterion = betoffer.GetPropertyValue<JObject>("criterion");
                        var lbl = criterion.GetPropertyValue<string>("label");
                        if (lbl != "Full Time")
                            continue;

                        var outcomes = betoffer.GetPropertyValue<JArray>("outcomes");
                        var oddsOutcomes = ParseFixtureOutcomes(outcomes);
                        var odds = new FixtureOdds
                        {
                            HomeWin = oddsOutcomes.HomeWin,
                            Draw = oddsOutcomes.Draw,
                            AwayWin = oddsOutcomes.AwayWin,
                            Fixture = fixture,
                            //PredictedScore = todo
                        };
                        return odds;
                    }
                }
            }
            catch(Exception ex)
            {

            }

            return null;
        }


        private FixtureOdds ParseFixtureOutcomes(JArray outcomes)
        {
            FixtureOdds res = null;
            foreach (var outcome in outcomes.Select(x => x.ToObjectOrDefault<JObject>()).Where(x => x != null))
            {
                var lbl = outcome.GetPropertyValue<string>("label");
                var odds = outcome.GetPropertyValue<double>("odds") / 1000;

                if (lbl == "1")
                {
                    res = res ?? new FixtureOdds();
                    res.HomeWin = odds;
                }
                else if (lbl == "X")
                {
                    res = res ?? new FixtureOdds();
                    res.Draw = odds;
                }
                else
                if (lbl == "2")
                {
                    res = res ?? new FixtureOdds();
                    res.AwayWin = odds;
                }
            }
            return res;
        }

    }
}
