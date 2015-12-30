namespace FantasySimulator.Simulator.Poker.Models
{
    public class PossibleHand : PlayerHand
    {
        public double Probability { get; set; }
        public int Outs { get; set; }
    }
}
