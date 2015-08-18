using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FantasySimulator.Simulator.Soccer
{
    public class LeagueTable : IEnumerable<LeagueTeam>
    {
        public LeagueTable(League league)
        {
            if (league == null)
                throw new ArgumentNullException(nameof(league));
            League = league;
        }

        public League League { get; private set; }


        public IEnumerator<LeagueTeam> GetEnumerator()
        {
            var result = League.Teams
                .OrderByDescending(x => x.Statistics.Points)
                .ThenByDescending(x => x.Statistics.WonGames)
                .ThenByDescending(x => x.Statistics.DrawGames)
                .ThenByDescending(x => x.Statistics.GoalsDifferance)
                .ThenByDescending(x => x.Statistics.GoalsFor)
                .ThenBy(x => x.Statistics.LostGames);
            return result.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string Print()
        {
            var tbl = string.Join(Environment.NewLine, 
                this.Select(x => string.Format("{1}p\t {2}|{3}|{4}  \t{0}", 
                                    x.Team.Name, x.Points, x.GamesWon, x.GamesDrawn, x.GamesLost)));
            System.Diagnostics.Debug.WriteLine(string.Format("LeagueTable '{0}'", League.Name));
            System.Diagnostics.Debug.WriteLine(tbl);
            return tbl;
        }
    }
}