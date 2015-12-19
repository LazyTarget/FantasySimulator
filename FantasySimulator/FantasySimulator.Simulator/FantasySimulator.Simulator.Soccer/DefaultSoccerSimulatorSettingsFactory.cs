using System;
using System.Collections.Generic;
using FantasySimulator.Simulator.Soccer.Analysers;

namespace FantasySimulator.Simulator.Soccer
{
    public class DefaultSoccerSimulatorSettingsFactory : ISoccerSimulatorSettingsFactory
    {
        private static readonly Lazy<ISoccerSimulatorSettings> Instance =
            new Lazy<ISoccerSimulatorSettings>(InitDefault);


        public ISoccerSimulatorSettings GetSettings()
        {
            return Instance.Value;
        }

        private static ISoccerSimulatorSettings InitDefault()
        {
            var settings = new SoccerSimulatorSettings();
            settings.TopRecommendationsPerPosition = 6;
            settings.FilterUnavailablePlayers = true;
            settings.CalculateOddsWhenSimulating = false;

            settings.PlayerAnalysers = InitDefaultPlayerAnalysers();
            settings.TeamAnalysers = InitDefaultTeamAnalysers();
            return settings;
        }

        public static IList<PlayerAnalyserBase> InitDefaultPlayerAnalysers()
        {
            var list = new List<PlayerAnalyserBase>();
            list.Add(new PlayerUnavailablePlayerAnalyser
            {
                Points = -10,
            });
            //list.Add(new ChanceOfPlayingNextFixturePlayerAnalyser
            //{
                
            //});
            return list;
        }

        public static IList<TeamAnalyserBase> InitDefaultTeamAnalysers()
        {
            var list = new List<TeamAnalyserBase>();
            list.Add(new HomeAdvantageTeamAnalyser
            {
                PointsWhenHomeTeam = 1,
                PointsWhenAwayTeam = -1,
            });
            return list;
        }

    }
}