using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using FantasySimulator.Core;
using FantasySimulator.Core.Diagnostics;
using FantasySimulator.Interfaces;
using FantasySimulator.Simulator.Soccer;

namespace FantasySimulator.DebugConsole.Data
{
    public class XmlLeagueDataFactory : XmlConfigFactoryBase<XmlLeagueDataFactory.SoccerSimulationDataXml>, ISoccerSimulationDataFactory
    {
        private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);
        
        public XmlLeagueDataFactory()
        {
            RootElementName = "soccerSimulatorData";
        }

        
        public async Task<SoccerSimulationData> Generate()
        {
            var data = new SoccerSimulationData();
            data = await GenerateApplyTo(data);
            return data;
        }

        public virtual async Task<SoccerSimulationData> GenerateApplyTo(SoccerSimulationData data)
        {
            data = await _GenerateApplyTo((SoccerSimulationDataXml) data);
            return data;
        }

        private async Task<SoccerSimulationData> _GenerateApplyTo(SoccerSimulationDataXml data)
        {
            using (var stream = GetConfigStream(ConfigUri))
            {
                ConfigureFromStream(stream, data);
            }
            return data;
        }


        protected override void ConfigureFromXml(XElement rootElement, SoccerSimulationDataXml result)
        {
            result.Configure(rootElement);
        }


        public class SoccerSimulationDataXml : SoccerSimulationData, IXmlConfigurable
        {
            private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);


            public SoccerSimulationDataXml()
            {
                
            }


            public virtual void Configure(XElement element)
            {
                var teamsElem = element.Elements("teams");
                if (teamsElem != null)
                {
                    var teamElems = teamsElem.Elements("team").Where(x => x != null).ToList();
                    if (teamElems.Any())
                    {
                        Teams = new Team[0];
                        foreach (var teamElem in teamElems)
                        {
                            var team = ParseTeam(teamElem);
                            Teams = Teams.Concat(new[] {team}).ToArray();
                        }
                    }
                }


                var leaguesElem = element.Element("leagues");
                if (leaguesElem != null)
                {
                    var leagueElems = leaguesElem.Elements("league").Where(x => x != null).ToList();
                    if (leagueElems.Any())
                    {
                        Leagues = new League[0];
                        foreach (var leagueElem in leagueElems)
                        {
                            var league = ParseLeague(leagueElem);
                            Leagues = Leagues.Concat(new[] { league }).ToArray();
                        }
                    }
                }


                // todo: update Team.Statistics after the gameweek results have been loaded?...
            }


            private Team ParseTeam(XElement element)
            {
                var team = new Team();
                team.ID = element.GetAttributeValue("id");
                team.Name = element.GetAttributeValue("name");
                team.ShortName = element.GetAttributeValue("shortname");
                team.Aliases =
                    element.GetAttributeValue("alias")
                        .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Trim())
                        .ToArray();
                
                var managerElem = element.Element("manager");
                if (managerElem != null)
                {
                    var manager = ParseManager(managerElem);
                    team.Manager = manager;
                }
                
                var playersElem = element.Element("players");
                if (playersElem != null)
                {
                    var playerElems = element.Elements("player").Where(x => x != null).ToList();
                    if (playerElems.Any())
                    {
                        team.Players = new Player[0];
                        foreach (var playerElem in playerElems)
                        {
                            var player = ParsePlayer(playerElem, team);
                            team.Players = team.Players.Concat(new[] { player }).ToArray();
                        }
                    }
                }

                var venueElem = element.Element("venue");
                if (venueElem != null)
                {
                    team.Stadium = new Venue
                    {
                        Name = venueElem.Element("name")?.GetAttributeValue("value").SafeConvert<string>(),
                        Capacity = venueElem.Element("capacity").GetAttributeValue("value").SafeConvert<int>(),
                        Attendance = null,
                    };
                }

                return team;
            }

            private Manager ParseManager(XElement element)
            {
                var manager = new Manager();
                manager.ID = element.GetAttributeValue("id");
                manager.DisplayName = element.GetAttributeValue("name");
                return manager;
            }


            private Player ParsePlayer(XElement element, Team team)
            {
                var player = new Player();
                player.ID = element.GetAttributeValue("id");
                player.DisplayName = element.GetAttributeValue("name");
                player.FirstName = element.GetAttributeValue("firstname");
                player.LastName = element.GetAttributeValue("lastname");
                //player.FullName = element.GetAttributeValue("fullname");
                player.Team = team;
                
                player.Age = element.GetAttributeValue("age").SafeConvert<int>();

                var birthday = element.GetAttributeValue("birthday");
                if (!string.IsNullOrWhiteSpace(birthday))
                {
                    DateTime birthdate;
                    if (DateTime.TryParse(birthday, out birthdate))
                    {
                        var ageSpan = DateTime.UtcNow.Subtract(birthdate);
                        var age = ageSpan.TotalDays / 365;
                        player.Age = (int) Math.Floor(age);
                    }
                }
                return player;
            }

            private League ParseLeague(XElement element)
            {
                var league = new League();
                league.ID = element.GetAttributeValue("id");
                league.Name = element.GetAttributeValue("name");
                league.Season = element.GetAttributeValue("season");


                var leagueTeamsElem = element.Elements("teams");
                if (leagueTeamsElem != null)
                {
                    var leagueTeamElems = leagueTeamsElem.Elements("team").Where(x => x != null).ToList();
                    if (leagueTeamElems.Any())
                    {
                        league.Teams = new LeagueTeam[0];
                        foreach (var leagueTeamElem in leagueTeamElems)
                        {
                            var teamName = leagueTeamElem.GetAttributeValue("name");
                            if (string.IsNullOrWhiteSpace(teamName))
                                continue;
                            var team = Teams?.FirstOrDefault(x => x.MatchName(teamName));
                            if (team == null)
                                continue;
                            
                            var leagueTeam = new LeagueTeam(league, team);
                            if (!league.Teams.Contains(leagueTeam))
                                league.Teams = league.Teams.Concat(new[] {leagueTeam}).ToArray();
                            if (!team.Leagues.Contains(league))
                                team.Leagues = team.Leagues.Concat(new[] {league}).ToArray();
                        }
                    }
                }


                var gameweeksElem = element.Element("gameweeks");
                if (gameweeksElem != null)
                {
                    var gameweekElems = gameweeksElem.Elements("gameweek").Where(x => x != null).ToList();
                    if (gameweekElems.Any())
                    {
                        league.Gameweeks = new Gameweek[0];
                        foreach (var gameweekElem in gameweekElems)
                        {
                            var gameweek = ParseGameweek(gameweekElem, league);
                            if (!league.Gameweeks.Contains(gameweek))
                                league.Gameweeks = league.Gameweeks.Concat(new[] {gameweek}).ToArray();
                        }
                    }
                }
                return league;
            }

            private Gameweek ParseGameweek(XElement element, League league)
            {
                var gameweek = new Gameweek(league);
                gameweek.Number = element.GetAttributeValue("number").SafeConvert<int>();


                var fixturesElem = element.Element("fixtures");
                if (fixturesElem != null)
                {
                    var fixtureElems = fixturesElem.Elements("fixture").Where(x => x != null).ToList();
                    if (fixtureElems.Any())
                    {
                        gameweek.Fixtures = new Fixture[0];
                        foreach (var fixtureElem in fixtureElems)
                        {
                            var fixture = ParseFixture(fixtureElem, gameweek);
                            if (!gameweek.Fixtures.Contains(fixture))
                                gameweek.Fixtures = gameweek.Fixtures.Concat(new[] {fixture}).ToArray();
                        }
                    }
                }
                return gameweek;
            }

            private Fixture ParseFixture(XElement element, Gameweek gameweek)
            {
                var fixture = new Fixture();
                fixture.Gameweek = gameweek;

                fixture.Time = element.Element("startTime")?.GetAttributeValue("value").SafeConvert<DateTime>() ?? DateTime.MinValue;

                var homeTeamName = element.Element("homeTeam")?.GetAttributeValue("name");
                var awayTeamName = element.Element("awayTeam")?.GetAttributeValue("name");
                fixture.HomeTeam = gameweek.League.Teams.FirstOrDefault(x => x.Team.MatchName(homeTeamName));
                fixture.AwayTeam = gameweek.League.Teams.FirstOrDefault(x => x.Team.MatchName(awayTeamName));


                FixtureScore score = new FixtureScore();

                var scoreElem = element.Element("score");
                if (scoreElem != null)
                {
                    score = FixtureScore.Create(
                        goalsForHomeTeam:
                            scoreElem.Element("goalsForHomeTeam")?.GetAttributeValue("value").SafeConvert<int>() ?? 0,
                        goalsForAwayTeam:
                            scoreElem.Element("goalsForAwayTeam")?.GetAttributeValue("value").SafeConvert<int>() ?? 0,
                        pointsForHomeTeam:
                            scoreElem.Element("pointsForHomeTeam")?.GetAttributeValue("value").SafeConvert<int>() ?? 0,
                        pointsForAwayTeam:
                            scoreElem.Element("pointsForAwayTeam")?.GetAttributeValue("value").SafeConvert<int>() ?? 0
                        );
                }


                var stateElem = element.Element("state");
                if (stateElem != null)
                {
                    fixture.Statistics = new FixtureStatistics
                    {
                        PlayedMinutes = stateElem.Element("time")?.GetAttributeValue("value").SafeConvert<int>() ?? 0,
                        Score = score,
                        GameFinished = stateElem.Element("ended")?.GetAttributeValue("value").SafeConvert<bool>() ?? false,
                    };
                }
                

                var oddsElem = element.Element("odds");
                if (oddsElem != null)
                {
                    fixture.Odds = new FixtureOdds
                    {
                        Fixture = fixture,
                        HomeWin = oddsElem.Element("homeTeamWin")?.GetAttributeValue("value").SafeConvert<double>() ?? 0,
                        Draw = oddsElem.Element("draw")?.GetAttributeValue("value").SafeConvert<double>() ?? 0,
                        AwayWin = oddsElem.Element("awayTeamWin")?.GetAttributeValue("value").SafeConvert<double>() ?? 0,
                    };
                }


                var venueElem = element.Element("venue");
                if (venueElem != null)
                {
                    fixture.Venue = new Venue
                    {
                        Name = venueElem.Element("name")?.GetAttributeValue("value").SafeConvert<string>(),
                        Capacity = venueElem.Element("capacity").GetAttributeValue("value").SafeConvert<int>(),
                        Attendance = venueElem.Element("attendance").GetAttributeValue("value").SafeConvert<int>(),
                    };
                }
                return fixture;
            }
        }


    }
}
