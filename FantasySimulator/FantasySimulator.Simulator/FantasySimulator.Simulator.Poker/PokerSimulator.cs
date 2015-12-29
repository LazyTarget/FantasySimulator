using System;
using System.Linq;
using FantasySimulator.Interfaces;
using FantasySimulator.Simulator.Poker.Models;

namespace FantasySimulator.Simulator.Poker
{
    public class PokerSimulator : ISimulator
    {
        public PokerSimulator()
        {
            HandCalculator = new HandCalculator();
        }

        public HandCalculator HandCalculator { get; set; }


        public PokerSimulationResult Simulate(PokerSimulationData data)
        {
            var result = new PokerSimulationResult();
            result.Results = new PokerGameResult[0];

            foreach (var pokerGame in data.Games)
            {
                if (pokerGame.PokerType == PokerType.TexasHoldEm)
                {
                    var res = SimulateTexasHoldEmGame(pokerGame);
                    result.Results = result.Results.Concat(new[] { res }).ToArray();
                }
                else
                {
                    throw new NotImplementedException($"PokerType '{pokerGame.PokerType}' not implemented");
                }
            }
            return result;
        }


        private PokerGameResult SimulateTexasHoldEmGame(PokerGame pokerGame)
        {
            var result = new PokerGameResult();
            result.Game = pokerGame;
            result.BestAvailableHands = new PlayerHand[0];
            result.BestPossibleHands = new PossibleHand[0];
            
            foreach (var player in pokerGame.Players)
            {
                var playerAvailableHands = HandCalculator.CalculateAvailableHands(player, pokerGame.CommunityCards);
                var playerBestAvailable = playerAvailableHands.FirstOrDefault();
                if (playerBestAvailable != null)
                {
                    result.BestAvailableHands = result.BestAvailableHands.Concat(new[] { playerBestAvailable }).ToArray();
                }
                
                var playerPossibleHands = HandCalculator.CalculatePossibleHands(player, pokerGame.CommunityCards);
                var playerBestPossible = playerPossibleHands.FirstOrDefault();
                if (playerBestPossible != null)
                {
                    result.BestPossibleHands = result.BestPossibleHands.Concat(new[] { playerBestPossible }).ToArray();
                }
            }
            return result;
        }
    }
}
