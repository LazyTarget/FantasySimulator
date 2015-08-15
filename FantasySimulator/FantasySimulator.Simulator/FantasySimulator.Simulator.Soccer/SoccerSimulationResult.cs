using System.Collections.Generic;
using FantasySimulator.Simulator.Soccer.Models;

namespace FantasySimulator.Simulator.Soccer
{
    public class SoccerSimulationResult
    {
        public SoccerSimulationResult()
        {
            //PlayerResults = new List<SoccerSimulationPlayerResult>().ToArray();
            PlayerResults = new Dictionary<Gameweek, List<SoccerSimulationPlayerResult>>();
        }

        public IDictionary<Gameweek, List<SoccerSimulationPlayerResult>> PlayerResults { get; set; }
    }
}