using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FantasySimulator.Core;
using FantasySimulator.Simulator.Soccer;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FantasySimulator.DebugConsole.Data
{
    public class FantasyPremierLeagueDataFactory : ISoccerSimulationDataFactory
    {
        private static string TransfersPageJsonFilename = "TransfersPage_201510151854.json";
        private const string GetTransferDataJsonUrl     = "http://fantasy.premierleague.com/transfers/";
        private const string GetPlayerDataJsonUrl       = "http://fantasy.premierleague.com/web/api/elements/";
        private const string GetFixturesJsonUrl         = "http://api.football-data.org/alpha/soccerseasons/398/fixtures";

        private List<Team> _ukClubs;


        public async Task<SoccerSimulationData> Generate()
        {
            try
            {
                var openFootball = new OpenFootballDB();
                var eng = await openFootball.GetEnglishClubs();
                var wal = await openFootball.GetWelshClubs();
                _ukClubs = new List<Team>();
                _ukClubs.AddRange(eng);
                _ukClubs.AddRange(wal);
                

                var transfersUpdated = await UpdateTeamsAndPlayerTextFile();
                var fixturesUpdated = await UpdateFixturesTextFile();

                //var teamsAndPlayerJson = await GetJsonDataFromWebsite();
                var teamsAndPlayerJson = await GetTeamsAndPlayerDataFromTextFile();

                var fixturesJson = await GetFixturesFromTextFile();

                var teams = new List<Team>();


                var premierLeague = GenerateLeague(teamsAndPlayerJson, fixturesJson, ref teams);


                //var faCup = GenerateLeague(teamsAndPlayerJson, fixturesJson, ref teams);
                //faCup.ID = "FACUP";
                //faCup.Name = "FA Cup 15/16";


                // todo: Champions League


                // todo: include in simulation: players playing in multiple leagues => tired, playtime, etc.


                var data = new SoccerSimulationData();
                data.Leagues = data.Leagues.Append(premierLeague);
                //data.Leagues = data.Leagues.Append(faCup);
                return data;
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        private async Task<bool> UpdateFixturesTextFile()
        {
            try
            {
                // Get fixtures
                var http = new HttpClient();
                var response = await http.GetAsync(GetFixturesJsonUrl);
                var json = await response.Content.ReadAsStringAsync();

                // Replace team names to match FPL team naming standards
                json = json
                    .Replace("Arsenal FC",                  "Arsenal")
                    .Replace("Aston Villa FC",              "Aston Villa")
                    .Replace("AFC Bournemouth",             "Bournemouth")
                    .Replace("Chelsea FC",                  "Chelsea")
                    .Replace("Crystal Palace FC",           "Crystal Palace")
                    .Replace("Everton FC",                  "Everton")
                    .Replace("Leicester City FC",           "Leicester")
                    .Replace("Liverpool FC",                "Liverpool")
                    .Replace("Manchester City FC",          "Man City")
                    .Replace("Manchester United FC",        "Man Utd")
                    .Replace("Newcastle United FC",         "Newcastle")
                    .Replace("Norwich City FC",             "Norwich")
                    .Replace("Southampton FC",              "Southampton")
                    .Replace("Tottenham Hotspur FC",        "Spurs")
                    .Replace("Stoke City FC",               "Stoke")
                    .Replace("Sunderland AFC",              "Sunderland")
                    .Replace("Swansea City FC",             "Swansea")
                    .Replace("Watford FC",                  "Watford")
                    .Replace("West Bromwich Albion FC",     "West Brom")
                    .Replace("West Ham United FC",          "West Ham");

                // Write to file
                var path = Path.Combine(Environment.CurrentDirectory, "App_Data/Fixtures.json");
                var exists = File.Exists(path);
                using (var stream = File.Open(path, exists ? FileMode.Truncate : FileMode.CreateNew, FileAccess.Write))
                {
                    var wr = new StreamWriter(stream);
                    var jw = new JsonTextWriter(wr)
                    {
                        Formatting = Formatting.Indented,
                    };
                    jw.WriteRaw(json);
                    await wr.FlushAsync();
                }
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }
        
        private async Task<bool> UpdateTeamsAndPlayerTextFile()
        {
            return false;
        }


        private async Task<JObject> GetFixturesFromTextFile()
        {
            try
            {
                string json;
                var path = Path.Combine(Environment.CurrentDirectory, "App_Data/Fixtures.json");
                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    var streamReader = new StreamReader(stream);
                    json = await streamReader.ReadToEndAsync();
                }
                var obj = JsonConvert.DeserializeObject<JObject>(json);
                return obj;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [Obsolete]
        private async Task<JObject> GetTeamsAndPlayerDataFromWebsite()
        {
            var http = new HttpClient();
            try
            {
                var response = await http.GetAsync(GetTransferDataJsonUrl);
                var content = await response.Content.ReadAsStringAsync();

                var html = new HtmlDocument();
                html.LoadHtml(content);
                var div = html.GetElementbyId("ismJson");
                var json = div.Element("script").InnerText;

                var obj = JsonConvert.DeserializeObject<JObject>(json);
                return obj;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
        private async Task<JObject> GetTeamsAndPlayerDataFromTextFile()
        {
            try
            {
                string json;
                var path = Path.Combine(Environment.CurrentDirectory, "App_Data/", TransfersPageJsonFilename);
                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    var streamReader = new StreamReader(stream);
                    json = await streamReader.ReadToEndAsync();
                }
                var obj = JsonConvert.DeserializeObject<JObject>(json);
                return obj;
            }
            catch (Exception ex)
            {
                throw;
            }
        }





        private async Task<JObject> GetPlayerDataFromWebsite(long id)
        {
            var http = new HttpClient();
            try
            {
                var url = GetPlayerDataJsonUrl + id;
                var response = await http.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<JObject>(json);
                return obj;
            }
            catch (Exception ex)
            {
                //throw;
            }
            return null;
        }

        


        private League GenerateLeague(JObject teamsAndPlayersJson, JObject fixturesJson, ref List<Team> teams)
        {
            var league = new League
            {
                ID = "PL",
                Name = "Premier League 15/16",
                Year = 2015,
            };
            var leagueTeams = new List<LeagueTeam>();
            var gameweeks = new List<Gameweek>();
            var positions = new Dictionary<int, PlayerPosition>();


            // Get playing positions
            var typeInfo = teamsAndPlayersJson.Property("typeInfo").Value.ToObject<JArray>();
            foreach (var type in typeInfo.Select(x => x.ToObject<JObject>()).Where(x => x != null))
            {
                PlayerPosition pos;
                var posID = type.GetPropertyValue<int>("id");
                var posShortName = type.GetPropertyValue<string>("singular_name_short");
                switch (posShortName)
                {
                    case "GKP":
                        pos = PlayerPosition.Goalkeeper;
                        break;

                    case "DEF":
                        pos = PlayerPosition.Defender;
                        break;

                    case "MID":
                        pos = PlayerPosition.Midfielder;
                        break;

                    case "FWD":
                        pos = PlayerPosition.Forward;
                        break;

                    default:
                        continue;
                }
                positions[posID] = pos;
            }


            // Get teams
            var eiwTeams = teamsAndPlayersJson.Property("eiwteams").Value.ToObject<JObject>();
            if (eiwTeams != null)
            {
                foreach (var prop in eiwTeams.Properties())
                {
                    var t = prop.Value.ToObjectOrDefault<JObject>();
                    var teamID = t.GetPropertyValue<string>("id");
                    if (string.IsNullOrWhiteSpace(teamID))
                        throw new FormatException("Invalid TeamID");

                    var team = teams.FirstOrDefault(x => x.ID == teamID);
                    if (team == null)
                    {
                        team = new Team
                        {
                            ID = teamID,
                            Name = t.GetPropertyValue<string>("name"),
                            ShortName = t.GetPropertyValue<string>("short_name"),

                            // todo: implement
                            //Rating = 
                            //Statistics = 
                        };
                        teams.Add(team);

                        var club = _ukClubs?.FirstOrDefault(x => x.MatchName(team.Name));
                        if (club != null)
                        {
                            team.Aliases = club.Aliases;
                        }
                    }
                
                    var leagueTeam = new LeagueTeam(league, team);
                    leagueTeams.Add(leagueTeam);
                    team.Leagues = team.Leagues.Append(league);
                }
            }




            // Get players
            var propMapping = teamsAndPlayersJson.Property("elStat").Value.ToObject<JObject>();
            var elInfo = teamsAndPlayersJson.Property("elInfo").Value.ToObject<JArray>();
            var stats = elInfo.Select(x => x.ToObjectOrDefault<JArray>()).Where(x => x != null);

            foreach (var playerData in stats)
            {
                var posID = GetProperty<int>(playerData, propMapping, "element_type_id");

                // todo: load and update player (statitics), if player plays in multiple leagues

                var player                          = new Player();
                player.ID                           = GetProperty<string>(playerData, propMapping, "id");
                player.FirstName                    = GetProperty<string>(playerData, propMapping, "first_name");
                player.LastName                     = GetProperty<string>(playerData, propMapping, "second_name");
                player.DisplayName                  = GetProperty<string>(playerData, propMapping, "web_name");
                
                player.Fantasy                      = new FantasyPlayer
                {
                    Position                        = positions[posID],
                    CurrentPrice                    = GetProperty<double>(playerData, propMapping, "event_cost") / 10,
                    OriginalPrice                   = GetProperty<double>(playerData, propMapping, "original_cost") / 10,
                    Unavailable                     = GetProperty<string>(playerData, propMapping, "status") != "a",
                    ChanceOfPlayingNextFixture      = GetProperty<double>(playerData, propMapping, "chance_of_playing_this_round", -1),
                    News                            = GetProperty<string>(playerData, propMapping, "news"),
                    OwnagePercent                   = GetProperty<double>(playerData, propMapping, "selected_by_percent", -1),
                    TransfersDetailsForSeason       = new TransferDetails
                    {
                        TransfersIn                 = GetProperty<int>(playerData, propMapping, "transfers_in"),
                        TransfersOut                = GetProperty<int>(playerData, propMapping, "transfers_out"),
                    },
                    TransfersDetailsForGW           = new TransferDetails
                    {
                        TransfersIn                 = GetProperty<int>(playerData, propMapping, "transfers_in_event"),
                        TransfersOut                = GetProperty<int>(playerData, propMapping, "transfers_out_event"),
                    },
                };

                player.Statistics                   = new PlayerStatistics
                {
                    PlayedMinutes                   = GetProperty<int>(playerData, propMapping, "minutes"),
                    Goals                           = GetProperty<int>(playerData, propMapping, "goals_scored"),
                    Assists                         = GetProperty<int>(playerData, propMapping, "assists"),
                    TimesInDreamteam                = GetProperty<int>(playerData, propMapping, "dreamteam_count"),
                    YellowCards                     = GetProperty<int>(playerData, propMapping, "yellow_cards"),
                    RedCards                        = GetProperty<int>(playerData, propMapping, "red_cards"),
                    BonusPoints                     = GetProperty<int>(playerData, propMapping, "bonus"),
                    Form                            = GetProperty<double>(playerData, propMapping, "form"),
                    PenaltiesMissed                 = GetProperty<int>(playerData, propMapping, "penalties_missed"),
                    PenaltiesSaved                  = GetProperty<int>(playerData, propMapping, "penalties_scored"),
                    TotalPoints                     = GetProperty<int>(playerData, propMapping, "total_points"),
                    CleanSheets                     = GetProperty<int>(playerData, propMapping, "clean_sheets"),
                    PointsPerGame                   = GetProperty<double>(playerData, propMapping, "points_per_game"),
                    OwnGoals                        = GetProperty<int>(playerData, propMapping, "own_goals"),


                    // todo:
                    //Appearances = "",
                    //Crosses = 
                    //Offsides = 
                    //PenaltiesScored = 
                    //Substitutions = 
                    //Shots = 
                };

                var teamID = GetProperty<string>(playerData, propMapping, "team_id");
                var team = teams.Single(x => x.ID == teamID);
                player.Team = team;
                team.Players = team.Players.Append(player);

                // Update Team statistics   (todo: will get wrong if player changes team)
                //team.Statistics.GoalsFor = player.Statistics.Goals;
            }



            // Fixtures
            var fixtures = fixturesJson.GetPropertyValue<JArray>("fixtures");
            foreach (var f in fixtures)
            {
                try
                {
                    var fixture = new Fixture();

                    var obj = f.ToObjectOrDefault<JObject>();
                    fixture.Time = obj.GetPropertyValue<DateTime>("date");

                    var homeTeamName = obj.GetPropertyValue<string>("homeTeamName");
                    var awayTeamName = obj.GetPropertyValue<string>("awayTeamName");
                    var homeTeam = leagueTeams.Single(x => x.Team.Name == homeTeamName || x.Team.MatchName(homeTeamName));
                    var awayTeam = leagueTeams.Single(x => x.Team.Name == awayTeamName || x.Team.MatchName(awayTeamName));
                    fixture.HomeTeam = homeTeam;
                    fixture.AwayTeam = awayTeam;

                    var resultsJson = obj.GetPropertyValue<JObject>("result");
                    var status = obj.GetPropertyValue<string>("status");
                    var finished = status == "FINISHED";
                    var min = obj.GetPropertyValue<int>("minute");      // todo: correct?
                    if (finished && min <= 0)
                        min = 90;
                    fixture.Statistics = new FixtureStatistics
                    {
                        PlayedMinutes = min,
                        GameFinished = finished,
                        Score = new FixtureScore
                        {
                            GoalsForHomeTeam = resultsJson?.GetPropertyValue<int>("goalsHomeTeam") ?? 0,
                            GoalsForAwayTeam = resultsJson?.GetPropertyValue<int>("goalsAwayTeam") ?? 0,
                        },
                    };


                    var matchday = obj.GetPropertyValue<int>("matchday");
                    var gw = gameweeks.SingleOrDefault(x => x.Number == matchday);
                    if (gw == null)
                    {
                        gw = new Gameweek(league)
                        {
                            Number = matchday,
                        };
                        gameweeks.Add(gw);
                    }
                    fixture.Gameweek = gw;
                    gw.Fixtures = gw.Fixtures.Append(fixture);


                    // Update Team statistics
                    fixture.HomeTeam.UpdateStatisticsBasedOnFixture(fixture);
                    fixture.AwayTeam.UpdateStatisticsBasedOnFixture(fixture);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }


            
            league.Teams = leagueTeams.ToArray();
            league.Gameweeks = gameweeks.ToArray();
            return league;
        }


        private TValue GetProperty<TValue>(JArray data, JObject mapping, string propertyName, TValue defaultValue = default(TValue))
        {
            try
            {
                var res = defaultValue;
                var prop = mapping.Property(propertyName);
                if (prop != null && prop.HasValues)
                {
                    var propIndex = prop.Value.ToObjectOrDefault<int>();
                    if (propIndex >= 0)
                        res = data.ElementAt(propIndex).ToObjectOrDefault<TValue>(defaultValue);
                }
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
