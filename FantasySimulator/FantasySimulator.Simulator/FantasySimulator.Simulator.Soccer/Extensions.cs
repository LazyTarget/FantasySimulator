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

    }
}
