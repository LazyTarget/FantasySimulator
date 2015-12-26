using System.Collections.Generic;
using System.Linq;

namespace FantasySimulator.Simulator.Soccer
{
    public class League
    {
        public League()
        {
            Teams = new List<LeagueTeam>().ToArray();
            Gameweeks = new List<Gameweek>().ToArray();
            Table = new LeagueTable(this);
        }

        public string ID { get; set; }

        public string Name { get; set; }

        public string Season { get; set; }

        public LeagueTeam[] Teams { get; set; }

        public Gameweek[] Gameweeks { get; set; }

        public LeagueTable Table { get; private set; }

        public int Year { get; set; }

        public int NumberOfTeams { get { return Teams?.Length ?? 0; } }

        public int NumberOfGames { get { return Gameweeks?.Sum(x => x.Fixtures?.Length ?? 0) ?? 0; } }

        public int NumberOfGameweeks { get { return Gameweeks?.Count() ?? 0; } }


        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(Season))
                return string.Format("{0} {1}", Name, Season);
            return string.Format("{0}", Name);
        }
    }
}
