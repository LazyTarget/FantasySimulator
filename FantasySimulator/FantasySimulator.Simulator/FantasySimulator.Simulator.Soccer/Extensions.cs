using System.Collections.Generic;
using System.Linq;

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
            if (!fixture.Statistics.GameFinished)
                res = FixtureWinner.Undetermined;
            else
            {
                if (fixture.Statistics.Score.GoalsForHomeTeam == fixture.Statistics.Score.GoalsForAwayTeam)
                    res = FixtureWinner.Draw;
                else
                {
                    if (fixture.Statistics.Score.GoalsForHomeTeam > fixture.Statistics.Score.GoalsForAwayTeam)
                        res = FixtureWinner.HomeTeam;
                    else
                        res = FixtureWinner.AwayTeam;
                }
            }
            return res;
        }


        public static Fixture[] GetFixturesByTeam(this Team team, IEnumerable<Fixture> fixtures)
        {
            var result = fixtures.Where(x => x.HomeTeam.ID == team.ID || x.AwayTeam.ID == team.ID);
            return result.ToArray();
        }


        public static int GetPlayedMinutesBeforeFixtureForTeam(this Team team, Fixture fixture)
        {
            var playedMinutes = 0;
            var league = fixture.Gameweek.League;
            for (var i = 0; i < league.Gameweeks.Length; i++)
            {
                var gw = league.Gameweeks[i];
                if (gw.Number > fixture.Gameweek.Number)
                    break;
                
                var fixtures = team.GetFixturesByTeam(gw.Fixtures).OrderBy(x => x.Time);
                foreach (var fix in fixtures)
                {
                    if (fix == fixture)
                        break;
                    playedMinutes += fix.Statistics.PlayedMinutes;
                }
            }
            return playedMinutes;
        }

        public static int GetPlayedMinutesBeforeFixtureForPlayer(this Player player, Fixture fixture)
        {
            var playedMinutes = 0;
            var league = fixture.Gameweek.League;
            var team = player.Team;
            for (var i = 0; i < league.Gameweeks.Length; i++)
            {
                var gw = league.Gameweeks[i];
                if (gw.Number > fixture.Gameweek.Number)
                    break;

                var fixtures = team.GetFixturesByTeam(gw.Fixtures).OrderBy(x => x.Time);
                foreach (var fix in fixtures)
                {
                    if (fix == fixture)
                        break;
                    var stats = fix.GetPlayerStats(player);
                    var min = stats != null && stats.PlayedMinutes > 0 ? stats.PlayedMinutes : 0;
                    playedMinutes += min;
                }
            }
            return playedMinutes;
        }

    }
}
