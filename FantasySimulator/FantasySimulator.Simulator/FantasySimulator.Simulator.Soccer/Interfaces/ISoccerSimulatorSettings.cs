namespace FantasySimulator.Simulator.Soccer
{
    public interface ISoccerSimulatorSettings
    {
        bool SimulateFinishedGames { get; }
        int TopRecommendationsPerPosition { get; }
        bool FilterUnavailablePlayers { get; }
        RecommendationType[] IgnoreRecommendationTypes { get; }
        int MinimumFixturesForPlaytimeRecommendationBonus { get; }
        int MinimumFixturesForFormRecommendationBonus { get; }
        int LengthOfFormWhenSimulating { get; }
        bool CalculateOddsWhenSimulating { get; }
    }
}
