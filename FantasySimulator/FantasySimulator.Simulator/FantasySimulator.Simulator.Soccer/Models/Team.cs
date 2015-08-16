using System;
using System.Collections.Generic;

namespace FantasySimulator.Simulator.Soccer
{
    public class Team
    {
        public Team()
        {
            Players = new List<Player>().ToArray();
            Statistics = new TeamStatistics(this);
        }


        public string ID { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public Rating Rating { get; set; }

        public Player[] Players { get; set; }

        public TeamStatistics Statistics { get; private set; }



        public void UpdateStatisticsBasedOnFixture(Fixture fixture)
        {
            // Statistics are updated only when the game has ended
            var fixtureWinner = fixture.GetFixtureLeader();
            if (fixtureWinner == FixtureWinner.Undetermined)
                return;


            var isHome = ID == fixture.HomeTeam.ID;
            var opposingTeam = isHome ? fixture.AwayTeam : fixture.HomeTeam;

            // todo: validate that statistics hasn't already been applied?

            Statistics.PlayedGames++;

            Statistics.GoalsFor += isHome
                ? fixture.Statistics.Score.GoalsForHomeTeam
                : fixture.Statistics.Score.GoalsForAwayTeam;
            Statistics.GoalsConceded += isHome
                ? fixture.Statistics.Score.GoalsForAwayTeam
                : fixture.Statistics.Score.GoalsForHomeTeam;
            
            switch (fixtureWinner)
            {
                case FixtureWinner.HomeTeam:
                    if (isHome)
                        Statistics.WonGames++;
                    else
                        Statistics.LostGames++;
                    break;

                case FixtureWinner.AwayTeam:
                    if (isHome)
                        Statistics.LostGames++;
                    else
                        Statistics.WonGames++;
                    break;

                case FixtureWinner.Draw:
                    Statistics.DrawGames++;
                    break;

                case FixtureWinner.None:
                case FixtureWinner.Undetermined:
                    throw new NotSupportedException("Cannot update statistics, when game has not ended");

                default:
                    throw new InvalidOperationException();
            }
        }


        public override string ToString()
        {
            return Name;
        }
    }
}
