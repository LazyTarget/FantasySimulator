using System.Collections.Generic;

namespace FantasySimulator.Simulator.Poker.Models
{
    public class PossibleHand : PlayerHand
    {
        public IList<Hand> Combinations { get; set; }
        public double Probability { get; set; }
        public int Outs { get; set; }
    }
}
