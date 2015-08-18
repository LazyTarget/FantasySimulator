using System;
using System.Collections.Generic;
using System.Linq;

namespace FantasySimulator.Simulator.Soccer
{
    public class Team
    {
        public Team()
        {
            Players = new List<Player>().ToArray();
        }


        public string ID { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public Rating Rating { get; set; }

        public Player[] Players { get; set; }

        public League[] Leagues { get; set; }


        public TeamStatistics GetFullStatistics()
        {
            var res = new TeamStatistics();
            foreach (var league in Leagues)
            {
                var team = league.Teams.FirstOrDefault(x => x.Team.ID == ID);
                if (team == null)
                    throw new InvalidOperationException("Team is not in league");
                res += team.Statistics;
            }
            return res;
        }
        
        
        public override string ToString()
        {
            return Name;
        }
    }
}
