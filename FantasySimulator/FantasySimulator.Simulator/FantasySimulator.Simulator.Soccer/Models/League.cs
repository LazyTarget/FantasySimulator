using System.Collections.Generic;
using System.Linq;

namespace FantasySimulator.Simulator.Soccer
{
    public class League
    {
        public League()
        {
            Teams = new List<Team>().ToArray();
            Gameweeks = new List<Gameweek>().ToArray();
            Table = new LeagueTable(this);
        }

        public string Name { get; set; }

        public Team[] Teams { get; set; }

        public Gameweek[] Gameweeks { get; set; }

        public LeagueTable Table { get; private set; }

        public int Year { get; set; }

        public int NumberOfTeams { get { return Teams?.Length ?? 0; } }

        public int NumberOfGames { get { return Gameweeks?.Sum(x => x.Fixtures?.Length ?? 0) ?? 0; } }
    }
}
