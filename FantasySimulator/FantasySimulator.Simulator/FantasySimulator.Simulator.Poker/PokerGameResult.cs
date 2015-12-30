using System.Collections.Generic;
using FantasySimulator.Simulator.Poker.Models;

namespace FantasySimulator.Simulator.Poker
{
    public class PokerGameResult
    {
        public PokerGameResult()
        {
            BestAvailableHands = new PlayerHand[0];
            BestPossibleHands = new PossibleHand[0];
            PlayerWinPercentages = new Dictionary<Player, double>();
        }

        public PokerGame Game { get; set; }

        public PlayerHand[] BestAvailableHands { get; set; }

        public PossibleHand[] BestPossibleHands { get; set; }

        public IDictionary<Player, double> PlayerWinPercentages { get; set; } 
    }
}
