using System;
using System.Collections.Generic;
using System.Linq;
using FantasySimulator.Simulator.Poker.Models;

namespace FantasySimulator.Simulator.Poker
{
    public class HandCalculator
    {
        public IEnumerable<PlayerHand> CalculateAvailableHands(Player player, IList<Card> communityCards)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PossibleHand> CalculatePossibleHands(Player player, IList<Card> communityCards)
        {
            throw new NotImplementedException();
        }


        public PlayerHand GetBestHand(IList<PlayerHand> hands)
        {
            var hand = hands.OrderByDescending(x => (int) x.Strength).FirstOrDefault();
            // todo: order by best kicker
            return hand;
        }

    }
}
