using System;
using System.Collections;
using System.Collections.Generic;
using FantasySimulator.Simulator.Soccer.Analysers;

namespace FantasySimulator.Simulator.Soccer
{
    public interface ISoccerSimulatorSettings
    {
        bool SimulateFinishedGames { get; }
        int TopRecommendationsPerPosition { get; }
        bool FilterUnavailablePlayers { get; }
        bool CalculateOddsWhenSimulating { get; }
        IList<PlayerAnalyserBase> PlayerAnalysers { get; }
        IList<TeamAnalyserBase> TeamAnalysers { get; }
        IFixtureOddsProvider FixtureOddsProvider { get; }
    }
}
