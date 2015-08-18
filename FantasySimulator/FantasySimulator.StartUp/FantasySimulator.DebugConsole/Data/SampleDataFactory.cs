using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FantasySimulator.Simulator.Soccer;

namespace FantasySimulator.DebugConsole.Data
{
    public class SampleDataFactory : ISoccerSimulationDataFactory
    {
        public async Task<SoccerSimulationData> Generate()
        {
            var league = new League
            {
                Name = "Sample League",
                Year = DateTime.Now.Year,
            };

            #region Players

            var teams = new List<Team>
            {
                new Team
                {
                    ID = "MCI",
                    Name = "Manchester City",
                    Rating = (Rating) 9,
                    Players = new List<Player>
                    {
                        new Player
                        {
                            ID = "SILVA",
                            DisplayName = "David Silva",
                            Rating = (Rating) 8,
                            CurrentPrice = 9.0,
                            Position = PlayerPosition.Midfielder,
                        },

                        new Player
                        {
                            ID = "KOMPANY",
                            DisplayName = "Vincent Kompany",
                            Rating = (Rating) 7,
                            CurrentPrice = 7.0,
                            Position = PlayerPosition.Defender,
                        },
                    }.ToArray(),
                },


                new Team
                {
                    ID = "CHE",
                    Name = "Chelsea",
                    Rating = (Rating) 9,
                    Players = new List<Player>
                    {
                        new Player
                        {
                            ID = "HAZARD",
                            DisplayName = "Eden Hazard",
                            Rating = (Rating) 9,
                            OriginalPrice = 11.0,
                            CurrentPrice = 11.0,
                            Position = PlayerPosition.Midfielder,
                        },

                    }.ToArray(),
                },


                new Team
                {
                    ID = "LIV",
                    Name = "Liverpool",
                    Rating = (Rating) 8,
                    Players = new List<Player>
                    {
                        new Player
                        {
                            ID = "MIGNOLET",
                            DisplayName = "Simon Mignolet",
                            Rating = (Rating) 7,
                            OriginalPrice = 5.0,
                            CurrentPrice = 5.0,
                            Position = PlayerPosition.Goalkeeper,
                        },

                        new Player
                        {
                            ID = "HENDERSON",
                            DisplayName = "Jordan Henderson",
                            Rating = (Rating) 7,
                            OriginalPrice = 7.0,
                            CurrentPrice = 7.0,
                            Position = PlayerPosition.Midfielder,
                        },

                        new Player
                        {
                            ID = "COUTINHO",
                            DisplayName = "Philipe Couthinho",
                            Rating = (Rating) 8,
                            OriginalPrice = 8.5,
                            CurrentPrice = 8.5,
                            Position = PlayerPosition.Midfielder,
                        },

                    }.ToArray(),
                },


                new Team
                {
                    ID = "BOU",
                    Name = "AFC Bournemouth",
                    Rating = (Rating) 4,
                    Players = new List<Player>
                    {
                        new Player
                        {
                            ID = "RICHE",
                            DisplayName = "Matt Richie",
                            Rating = (Rating) 7,
                            OriginalPrice = 5.0,
                            CurrentPrice = 5.0,
                            Position = PlayerPosition.Midfielder,
                        },

                        new Player
                        {
                            ID = "WILSON",
                            DisplayName = "Calum Wilson",
                            Rating = (Rating) 6,
                            OriginalPrice = 5.5,
                            CurrentPrice = 5.5,
                            Position = PlayerPosition.Forward,
                        },

                    }.ToArray(),
                },

            }.ToArray();


            foreach (var team in teams)
            {
                foreach (var player in team.Players)
                {
                    player.Team = team;
                }
            }

            var leagueTeams = teams.Select(x => new LeagueTeam(league, x)).ToList();

            #endregion
            

            #region Gameweeks

            var gameweeks = new List<Gameweek>
            {
                new Gameweek(league)
                {
                    Number = 1,
                    Fixtures = new List<Fixture>
                    {
                        new Fixture
                        {
                            HomeTeam = leagueTeams.Single(x => x.Team.ID == "MCI"),
                            AwayTeam = leagueTeams.Single(x => x.Team.ID == "CHE"),
                            Statistics = new FixtureStatistics
                            {
                                GameFinished = true,
                                PlayedMinutes = 90,
                                Score = new FixtureScore
                                {
                                    GoalsForHomeTeam = 2,
                                    GoalsForAwayTeam = 2,
                                },
                            },
                        },

                        new Fixture
                        {
                            HomeTeam = leagueTeams.Single(x => x.Team.ID == "LIV"),
                            AwayTeam = leagueTeams.Single(x => x.Team.ID == "BOU"),
                            Statistics = new FixtureStatistics
                            {
                                GameFinished = false,
                                PlayedMinutes = 74,
                                Score = new FixtureScore
                                {
                                    GoalsForHomeTeam = 3,
                                    GoalsForAwayTeam = 0,
                                },
                            },
                        },

                    }.ToArray(),
                },

                new Gameweek(league)
                {
                    Number = 2,
                    Fixtures = new List<Fixture>
                    {
                        new Fixture
                        {
                            HomeTeam = leagueTeams.Single(x => x.Team.ID == "BOU"),
                            AwayTeam = leagueTeams.Single(x => x.Team.ID == "MCI"),
                        },

                        new Fixture
                        {
                            HomeTeam = leagueTeams.Single(x => x.Team.ID == "CHE"),
                            AwayTeam = leagueTeams.Single(x => x.Team.ID == "LIV"),
                        },
                    }.ToArray(),
                },

                new Gameweek(league)
                {
                    Number = 3,
                    Fixtures = new List<Fixture>
                    {
                        new Fixture
                        {
                            HomeTeam = leagueTeams.Single(x => x.Team.ID == "LIV"),
                            AwayTeam = leagueTeams.Single(x => x.Team.ID == "MCI"),
                        },

                        new Fixture
                        {
                            HomeTeam = leagueTeams.Single(x => x.Team.ID == "BOU"),
                            AwayTeam = leagueTeams.Single(x => x.Team.ID == "CHE"),
                        },
                    }.ToArray(),
                },

            }.ToArray();


            foreach (var week in gameweeks)
            {
                foreach (var fixture in week.Fixtures)
                {
                    fixture.Gameweek = week;
                }
            }

            #endregion


            league.Teams = leagueTeams.ToArray();
            league.Gameweeks = gameweeks;

            var simulationData = new SoccerSimulationData();
            simulationData.Leagues = simulationData.Leagues.Append(league);
            //simulationData.Teams = teams;
            //simulationData.Gameweeks = gameweeks;
            return simulationData;
        }
    }
}
