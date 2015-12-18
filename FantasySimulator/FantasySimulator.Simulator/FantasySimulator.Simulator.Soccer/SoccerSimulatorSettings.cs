using System.Collections.Generic;

namespace FantasySimulator.Simulator.Soccer
{
    public class SoccerSimulatorSettings : ISoccerSimulatorSettings
    {
        public static readonly SoccerSimulatorSettings Default;
        //private static readonly Analysers.PlayerAnalyserBase[] DefaultAnalysers;

        static SoccerSimulatorSettings()
        {
            //DefaultAnalysers = new List<Analysers.PlayerAnalyserBase>
            //{
            //    new Analysers.PlayerUnavailablePlayerAnalyser(),
            //    new Analysers.ChanceOfPlayingNextFixturePlayerAnalyser(),

            //}.ToArray();


            Default = new SoccerSimulatorSettings();
            //Default.PlayerAnalysers = new Analysers.PlayerAnalyserBase[DefaultAnalysers.Length];
            //DefaultAnalysers.CopyTo(Default.PlayerAnalysers, 0);
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

            //PlayerAnalysers = new Analysers.PlayerAnalyserBase[DefaultAnalysers.Length];
            //DefaultAnalysers.CopyTo(PlayerAnalysers, 0);
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
    }
}