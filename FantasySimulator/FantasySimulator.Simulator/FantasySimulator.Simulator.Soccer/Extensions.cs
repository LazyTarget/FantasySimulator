using FantasySimulator.Simulator.Soccer.Models;

namespace FantasySimulator.Simulator.Soccer
{
    public static class Extensions
    {
        public static bool HasHomeTeamAdvantage(this Fixture fixture, Player player)
        {
            if (player != null)
            {
                if (player.Team.ID == fixture.HomeTeam.ID)
                    return true;
            }
            return false;
        }


        public static Team GetOpposingTeam(this Fixture fixture, Player player)
        {
            if (player != null)
            {
                if (player.Team.ID == fixture.HomeTeam.ID)
                    return fixture.AwayTeam;
                if (player.Team.ID == fixture.AwayTeam.ID)
                    return fixture.HomeTeam;
            }
            return null;
        }


        public static FixtureWinner GetFixtureLeader(this Fixture fixture)
        {
            var res = FixtureWinner.None;
            if (fixture.Statistics != null)
            {
                if (!fixture.Statistics.GameEnded)
                    res = FixtureWinner.Undetermined;
                else
                {
                    if (fixture.Statistics.GoalsForHomeTeam == fixture.Statistics.GoalsForAwayTeam)
                        res = FixtureWinner.Draw;
                    else
                    {
                        if (fixture.Statistics.GoalsForHomeTeam > fixture.Statistics.GoalsForAwayTeam)
                            res = FixtureWinner.HomeTeam;
                        else
                            res = FixtureWinner.AwayTeam;
                    }
                }
            }
            return res;
        }

    }
}
