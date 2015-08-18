using System;

namespace FantasySimulator.Simulator.Soccer
{
    public class LeagueTeam
    {
        public LeagueTeam(League league, Team team)
        {
            if (league == null)
                throw new ArgumentNullException(nameof(league));
            if (team == null)
                throw new ArgumentNullException(nameof(team));
            League = league;
            Team = team;
            Statistics = new TeamStatistics();
        }

        public League League { get; private set; }

        public Team Team { get; private set; }

        public TeamStatistics Statistics { get; private set; }


        public int PlayedGames  => Statistics?.PlayedGames  ?? 0;
        public int GamesWon     => Statistics?.WonGames     ?? 0;
        public int GamesDrawn   => Statistics?.DrawGames    ?? 0;
        public int GamesLost    => Statistics?.LostGames    ?? 0;
        public int Points       => Statistics?.Points       ?? 0;


        public override string ToString()
        {
            return Team.Name;
            //return string.Format("{1}p\t {2}|{3}|{4}  \t{0}", Team.Name, Points, GamesWon, GamesDrawn, GamesLost);
        }


        public void UpdateStatisticsBasedOnFixture(Fixture fixture)
        {
            // Statistics are updated only when the game has ended
            var fixtureWinner = fixture.GetFixtureLeader();
            if (fixtureWinner == FixtureWinner.Undetermined)
                return;


            var isHome = Team.ID == fixture.HomeTeam.Team.ID;
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
    }
}
