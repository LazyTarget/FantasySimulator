using System;
using System.Collections.Generic;
using System.Linq;

namespace FantasySimulator.Simulator.Soccer
{
    public static class ModelHelpers
    {
        public static bool HasHomeTeamAdvantage(this Player player, Fixture fixture)
        {
            if (player != null)
            {
                //if (player.Team.ID == fixture.HomeTeam.Team.ID)
                //    return true;
                return HasHomeTeamAdvantage(player.Team, fixture);
            }
            return false;
        }

        public static bool HasHomeTeamAdvantage(this Team team, Fixture fixture)
        {
            if (team.ID == fixture.HomeTeam.Team.ID)
                return true;
            return false;
        }


        public static LeagueTeam GetOpposingTeam(this Player player, Fixture fixture)
        {
            if (player != null)
            {
                if (player.Team.ID == fixture.HomeTeam.Team.ID)
                    return fixture.AwayTeam;
                if (player.Team.ID == fixture.AwayTeam.Team.ID)
                    return fixture.HomeTeam;
            }
            return null;
        }


        public static LeagueTeam GetLeagueTeam(this Player player, Fixture fixture)
        {
            return GetLeagueTeam(player, fixture.Gameweek);
        }

        public static LeagueTeam GetLeagueTeam(this Player player, Gameweek gameweek)
        {
            var league = player.Team.Leagues.FirstOrDefault(x => x.ID == gameweek.League.ID);
            var leaugeTeam = GetLeagueTeam(player, league);
            return leaugeTeam;
        }

        public static LeagueTeam GetLeagueTeam(this Player player, League league)
        {
            var leaugeTeam = league.Teams.FirstOrDefault(x => x.Team.ID == player.Team.ID);
            return leaugeTeam;
        }



        public static FixtureOutcome GetFixtureOutcome(this Fixture fixture)
        {
            var res = FixtureOutcome.None;
            if (!fixture.Statistics.GameFinished)
                res = FixtureOutcome.Undetermined;
            else
            {
                if (fixture.Statistics.Score.GoalsForHomeTeam == fixture.Statistics.Score.GoalsForAwayTeam)
                    res = FixtureOutcome.Draw;
                else
                {
                    if (fixture.Statistics.Score.GoalsForHomeTeam > fixture.Statistics.Score.GoalsForAwayTeam)
                        res = FixtureOutcome.HomeTeam;
                    else
                        res = FixtureOutcome.AwayTeam;
                }
            }
            return res;
        }

        public static TeamFixtureOutcome GetTeamFixtureOutcome(this LeagueTeam team, Fixture fixture)
        {
            var res = TeamFixtureOutcome.None;
            if (!fixture.Statistics.GameFinished)
                res = TeamFixtureOutcome.Undetermined;
            else
            {
                if (fixture.Statistics.Score.GoalsForHomeTeam == fixture.Statistics.Score.GoalsForAwayTeam)
                    res = TeamFixtureOutcome.Draw;
                else
                {
                    var teamIsHome = team.Team.ID == fixture.HomeTeam.Team.ID;
                    var teamIsAway = team.Team.ID == fixture.AwayTeam.Team.ID;

                    if (fixture.Statistics.Score.GoalsForHomeTeam > fixture.Statistics.Score.GoalsForAwayTeam)
                        res = teamIsHome
                            ? TeamFixtureOutcome.Win
                            : teamIsAway
                                ? TeamFixtureOutcome.Loss
                                : TeamFixtureOutcome.Undetermined;
                    else
                        res = teamIsHome
                            ? TeamFixtureOutcome.Loss
                            : teamIsAway
                                ? TeamFixtureOutcome.Win
                                : TeamFixtureOutcome.Undetermined;
                }
            }
            return res;
        }


        public static IEnumerable<Fixture> GetFixturesFromLeagueTeam(this LeagueTeam team)
        {
            var fixtures = team.League.Gameweeks
                .SelectMany(x => x.Fixtures)
                .Where(x => x.HomeTeam.Team.ID == team.Team.ID || x.AwayTeam.Team.ID == team.Team.ID);
            return fixtures;
        }

        [Obsolete("Improve method")]
        public static Fixture[] GetFixturesByTeam(this LeagueTeam team, IEnumerable<Fixture> fixtures)
        {
            var result = fixtures.Where(x => x.HomeTeam.Team.ID == team.Team.ID || x.AwayTeam.Team.ID == team.Team.ID);
            return result.ToArray();
        }

        
        public static IEnumerable<Fixture> GetRecentFixtures(this Team team, int count = 5)
        {
            var fixtures = team.Leagues
                .SelectMany(x => x.Gameweeks)
                .SelectMany(x => x.Fixtures)
                .Where(x => x.HomeTeam.Team.ID == team.ID || x.AwayTeam.Team.ID == team.ID)
                .Where(x => x.Statistics.GameStarted)
                //.Where(x => x.Statistics.GameFinished)
                .OrderByDescending(x => x.Time)
                .Take(count);
            return fixtures;
        }

        [Obsolete("Use specific implementation where is needed, where GameFinished, Count is specified...")]
        public static IEnumerable<Fixture> GetRecentFixtures(this LeagueTeam team, int count = 5)
        {
            var fixtures = GetFixturesFromLeagueTeam(team)
                .Where(x => x.Statistics.GameStarted)
                //.Where(x => x.Statistics.GameFinished)
                .OrderByDescending(x => x.Time)
                .Take(count);
            return fixtures;
        }




        public static int GetPlayedMinutesBeforeFixtureForTeam(this LeagueTeam team, Fixture fixture)
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
            var team = player.GetLeagueTeam(fixture);
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
