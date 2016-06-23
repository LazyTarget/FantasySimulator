using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using FantasySimulator.Core;
using FantasySimulator.Interfaces;
using FantasySimulator.Simulator.Soccer;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FantasySimulator.DebugConsole.Data
{
    //[AutoConfigureProperties] todo:
    public class FantasyEuro2016DataFactory : ISoccerSimulationDataFactory, IHasProperties, IXmlConfigurable
    {
        private const string GetTeamsJsonUrl            = "http://eurofantasy.uefa.com/services/api/gameplay/teams?language=en";
        private const string GetFixturesJsonUrl         = "http://eurofantasy.uefa.com/services/api/gameplay/fixtures?language=en";
        private const string GetFormationsJsonUrl       = "http://eurofantasy.uefa.com/services/api/gameplay/formations";
        private const string GetPlayerJsonUrl           = "http://eurofantasy.uefa.com/services/api/gameplay/players?gamedayid=1&language=en";
        private const string GetPlayerDetailsJsonUrl    = "http://eurofantasy.uefa.com/services/api/feed/popupstats?playerId=1901409";
        private const string GetFantasyTeamJsonUrl      = "http://eurofantasy.uefa.com/services/api/gameplay/user/3404336E77F8C65CE0530100007F012B/team?vOptType=1&gamedayid=2&phaseid=1&language=en";
        private const string GetFantasyDailyJsonUrl     = "http://eurofantasy.uefa.com/services/api/daily/gameplay/user/3404336E77F8C65CE0530100007F012B/team?vOptType=2&gamedayid=2&phaseid=1&language=en";
        

        public FantasyEuro2016DataFactory()
        {
            Properties = new Dictionary<string, object>();

            UseOfflineData = false;
            DataTeamsFileName           = "EuroDataTeams.json";
            DataFixturesFileName        = "EuroDataFixtures.json";
            DataFormationsFileName      = "EuroDataFormations.json";
            DataPlayersFileName         = "EuroDataPlayers.json";
        }


        public IDictionary<string, object> Properties { get; private set; }


        public bool UseOfflineData
        {
            get { return Properties["UseOfflineData"].SafeConvert<bool>(); }
            set { Properties["UseOfflineData"] = value; }
        }

        public string DataTeamsFileName
        {
            get
            {
                var tmp = Properties["DataTeamsFileName"].SafeConvert<string>();
                if (!string.IsNullOrWhiteSpace(tmp) && tmp.IndexOf("{") >= 0)
                {
                    tmp = tmp.Replace("{DateTime}", DateTime.Now.ToString("yyyyMMdd_HHmm"));
                    Properties["DataTeamsFileName"] = tmp;
                }
                return tmp;
            }
            set { Properties["DataTeamsFileName"] = value; }
        }

        public string DataFixturesFileName
        {
            get
            {
                var tmp = Properties["DataFixturesFileName"].SafeConvert<string>();
                if (!string.IsNullOrWhiteSpace(tmp) && tmp.IndexOf("{") >= 0)
                {
                    tmp = tmp.Replace("{DateTime}", DateTime.Now.ToString("yyyyMMdd_HHmm"));
                    Properties["DataFixturesFileName"] = tmp;
                }
                return tmp;
            }
            set { Properties["DataFixturesFileName"] = value; }
        }

        public string DataFormationsFileName
        {
            get
            {
                var tmp = Properties["DataFormationsFileName"].SafeConvert<string>();
                if (!string.IsNullOrWhiteSpace(tmp) && tmp.IndexOf("{") >= 0)
                {
                    tmp = tmp.Replace("{DateTime}", DateTime.Now.ToString("yyyyMMdd_HHmm"));
                    Properties["DataFormationsFileName"] = tmp;
                }
                return tmp;
            }
            set { Properties["DataFormationsFileName"] = value; }
        }

        public string DataPlayersFileName
        {
            get
            {
                var tmp = Properties["DataPlayersFileName"].SafeConvert<string>();
                if (!string.IsNullOrWhiteSpace(tmp) && tmp.IndexOf("{") >= 0)
                {
                    tmp = tmp.Replace("{DateTime}", DateTime.Now.ToString("yyyyMMdd_HHmm"));
                    Properties["DataPlayersFileName"] = tmp;
                }
                return tmp;
            }
            set { Properties["DataPlayersFileName"] = value; }
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
                if (!UseOfflineData)
                {
                    var teamsUpdated        = await UpdateDataFileFromAPI(GetTeamsJsonUrl, DataTeamsFileName);
                    var fixturesUpdated     = await UpdateDataFileFromAPI(GetFixturesJsonUrl, DataFixturesFileName);
                    var formationsUpdated   = await UpdateDataFileFromAPI(GetFormationsJsonUrl, DataFormationsFileName);
                    var playersUpdated      = await UpdateDataFileFromAPI(GetPlayerJsonUrl, DataPlayersFileName);
                }

                var teamsJson       = await ReadDataFile(DataTeamsFileName);
                var fixturesJson    = await ReadDataFile(DataFixturesFileName);
                var formationsJson  = await ReadDataFile(DataFormationsFileName);
                var playersJson     = await ReadDataFile(DataPlayersFileName);



                var teams = new List<Team>();

                var euro2016 = GenerateLeague(teamsJson, fixturesJson, formationsJson, playersJson, ref teams);
                
                // todo: include in simulation: players playing in multiple leagues => tired, playtime, etc.


                data.Teams = (data.Teams ?? new Team[0]).Concat(teams).ToArray();
                data.Leagues = data.Leagues.Append(euro2016);
                //data.Leagues = data.Leagues.Append(premierLeague);
                return data;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private async Task<bool> UpdateDataFileFromAPI(string url, string fileName)
        {
            try
            {
                // Get fixtures
                var http = new HttpClient();
                var response = await http.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();
                
                // Write to file
                var path = Path.Combine(Environment.CurrentDirectory, "App_Data/SoccerSimulator/", fileName);
                var exists = File.Exists(path);
                var mode = exists ? FileMode.Truncate : FileMode.CreateNew;
                using (var stream = File.Open(path, mode, FileAccess.Write, FileShare.Read))
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


        private async Task<JObject> ReadDataFile(string fileName)
        {
            try
            {
                string json;
                var path = Path.Combine(Environment.CurrentDirectory, "App_Data/SoccerSimulator/", fileName);
                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
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

        
        


        private League GenerateLeague(JObject teamsJson, JObject fixturesJson, JObject formationsJson, JObject playersJson, ref List<Team> teams)
        {
            var league = new League
            {
                ID = "EURO",
                Name = "Euro 2016",
                Year = 2016,
            };
            var leagueTeams = new List<LeagueTeam>();
            var gameweeks = new List<Gameweek>();
            var positions = new Dictionary<int, PlayerPosition>();


            // Get playing formations
            positions[1] = PlayerPosition.Goalkeeper;
            positions[2] = PlayerPosition.Defender;
            positions[3] = PlayerPosition.Midfielder;
            positions[4] = PlayerPosition.Forward;

            var formationsData = formationsJson.Property("Data").Value.ToObject<JArray>();
            if (formationsData != null)
            {
                foreach (var formation in formationsData.Select(x => x.ToObject<JObject>()).Where(x => x != null))
                {
                    // todo: implement for calculation of valid formations
                }
            }


            // Get teams
            var teamsData = teamsJson.Property("Data").Value.ToObject<JArray>();
            if (teamsData != null)
            {
                foreach (var t in teamsData.Select(x => x.ToObjectOrDefault<JObject>()).Where(x => x != null))
                {
                    try
                    {
                        var teamID = t.GetPropertyValue<string>("Id");
                        if (string.IsNullOrWhiteSpace(teamID))
                            throw new FormatException("Invalid TeamID");

                        var team = teams.FirstOrDefault(x => x.ID == teamID);
                        if (team == null)
                        {
                            team = new Team
                            {
                                ID = teamID,
                                Name = t.GetPropertyValue<string>("webName"),
                                ShortName = t.GetPropertyValue<string>("ShortName"),
                                Aliases = new string[]
                                {
                                    t.GetPropertyValue<string>("webName"),
                                    t.GetPropertyValue<string>("OffName")
                                },

                                // todo: implement
                                //Rating = 
                                //Statistics = 
                            };
                            teams.Add(team);
                        }
                    
                        var leagueTeam = new LeagueTeam(league, team);
                        leagueTeams.Add(leagueTeam);
                        team.Leagues = team.Leagues.Append(league);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
            league.Teams = leagueTeams.ToArray();


            // Fixtures
            var fixturesData = fixturesJson.Property("Data").Value.ToObject<JArray>();
            foreach (var f in fixturesData)
            {
                try
                {
                    var fixture = new Fixture();

                    var obj = f.ToObjectOrDefault<JObject>();
                    fixture.Time = obj.GetPropertyValue<DateTime>("DateTime");

                    var homeTeamId = obj.GetPropertyValue<string>("HomeTeamId");
                    var awayTeamId = obj.GetPropertyValue<string>("AwayTeamId");
                    var homeTeam = leagueTeams.SingleOrDefault(x => x.Team.ID == homeTeamId);
                    var awayTeam = leagueTeams.SingleOrDefault(x => x.Team.ID == awayTeamId);
                    fixture.HomeTeam = homeTeam;
                    fixture.AwayTeam = awayTeam;
                    
                    var status = obj.GetPropertyValue<string>("MatchStatus");
                    var finished = status == "2";
                    var min = obj.GetPropertyValue<string>("MatchMinute").SafeConvert<int>();
                    if (finished && min <= 0)
                        min = 90;
                    var goalsHome = obj.GetPropertyValue<string>("HomeTeamScore").SafeConvert<int>();
                    var goalsAway = obj.GetPropertyValue<string>("AwayTeamScore").SafeConvert<int>();
                    fixture.Statistics = new FixtureStatistics
                    {
                        PlayedMinutes = min,
                        GameFinished = finished,
                        Score = FixtureScore.Create(goalsHome, goalsAway),
                    };


                    var matchday = obj.GetPropertyValue<int>("Gameday");
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
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            league.Gameweeks = gameweeks.ToArray();





            // Get players
            var playersData = playersJson.Property("Data").Value.ToObject<JArray>();
            foreach (var playerData in playersData.Select(x => x.ToObjectOrDefault<JObject>()).Where(x => x != null))
            {
                try
                {
                    var posID = playerData.GetPropertyValue<int>("skill");

                    // todo: load and update player (statistics), if player plays in multiple leagues
                
                    var valueStr = (playerData.GetPropertyValue<string>("value") ?? "0").Trim().Replace(",", ".");
                    double value;
                    double.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out value);

                    var availStatus = playerData.GetPropertyValue<string>("availStatus").SafeConvert<int>();
                    var playerStatus = playerData.GetPropertyValue<string>("playerStatus");

                    bool unavailable = false;
                    unavailable = unavailable || availStatus != 1;
                    //unavailable = unavailable || playerStatus == "D";       // if player is doubtful
                    unavailable = unavailable || playerStatus == "S";       // if player is suspended
                    unavailable = unavailable || playerStatus == "E";       // if player is eliminated
                    unavailable = unavailable || playerStatus == "I";       // if player is injured

                    double chanceOfPlaying = -1;
                    if (playerStatus == "D")
                    {
                        chanceOfPlaying = 25;
                    }

                    string news = null;
                    if (playerStatus == "S")
                        news = "Suspended";
                    else if (playerStatus == "I")
                        news = "Injured";
                    else if (playerStatus == "D")
                        news = "Doubtful";
                    else if (playerStatus == "E")
                        news = "Eliminated";


                    var player = new Player();
                    player.ID = playerData.GetPropertyValue<string>("Id");
                    player.FirstName = playerData.GetPropertyValue<string>("Name");
                    player.LastName = playerData.GetPropertyValue<string>("Surname");
                    player.DisplayName = playerData.GetPropertyValue<string>("webName");
                    player.FullName = playerData.GetPropertyValue<string>("webNameAlt");
                    
                    var teamID = playerData.GetPropertyValue<string>("teamId");
                    var leagueTeam = league.Teams.SingleOrDefault(x => x.Team.ID == teamID);
                    var team = leagueTeam?.Team;
                    player.Team = team;
                    if (team != null)
                    {
                        team.Players = team.Players.Append(player);
                    }

                    player.Fantasy                      = new FantasyPlayer
                    {
                        Position                        = positions[posID],
                        CurrentPrice                    = value,
                        OriginalPrice                   = value,
                        Unavailable                     = unavailable,
                        ChanceOfPlayingNextFixture      = -1,
                        News                            = news,
                        OwnagePercent                   = playerData.GetPropertyValue<double>("selectionPer"),
                        //TransfersDetailsForSeason       = new TransferDetails
                        //{
                        //    TransfersIn                 = GetProperty<int>(playerData, propMapping, "transfers_in"),
                        //    TransfersOut                = GetProperty<int>(playerData, propMapping, "transfers_out"),
                        //},
                        //TransfersDetailsForGW           = new TransferDetails
                        //{
                        //    TransfersIn                 = GetProperty<int>(playerData, propMapping, "transfers_in_event"),
                        //    TransfersOut                = GetProperty<int>(playerData, propMapping, "transfers_out_event"),
                        //},
                    };


                    var points = playerData.GetPropertyValue<double>("points");
                    var nrGamesTeamHasPlayed = leagueTeam?.PlayedGames ?? 0;

                    player.Statistics                   = new PlayerStatistics
                    {
                        PlayedMinutes                   = playerData.GetPropertyValue<int>("minutes"),
                        Goals                           = playerData.GetPropertyValue<int>("goalScored"),
                        Assists                         = playerData.GetPropertyValue<int>("goalAssist"),
                        //TimesInDreamteam                = playerData.GetPropertyValue<int>("dreamteam_count"),
                        YellowCards                     = playerData.GetPropertyValue<int>("yellowCard"),
                        RedCards                        = playerData.GetPropertyValue<int>("redCard"),
                        //BonusPoints                     = playerData.GetPropertyValue<int>("bonus"),
                        //Form                            = playerData.GetPropertyValue<double>("form"),
                        PenaltiesMissed                 = playerData.GetPropertyValue<int>("penaltyMissed"),
                        PenaltiesSaved                  = playerData.GetPropertyValue<int>("penaltySaved"),
                        PenaltiesScored                 = playerData.GetPropertyValue<int>("goalByPenalty"),
                        Saves                           = playerData.GetPropertyValue<int>("shotSaved"),
                        TotalPoints                     = (int)points,
                        CleanSheets                     = playerData.GetPropertyValue<int>("cleanSheet"),
                        PointsPerGame                   = points / Math.Max(1, nrGamesTeamHasPlayed),
                        OwnGoals                        = playerData.GetPropertyValue<int>("ownGoal"),

                    
                        // todo:
                        //Appearances = "",
                        //Crosses = 
                        //Offsides = 
                        //PenaltiesScored = 
                        //Substitutions = 
                        //Shots = 
                    };


                    // Update Team statistics   (todo: will get wrong if player changes team)
                    //team.Statistics.GoalsFor = player.Statistics.Goals;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            
            return league;
        }


        [Obsolete]
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
