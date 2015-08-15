using System.Collections.Generic;
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
    }
}
