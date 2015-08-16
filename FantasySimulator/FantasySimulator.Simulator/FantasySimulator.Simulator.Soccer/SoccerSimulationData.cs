using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FantasySimulator.Simulator.Soccer.Models;

namespace FantasySimulator.Simulator.Soccer
{
    public class SoccerSimulationData
    {
        public SoccerSimulationData()
        {
            Teams = new List<Team>().ToArray();
            Gameweeks = new List<Gameweek>().ToArray();
        }

        public Team[] Teams { get; set; }

        public Gameweek[] Gameweeks { get; set; }
        

        public LeagueTable GetTable()
        {
            var tbl = new LeagueTable(Teams);
            return tbl;
        }
    }


    public class LeagueTable : IEnumerable<Team>
    {
        private readonly Team[] _teams;
        private readonly IOrderedEnumerable<Team> _result;

        public LeagueTable(Team[] teams)
        {
            _teams = teams;
            _result = _teams.Where(x => x.Statistics != null)
                            .OrderByDescending(x => x.Statistics.Points)
                            .ThenByDescending(x => x.Statistics.WonGames)
                            .ThenByDescending(x => x.Statistics.DrawGames)
                            .ThenByDescending(x => x.Statistics.GoalsDifferance)
                            .ThenByDescending(x => x.Statistics.GoalsFor)
                            .ThenBy(x => x.Statistics.LostGames);
        }

        
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
