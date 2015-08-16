using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FantasySimulator.Simulator.Soccer
{
    public class LeagueTable : IEnumerable<Team>
    {
        private readonly Team[] _teams;
        private readonly IOrderedEnumerable<Team> _result;


        public LeagueTable(League league)
        {
            League = league;
            _teams = league.Teams;
            _result = _teams.OrderByDescending(x => x.Statistics.Points)
                            .ThenByDescending(x => x.Statistics.WonGames)
                            .ThenByDescending(x => x.Statistics.DrawGames)
                            .ThenByDescending(x => x.Statistics.GoalsDifferance)
                            .ThenByDescending(x => x.Statistics.GoalsFor)
                            .ThenBy(x => x.Statistics.LostGames);
        }

        public League League { get; private set; }


        public IEnumerator<Team> GetEnumerator()
        {
            return _result.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}