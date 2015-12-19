using System;
using System.Collections.Generic;

namespace FantasySimulator.Simulator.Soccer
{
    public class SoccerSimulatorSettings : ISoccerSimulatorSettings
    {
        public SoccerSimulatorSettings()
        {
            //SimulateFinishedGames = true;
            TopRecommendationsPerPosition = 6;
            FilterUnavailablePlayers = true;
            CalculateOddsWhenSimulating = true;
            PlayerAnalysers = new List<Analysers.PlayerAnalyserBase>();
            TeamAnalysers = new List<Analysers.TeamAnalyserBase>();
        }
        
        public bool SimulateFinishedGames { get; set; }
        public int TopRecommendationsPerPosition { get; set; }
        public bool FilterUnavailablePlayers { get; set; }
        public bool CalculateOddsWhenSimulating { get; set; }
        public IList<Analysers.PlayerAnalyserBase> PlayerAnalysers { get; set; }
        public IList<Analysers.TeamAnalyserBase> TeamAnalysers { get; set; }
    }
}