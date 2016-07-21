using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using FantasySimulator.Core;
using FantasySimulator.Interfaces;
using FantasySimulator.Simulator.Soccer;
using HtmlAgilityPack;
using Lux.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FantasySimulator.DebugConsole.Data
{
    //[AutoConfigureProperties] todo:
    public class FantasyPremierLeague2016DataFactory : ISoccerSimulationDataFactory, IHasProperties, IXmlConfigurable
    {
        private const string GetDataJsonUrl         = "https://fantasy.premierleague.com/drf/bootstrap-static";

        private List<Team> _ukClubs;

        private readonly MacroResolver _macroResolver = new MacroResolver();


        public FantasyPremierLeague2016DataFactory()
        {
            Properties = new Dictionary<string, object>();
        }

        public IDictionary<string, object> Properties { get; private set; }

        public bool FetchNewData
        {
            get { return Properties["FetchNewData"].SafeConvert<bool>(); }
            set { Properties["FetchNewData"] = value; }
        }

        public string JsonDataOutputFilename
        {
            get { return Properties["JsonDataOutputFilename"].SafeConvert<string>(); }
            set { Properties["JsonDataOutputFilename"] = value; }
        }


        public void Configure(XElement element)
        {

        }



        public async Task<SoccerSimulationData> Generate()
        {
            var data = new SoccerSimulationData();
            data = await GenerateApplyTo(data);
            return data;
        }

        public async Task<SoccerSimulationData> GenerateApplyTo(SoccerSimulationData data)
        {
            try
            {
                var openFootball = new OpenFootballDB();
                var eng = await openFootball.GetEnglishClubs();
                var wal = await openFootball.GetWelshClubs();
                _ukClubs = new List<Team>();
                _ukClubs.AddRange(eng);
                _ukClubs.AddRange(wal);



                var macros = new Dictionary<string, Func<string, string, object>>();
                macros["now"]       = (macro, format) => DateTime.UtcNow.ToString(format);
                macros["now-utc"]   = (macro, format) => DateTime.UtcNow.ToString(format);
                macros["now-local"] = (macro, format) => DateTime.Now.ToString(format);

                var fileName = JsonDataOutputFilename;
                fileName = _macroResolver.Resolve(fileName, macros);

                
                JObject leagueDataJson;
                if (FetchNewData)
                {
                    leagueDataJson = await GetLeagueDataFromWebsite();
                    var saved = await SaveLeagueDataToTextFile(leagueDataJson, fileName);
                }
                else
                    leagueDataJson = await GetLeagueDataFromTextFile(fileName);


                JObject fixturesJson = null;


                var teams = new List<Team>();
                var premierLeague = GenerateLeague(leagueDataJson, fixturesJson, ref teams);


                // todo: FA Cup
                //var faCup = GenerateLeague(teamsAndPlayerJson, fixturesJson, ref teams);
                //faCup.ID = "FACUP";
                //faCup.Name = "FA Cup 16/17";


                // todo: Champions League


                // todo: include in simulation: players playing in multiple leagues => tired, playtime, etc.


                data.Teams = (data.Teams ?? new Team[0]).Concat(teams).ToArray();
                data.Leagues = data.Leagues.Append(premierLeague);
                //data.Leagues = data.Leagues.Append(faCup);
                return data;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private async Task<bool> SaveLeagueDataToTextFile(JObject leagueDataJson, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));
            try
            {
                // Write to file
                var path = fileName;
                if (!Path.IsPathRooted(path))
                {
                    //path = Path.Combine(Environment.CurrentDirectory, path);
                    path = PathHelper.Combine(Environment.CurrentDirectory, path);
                }
                //var dir = new DirectoryInfo(Path.GetDirectoryName(path));
                var dir = new DirectoryInfo(PathHelper.GetParent(path));
                if (!dir.Exists)
                    dir.Create();
                
                var exists = File.Exists(path);
                using (var stream = File.Open(path, exists ? FileMode.Truncate : FileMode.CreateNew, FileAccess.Write))
                {
                    var wr = new StreamWriter(stream);
                    var jw = new JsonTextWriter(wr)
                    {
                        Formatting = Formatting.Indented,
                    };
                    JsonConverter[] converters = new JsonConverter[0];
                    leagueDataJson.WriteTo(jw, converters);
                    await wr.FlushAsync();
                }
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }
        
        
        private async Task<JObject> GetLeagueDataFromWebsite()
        {
            var http = new HttpClient();
            try
            {
                var response = await http.GetAsync(GetDataJsonUrl);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<JObject>(json);
                return obj;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
        private async Task<JObject> GetLeagueDataFromTextFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));
            try
            {
                // Write to file
                var path = fileName;
                if (!Path.IsPathRooted(path))
                    path = Path.Combine(Environment.CurrentDirectory, path);
                string json;
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
        


        private League GenerateLeague(JObject leagueDataJson, JObject fixturesJson, ref List<Team> teams)
        {
            var league = new League
            {
                ID = "PL",
                Name = "Premier League 16/17",
                Year = 2016,
            };
            var leagueTeams = new List<LeagueTeam>();
            var gameweeks = new List<Gameweek>();
            var positions = new Dictionary<int, PlayerPosition>();


            // Get playing positions
            var typeInfo = leagueDataJson.Property("typeInfo").Value.ToObject<JArray>();
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
            var eiwTeams = leagueDataJson.Property("eiwteams").Value.ToObject<JObject>();
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
            var propMapping = leagueDataJson.Property("elStat").Value.ToObject<JObject>();
            var elInfo = leagueDataJson.Property("elInfo").Value.ToObject<JArray>();
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
                    ChanceOfPlayingNextFixture      = GetProperty<double>(playerData, propMapping, "chance_of_playing_this_round", -1) / 100,
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
                    Saves                           = GetProperty<int>(playerData, propMapping, "saves"),
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
            if (fixturesJson != null)
            {
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
                        var goalsHome = resultsJson?.GetPropertyValue<int>("goalsHomeTeam") ?? 0;
                        var goalsAway = resultsJson?.GetPropertyValue<int>("goalsAwayTeam") ?? 0;
                        fixture.Statistics = new FixtureStatistics
                        {
                            PlayedMinutes = min,
                            GameFinished = finished,
                            Score = FixtureScore.Create(goalsHome, goalsAway),
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
