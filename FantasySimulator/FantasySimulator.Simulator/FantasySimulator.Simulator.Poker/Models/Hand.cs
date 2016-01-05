using System;
using System.Collections.Generic;

namespace FantasySimulator.Simulator.Poker.Models
{
    public class Hand
    {
        public IList<Card> Cards { get; set; }
        public HandStrength Strength { get; set; }
    }
}
