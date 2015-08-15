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
            fixtureOdds.HomeWin = (Odds) (fixture.HomeTeam.Rating > fixture.AwayTeam.Rating ? 1 : 3);
            fixtureOdds.Draw    = (Odds) (fixture.HomeTeam.Rating == fixture.AwayTeam.Rating ? 1 : 5);
            fixtureOdds.AwayWin = (Odds) (fixture.AwayTeam.Rating > fixture.HomeTeam.Rating ? 1 : 3);
            return fixtureOdds;
        }



        

        private SoccerSimulationPlayerResult AnalysePlayerResult(Player player, Gameweek gameweek)
        {
            var res = new SoccerSimulationPlayerResult();
            res.Player = player;


            var fixtures = gameweek.Fixtures.Where(x => x.HomeTeam.ID == player.Team.ID || x.AwayTeam.ID == player.Team.ID);
            foreach (var fixture in fixtures)
            {
                var opposingTeam = fixture.GetOpposingTeam(player);

                var homeTeamAdvantage = fixture.HasHomeTeamAdvantage(player);
                var odds = CalculateOdds(fixture);
                var teamBetter = homeTeamAdvantage
                    ? odds.HomeWin < 2
                    : odds.AwayWin < 2;


                if (teamBetter)
                    res.Recommendations.Add(RecommendationType.FixtureRatingDiff, 1);
            }
            return res;
        }
        
    }
}
