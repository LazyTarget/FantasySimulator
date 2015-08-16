using System;
using System.Collections.Generic;
using System.Linq;
using FantasySimulator.Interfaces;
using FantasySimulator.Simulator.Soccer.Models;

namespace FantasySimulator.Simulator.Soccer
{
    public class SoccerSimulator : ISimulator
    {
        private SoccerSimulatorSettings _settings;


        public SoccerSimulator()
        {
            Settings = new SoccerSimulatorSettings();
        }


        public SoccerSimulatorSettings Settings
        {
            get { return _settings; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                _settings = value;
            }
        }



        public SoccerSimulationResult Simulate(SoccerSimulationData data)
        {
            var result = new SoccerSimulationResult();
            foreach (var gameweek in data.Gameweeks)
            {
                if (!result.PlayerResults.ContainsKey(gameweek))
                    result.PlayerResults.Add(gameweek, new List<SoccerSimulationPlayerResult>());

                var players = gameweek.GetPlayers();
                foreach (var player in players)
                {
                    var res = AnalysePlayerResult(player, gameweek);
                    result.PlayerResults[gameweek].Add(res);
                }
            }
            return result;
        }


        private FixtureOdds CalculateOdds(Fixture fixture)
        {
            var fixtureOdds = new FixtureOdds();
            fixtureOdds.Fixture = fixture;

            // todo: implement odds algorithm
            fixtureOdds.HomeWin = fixture.HomeTeam.Rating > fixture.AwayTeam.Rating ? 1 : 3;
            fixtureOdds.Draw    = fixture.HomeTeam.Rating == fixture.AwayTeam.Rating ? 1 : 5;
            fixtureOdds.AwayWin = fixture.AwayTeam.Rating > fixture.HomeTeam.Rating ? 1 : 3;
            return fixtureOdds;
        }



        

        private SoccerSimulationPlayerResult AnalysePlayerResult(Player player, Gameweek gameweek)
        {
            var res = new SoccerSimulationPlayerResult();
            res.Player = player;


            var fixtures = gameweek.Fixtures.Where(x => x.HomeTeam.ID == player.Team.ID || x.AwayTeam.ID == player.Team.ID);
            foreach (var fixture in fixtures)
            {
                if (!Settings.SimulateEndedGames)
                {
                    if (fixture.Statistics != null && fixture.Statistics.GameEnded)
                        continue;
                }

                var opposingTeam = fixture.GetOpposingTeam(player);

                var homeTeamAdvantage = fixture.HasHomeTeamAdvantage(player);
                var odds = CalculateOdds(fixture);
                var teamBetter = homeTeamAdvantage
                    ? odds.HomeWin < 2
                    : odds.AwayWin < 2;
                var teamWorse = homeTeamAdvantage
                    ? odds.HomeWin > 3
                    : odds.AwayWin > 3;


                // Algorithms
                // todo: give recommendation if is not Forward and tends to score/assist often
                // todo: give recommendation if is defender, but plays as mid and tends to score (points for CS and goals)


                if (homeTeamAdvantage)
                    res.AddRecommendation(RecommendationType.HomeTeamAdvantage, 1);

                if (teamBetter)
                    res.AddRecommendation(RecommendationType.FixtureRatingDiff, 1);
                else if (teamWorse)
                    res.AddRecommendation(RecommendationType.FixtureRatingDiff, -1);
            }
            return res;
        }
        
    }
}
