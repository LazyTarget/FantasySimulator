using System;
using System.Collections.Generic;
using System.Linq;

namespace FantasySimulator.Simulator.Poker.Models
{
    public abstract class Hand
    {
        public IList<Card> Cards { get; set; }

        public HandStrength Strength { get; set; }


        [Obsolete]
        public IEnumerable<Card> GetKickers()
        {
            return Cards.OrderByDescending(x => (int) x.Denomination);
        }
    }
}
