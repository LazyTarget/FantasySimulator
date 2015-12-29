using FantasySimulator.Simulator.Poker.Models;

namespace FantasySimulator.Simulator.Poker
{
    public class PokerGameResult
    {
        public PokerGame Game { get; set; }

        public PlayerHand[] BestAvailableHands { get; set; }

        public PossibleHand[] BestPossibleHands { get; set; }
    }
}
