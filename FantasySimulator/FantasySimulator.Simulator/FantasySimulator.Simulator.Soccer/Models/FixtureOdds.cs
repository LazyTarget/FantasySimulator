namespace FantasySimulator.Simulator.Soccer.Models
{
    public class FixtureOdds
    {
        public Fixture Fixture { get; set; }
        public Odds HomeWin { get; set; }
        public Odds Draw { get; set; }
        public Odds AwayWin { get; set; }

        public override string ToString()
        {
            return string.Format("1: {0}, X: {1}, 2: {2}", HomeWin, Draw, AwayWin);
        }
    }
}
