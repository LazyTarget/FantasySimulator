using System.Collections.Generic;

namespace FantasySimulator.Simulator.Soccer.Models
{
    public class Team
    {
        public Team()
        {
            Players = new List<Player>().ToArray();
        }


        public string ID { get; set; }

        public string Name { get; set; }

        public Rating Rating { get; set; }

        public Player[] Players { get; set; }


        public override string ToString()
        {
            return Name;
        }
    }
}
