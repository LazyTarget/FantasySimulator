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
                _ukClubs = new List<Team>();
                try
                {
                    var eng = await openFootball.GetEnglishClubs();
                    var wal = await openFootball.GetWelshClubs();
                    _ukClubs.AddRange(eng);
                    _ukClubs.AddRange(wal);
                }
                catch (Exception ex)
                {
                    
                }



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
            var typeInfo = leagueDataJson?.Property("element_types")?.Value?.ToObject<JArray>();
            if (typeInfo != null)
            {
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
            }
            


            // Get teams
            var eiwTeams = leagueDataJson?.Property("teams")?.Value?.ToObject<JArray>();
            if (eiwTeams != null)
            {
                foreach (var eiwTeam in eiwTeams)
                {
                    var t = eiwTeam?.ToObjectOrDefault<JObject>();
                    if (t == null)
                        continue;
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
                            //Statistics = 
                            //Rating = new Rating
                            //{
                            //    // todo: normalize to a 1-10 rating
                            //    //Value = (t.GetPropertyValue<int>("strength_overall_home") +
                            //    //        t.GetPropertyValue<int>("strength_overall_away")) / 2
                            //}
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
            var elements = leagueDataJson?.Property("elements")?.Value?.ToObject<JArray>();
            if (elements != null)
            {
                foreach (var playerData in elements)
                {
                    var p = playerData?.ToObjectOrDefault<JObject>();
                    if (p == null)
                        continue;

                    var posID = p.GetPropertyValue<int>("element_type");
                    var pos = positions[posID];
                    
                    // todo: load and update player (statitics), if player plays in multiple leagues

                    var player                          = new Player();
                    player.ID                           = p.GetPropertyValue<string>("id");
                    player.FirstName                    = p.GetPropertyValue<string>("first_name");
                    player.LastName                     = p.GetPropertyValue<string>("second_name");
                    player.DisplayName                  = p.GetPropertyValue<string>("web_name");
                    
                    var currentPrice = p.GetPropertyValue<double>("now_cost") / 10;
                    var costChangeSinceStart = p.GetPropertyValue<double>("cost_change_start") / 10;

                    player.Fantasy                      = new FantasyPlayer
                    {
                        Position                        = pos,
                        CurrentPrice                    = currentPrice,
                        OriginalPrice                   = currentPrice + costChangeSinceStart,
                        Unavailable                     = p.GetPropertyValue<string>("status") != "a",
                        ChanceOfPlayingNextFixture      = p.GetPropertyValue<double>("chance_of_playing_this_round", -1) / 100,
                        News                            = p.GetPropertyValue<string>("news"),
                        OwnagePercent                   = p.GetPropertyValue<double>("selected_by_percent", -1),
                        TransfersDetailsForSeason       = new TransferDetails
                        {
                            TransfersIn                 = p.GetPropertyValue<int>("transfers_in"),
                            TransfersOut                = p.GetPropertyValue<int>("transfers_out"),
                        },
                        TransfersDetailsForGW           = new TransferDetails
                        {
                            TransfersIn                 = p.GetPropertyValue<int>("transfers_in_event"),
                            TransfersOut                = p.GetPropertyValue<int>("transfers_out_event"),
                        },
                    };

                    player.Statistics                   = new PlayerStatistics
                    {
                        PlayedMinutes                   = p.GetPropertyValue<int>("minutes"),
                        Goals                           = p.GetPropertyValue<int>("goals_scored"),
                        Assists                         = p.GetPropertyValue<int>("assists"),
                        TimesInDreamteam                = p.GetPropertyValue<int>("dreamteam_count"),
                        YellowCards                     = p.GetPropertyValue<int>("yellow_cards"),
                        RedCards                        = p.GetPropertyValue<int>("red_cards"),
                        BonusPoints                     = p.GetPropertyValue<int>("bonus"),
                        Form                            = p.GetPropertyValue<double>("form"),
                        PenaltiesMissed                 = p.GetPropertyValue<int>("penalties_missed"),
                        PenaltiesSaved                  = p.GetPropertyValue<int>("penalties_scored"),
                        Saves                           = p.GetPropertyValue<int>("saves"),
                        TotalPoints                     = p.GetPropertyValue<int>("total_points"),
                        CleanSheets                     = p.GetPropertyValue<int>("clean_sheets"),
                        PointsPerGame                   = p.GetPropertyValue<double>("points_per_game"),
                        OwnGoals                        = p.GetPropertyValue<int>("own_goals"),


                        // todo:
                        //Appearances = "",
                        //Crosses = 
                        //Offsides = 
                        //PenaltiesScored = 
                        //Substitutions = 
                        //Shots = 
                    };

                    var teamID = p.GetPropertyValue<string>("team");
                    var team = teams.SingleOrDefault(x => x.ID == teamID);
                    if (team != null)
                    {
                        player.Team = team;
                        team.Players = team.Players.Append(player);
                    }
                    else
                    {
                        
                    }

                    // Update Team statistics   (todo: will get wrong if player changes team)
                    //team.Statistics.GoalsFor = player.Statistics.Goals;
                }
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
        
    }
}
