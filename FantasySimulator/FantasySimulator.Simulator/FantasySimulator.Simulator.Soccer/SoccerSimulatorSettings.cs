using System.Collections.Generic;

namespace FantasySimulator.Simulator.Soccer
{
    public class SoccerSimulatorSettings : ISoccerSimulatorSettings
    {
        public static readonly SoccerSimulatorSettings Default;

        static SoccerSimulatorSettings()
        {
            Default = new SoccerSimulatorSettings
            {
                PlayerAnalysers = new List<Analysers.PlayerAnalyserBase>
                {
                    new Analysers.HomeTeamAdvantagePlayerAnalyser(),
                },
            };
        }


        public SoccerSimulatorSettings()
        {
            //SimulateFinishedGames = true;
            TopRecommendationsPerPosition = 6;
            FilterUnavailablePlayers = true;
            IgnoreRecommendationTypes = null;
            //MinimumFixturesForPlaytimeRecommendationBonus = 3;
            //MinimumFixturesForFormRecommendationBonus = 3;
            LengthOfFormWhenSimulating = 5;
            CalculateOddsWhenSimulating = true;
        }
        
        public bool SimulateFinishedGames { get; set; }
        public int TopRecommendationsPerPosition { get; set; }
        public bool FilterUnavailablePlayers { get; set; }
        public RecommendationType[] IgnoreRecommendationTypes { get; set; }
        public int MinimumFixturesForPlaytimeRecommendationBonus { get; set; }
        public int MinimumFixturesForFormRecommendationBonus { get; set; }
        public int LengthOfFormWhenSimulating { get; set; }
        public bool CalculateOddsWhenSimulating { get; set; }
        public IList<Analysers.PlayerAnalyserBase> PlayerAnalysers { get; set; }


        public static SoccerSimulatorSettings FromConfig()
        {
            var settings = new SoccerSimulatorSettings();


            return settings;
        }
    }
}