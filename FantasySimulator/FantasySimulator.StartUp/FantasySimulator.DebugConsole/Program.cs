using System;
using System.Collections.Generic;
using System.Linq;
using FantasySimulator.Simulator.Soccer;
using FantasySimulator.Simulator.Soccer.Models;

namespace FantasySimulator.DebugConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var simulationData = GetSimulationData();
            var simulator = new SoccerSimulator();
            var result = simulator.Simulate(simulationData);

            var top = 15;
            //foreach (var playerResult in result.PlayerResults.Cast<SoccerSimulationPlayerResult>().Take(top))
            //foreach (var playerResult in result.PlayerResults)
            //{
            //    Console.WriteLine("{0} recommendation points: {1}", playerResult.Player.Name, playerResult.RecommendationPoints);
            //}

            for (var i = 0; i < result.PlayerResults.Count; i++)
            {
                var gameweek = result.PlayerResults.Keys.Cast<Gameweek>().ElementAt(i);
                var players = (IEnumerable<SoccerSimulationPlayerResult>) result.PlayerResults.Values.ElementAt(i);
                var positionGroups = players.GroupBy(x => x.Player.Position);

                Console.WriteLine("Gameweek #{0}", gameweek.Number);
                foreach (var group in positionGroups)
                {
                    Console.WriteLine("Position: {0}", group.Key);
                    foreach (var playerRes in group)
                    {
                        Console.WriteLine("{0} [{1}] \t\t--- rec.pts: {2}", playerRes.Player.Name, playerRes.Player.Rating, playerRes.RecommendationPoints);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }

            Console.ReadLine();
        }


        private static SoccerSimulationData GetSimulationData()
        {
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
                            Name = "David Silva",
                            Rating = (Rating) 8,
                            Price = 9.0,
                            Position = PlayerPosition.Midfielder,
                        },

                        new Player
                        {
                            ID = "KOMPANY",
                            Name = "Vincent Kompany",
                            Rating = (Rating) 7,
                            Price = 7.0,
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
                            Name = "Eden Hazard",
                            Rating = (Rating) 9,
                            Price = 11.0,
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
                            Name = "Simon Mignolet",
                            Rating = (Rating) 7,
                            Price = 5.0,
                            Position = PlayerPosition.Goalkeeper,
                        },

                        new Player
                        {
                            ID = "HENDERSON",
                            Name = "Jordan Henderson",
                            Rating = (Rating) 7,
                            Price = 7.0,
                            Position = PlayerPosition.Midfielder,
                        },

                        new Player
                        {
                            ID = "COUTINHO",
                            Name = "Philipe Couthinho",
                            Rating = (Rating) 8,
                            Price = 8.5,
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
                            Name = "Matt Richie",
                            Rating = (Rating) 7,
                            Price = 5.0,
                            Position = PlayerPosition.Midfielder,
                        },

                        new Player
                        {
                            ID = "WILSON",
                            Name = "Calum Wilson",
                            Rating = (Rating) 6,
                            Price = 5.5,
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

            #endregion
            

            #region Gameweeks

            var gameweeks = new List<Gameweek>
            {
                new Gameweek
                {
                    Number = 1,
                    Fixtures = new List<Fixture>
                    {
                        new Fixture
                        {
                            HomeTeam = teams.Single(x => x.ID == "MCI"),
                            AwayTeam = teams.Single(x => x.ID == "CHE"),
                            Statistics = new FixtureStatistics
                            {
                                GameEnded = true,
                                PlayedMinutes = 90,
                                GoalsForHomeTeam = 2,
                                GoalsForAwayTeam = 2,
                            },
                        },

                        new Fixture
                        {
                            HomeTeam = teams.Single(x => x.ID == "LIV"),
                            AwayTeam = teams.Single(x => x.ID == "BOU"),
                            Statistics = new FixtureStatistics
                            {
                                GameEnded = true,
                                PlayedMinutes = 90,
                                GoalsForHomeTeam = 3,
                                GoalsForAwayTeam = 0,
                            },
                        },

                    }.ToArray(),
                },

                new Gameweek
                {
                    Number = 2,
                    Fixtures = new List<Fixture>
                    {
                        new Fixture
                        {
                            HomeTeam = teams.Single(x => x.ID == "BOU"),
                            AwayTeam = teams.Single(x => x.ID == "MCI"),
                        },

                        new Fixture
                        {
                            HomeTeam = teams.Single(x => x.ID == "CHE"),
                            AwayTeam = teams.Single(x => x.ID == "LIV"),
                        },
                    }.ToArray(),
                },

                new Gameweek
                {
                    Number = 3,
                    Fixtures = new List<Fixture>
                    {
                        new Fixture
                        {
                            HomeTeam = teams.Single(x => x.ID == "LIV"),
                            AwayTeam = teams.Single(x => x.ID == "MCI"),
                        },

                        new Fixture
                        {
                            HomeTeam = teams.Single(x => x.ID == "BOU"),
                            AwayTeam = teams.Single(x => x.ID == "CHE"),
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
            

            var simulationData = new SoccerSimulationData();
            simulationData.Teams = teams;
            simulationData.Gameweeks = gameweeks;
            return simulationData;
        }
    }
}
