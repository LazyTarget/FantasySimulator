using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using FantasySimulator.Core;
using FantasySimulator.Interfaces;
using FantasySimulator.Simulator.Soccer;
using HtmlAgilityPack;
using Lux;
using Lux.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FantasySimulator.DebugConsole.Data
{
    //[AutoConfigureProperties] todo:
    public class FantasyPremierLeague2016DataFactory : ISoccerSimulationDataFactory, IHasProperties, IXmlConfigurable
    {
        private const string GetDataJsonUrl                         = "https://fantasy.premierleague.com/drf/bootstrap-static";
        private static readonly Uri GetBootstrapJsonUri             = new Uri("https://fantasy.premierleague.com/drf/bootstrap");
        private static readonly Uri GetBootstrapStaticJsonUri       = new Uri("https://fantasy.premierleague.com/drf/bootstrap-static");
        private static readonly Uri GetElementsJsonUri              = new Uri("https://fantasy.premierleague.com/drf/elements");
        private static readonly Uri GetElementTypesJsonUri          = new Uri("https://fantasy.premierleague.com/drf/element-types");
        private static readonly Uri GetRegionsJsonUri               = new Uri("https://fantasy.premierleague.com/drf/region");
        private static readonly Uri GetTeamsJsonUri                 = new Uri("https://fantasy.premierleague.com/drf/teams");
        private static readonly Uri GetEventsJsonUri                = new Uri("https://fantasy.premierleague.com/drf/events");
        private static readonly Uri GetFixturesJsonUri              = new Uri("https://fantasy.premierleague.com/drf/fixtures");
        private const string GetFixturesJsonUrl                     = "http://api.football-data.org/alpha/soccerseasons/426/fixtures";

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

        public string Username
        {
            get { return Properties["Username"].SafeConvert<string>(); }
            set { Properties["Username"] = value; }
        }

        public string Password
        {
            get { return Properties["Password"].SafeConvert<string>(); }
            set { Properties["Password"] = value; }
        }

        public string BootstrapFilename
        {
            get { return Properties["BootstrapFilename"].SafeConvert<string>(); }
            set { Properties["BootstrapFilename"] = value; }
        }

        public string BootstrapStaticFilename
        {
            get { return Properties["BootstrapStaticFilename"].SafeConvert<string>(); }
            set { Properties["BootstrapStaticFilename"] = value; }
        }

        public string ElementsFilename
        {
            get { return Properties["ElementsFilename"].SafeConvert<string>(); }
            set { Properties["ElementsFilename"] = value; }
        }

        public string ElementTypesFilename
        {
            get { return Properties["ElementTypesFilename"].SafeConvert<string>(); }
            set { Properties["ElementTypesFilename"] = value; }
        }

        public string RegionsFilename
        {
            get { return Properties["RegionsFilename"].SafeConvert<string>(); }
            set { Properties["RegionsFilename"] = value; }
        }

        public string TeamsFilename
        {
            get { return Properties["TeamsFilename"].SafeConvert<string>(); }
            set { Properties["TeamsFilename"] = value; }
        }

        public string EventsFilename
        {
            get { return Properties["EventsFilename"].SafeConvert<string>(); }
            set { Properties["EventsFilename"] = value; }
        }

        public string FixturesFilename
        {
            get { return Properties["FixturesFilename"].SafeConvert<string>(); }
            set { Properties["FixturesFilename"] = value; }
        }

        public string OpenFootballFixturesFilename
        {
            get { return Properties["OpenFootballFixturesFilename"].SafeConvert<string>(); }
            set { Properties["OpenFootballFixturesFilename"] = value; }
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
                    var eng2 = await openFootball.GetEnglishClubs2();
                    var wal = await openFootball.GetWelshClubs();

                    _ukClubs.AddRange(eng.Where(x => _ukClubs.All(y => !y.MatchName(x.Name))));
                    _ukClubs.AddRange(eng2.Where(x => _ukClubs.All(y => !y.MatchName(x.Name))));
                    _ukClubs.AddRange(wal.Where(x => _ukClubs.All(y => !y.MatchName(x.Name))));
                }
                catch (Exception ex)
                {
                    
                }



                var macros = new Dictionary<string, Func<string, string, object>>();
                macros["now"]       = (macro, format) => DateTime.UtcNow.ToString(format);
                macros["now-utc"]   = (macro, format) => DateTime.UtcNow.ToString(format);
                macros["now-local"] = (macro, format) => DateTime.Now.ToString(format);
                

                var bootstrapFilename               = _macroResolver.Resolve(BootstrapFilename, macros);
                var bootstrapStaticFilename         = _macroResolver.Resolve(BootstrapStaticFilename, macros);
                var elementsFilename                = _macroResolver.Resolve(ElementsFilename, macros);
                var elementTypesFilename            = _macroResolver.Resolve(ElementTypesFilename, macros);
                var regionsFilename                 = _macroResolver.Resolve(RegionsFilename, macros);
                var teamsFilename                   = _macroResolver.Resolve(TeamsFilename, macros);
                var eventsFilename                  = _macroResolver.Resolve(EventsFilename, macros);
                var fixturesFilename                = _macroResolver.Resolve(FixturesFilename, macros);
                var openFootballFixturesFilename    = _macroResolver.Resolve(OpenFootballFixturesFilename, macros);
                
                JToken bootstrapJson,
                       bootstrapStaticJson,
                       elementsJson,
                       elementTypesJson,
                       regionsJson,
                       teamsJson,
                       eventsJson,
                       fixturesJson,
                       openFootballFixturesJson;
                if (FetchNewData)
                {
                    bootstrapJson               = await GetJTokenFromAPI(GetBootstrapJsonUri);
                    bootstrapStaticJson         = await GetJTokenFromAPI(GetBootstrapStaticJsonUri);
                    elementsJson                = await GetJTokenFromAPI(GetElementsJsonUri);
                    elementTypesJson            = await GetJTokenFromAPI(GetElementTypesJsonUri);
                    regionsJson                 = await GetJTokenFromAPI(GetRegionsJsonUri);
                    teamsJson                   = await GetJTokenFromAPI(GetTeamsJsonUri);
                    eventsJson                  = await GetJTokenFromAPI(GetEventsJsonUri);
                    fixturesJson                = await GetJTokenFromAPI(GetFixturesJsonUri);
                    openFootballFixturesJson    = await GetOpenFootballFixturesFromWebsite();

                    var s1 = await SaveJTokenToTextFile(bootstrapJson,              bootstrapFilename);
                    var s2 = await SaveJTokenToTextFile(bootstrapStaticJson,        bootstrapStaticFilename);
                    var s3 = await SaveJTokenToTextFile(elementsJson,               elementsFilename);
                    var s4 = await SaveJTokenToTextFile(elementTypesJson,           elementTypesFilename);
                    var s5 = await SaveJTokenToTextFile(regionsJson,                regionsFilename);
                    var s6 = await SaveJTokenToTextFile(teamsJson,                  teamsFilename);
                    var s7 = await SaveJTokenToTextFile(eventsJson,                 eventsFilename);
                    var s8 = await SaveJTokenToTextFile(fixturesJson,               fixturesFilename);
                    var s9 = await SaveJTokenToTextFile(openFootballFixturesJson,   openFootballFixturesFilename);
                    var success = s1 && s2 && s3 && s4 && s5 && s6 && s7 && s8 && s9;
                }
                else
                {
                    bootstrapJson               = await LoadJTokenFromTextFile(bootstrapFilename);
                    bootstrapStaticJson         = await LoadJTokenFromTextFile(bootstrapStaticFilename);
                    elementsJson                = await LoadJTokenFromTextFile(elementsFilename);
                    elementTypesJson            = await LoadJTokenFromTextFile(elementTypesFilename);
                    regionsJson                 = await LoadJTokenFromTextFile(regionsFilename);
                    teamsJson                   = await LoadJTokenFromTextFile(teamsFilename);
                    eventsJson                  = await LoadJTokenFromTextFile(eventsFilename);
                    fixturesJson                = await LoadJTokenFromTextFile(fixturesFilename);
                    openFootballFixturesJson    = await LoadJTokenFromTextFile(openFootballFixturesFilename);
                }



                var leagueDataJson = (JObject) bootstrapStaticJson;
                var fixturesDataJson = (JObject) openFootballFixturesJson;

                var teams = new List<Team>();
                var premierLeague = GenerateLeague(leagueDataJson, fixturesDataJson, ref teams);


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
        
        public async Task<JToken> GetJTokenFromAPI(Uri uri)
        {
            try
            {
                var handler = new HttpClientHandler();
                //handler.Credentials = new NetworkCredential(Username, Password, uri.Host);
                handler.CookieContainer = new CookieContainer();

                var http = new HttpClient(handler);

                var request = new HttpRequestMessage(HttpMethod.Get, uri);
                
                var response = await http.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<JToken>(json);
                return token;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<JObject> GetOpenFootballFixturesFromWebsite()
        {
            var http = new HttpClient();
            try
            {
                var response = await http.GetAsync(GetFixturesJsonUrl);
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


        public async Task<bool> SaveJTokenToTextFile(JToken token, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                //throw new ArgumentNullException(nameof(fileName));
                return true;
            }
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
                    token.WriteTo(jw, converters);
                    await wr.FlushAsync();
                }
                System.Console.WriteLine("File saved: " + path);
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
            return false;
        }
        
        public async Task<JToken> LoadJTokenFromTextFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                //throw new ArgumentNullException(nameof(fileName));
                return null;
            }
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
                var token = JsonConvert.DeserializeObject<JToken>(json);
                return token;
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

                    object indexInfo = new PremierLeagueFantasyIndex
                    {
                        EaIndex = new Assignable<double>(p.GetPropertyValue<double>("ea_index")),
                        Influence = new Assignable<double>(p.GetPropertyValue<double>("influence")),
                        Creativity = new Assignable<double>(p.GetPropertyValue<double>("creativity")),
                        Threat = new Assignable<double>(p.GetPropertyValue<double>("threat")),
                    };

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
                        IndexInfo = indexInfo,
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
                        var homeTeam = leagueTeams.SingleOrDefault(x => x.Team.Name == homeTeamName || x.Team.MatchName(homeTeamName));
                        var awayTeam = leagueTeams.SingleOrDefault(x => x.Team.Name == awayTeamName || x.Team.MatchName(awayTeamName));
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
                        if (fixture.HomeTeam != null && fixture.AwayTeam != null)
                        {
                            fixture.HomeTeam.UpdateStatisticsBasedOnFixture(fixture);
                            fixture.AwayTeam.UpdateStatisticsBasedOnFixture(fixture);
                        }
                        else
                        {

                        }
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
