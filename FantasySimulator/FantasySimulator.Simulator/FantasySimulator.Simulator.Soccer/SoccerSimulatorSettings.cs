namespace FantasySimulator.Simulator.Soccer
{
    public class SoccerSimulatorSettings : ISoccerSimulatorSettings
    {
        public static readonly SoccerSimulatorSettings Default;

        static SoccerSimulatorSettings()
        {
            Default = new SoccerSimulatorSettings
            {
                SimulateFinishedGames = true,
                TopRecommendationsPerPosition = 6,
                FilterUnavailablePlayers = true,
                IgnoreRecommendationTypes = null,
                //MinimumFixturesForPlaytimeRecommendationBonus = 3,
                //MinimumFixturesForFormRecommendationBonus = 3,
            };
        }
        
        public bool SimulateFinishedGames { get; set; }
        public int TopRecommendationsPerPosition { get; set; }
        public bool FilterUnavailablePlayers { get; set; }
        public RecommendationType[] IgnoreRecommendationTypes { get; set; }
        public int MinimumFixturesForPlaytimeRecommendationBonus { get; set; }
        public int MinimumFixturesForFormRecommendationBonus { get; set; }
    }
}