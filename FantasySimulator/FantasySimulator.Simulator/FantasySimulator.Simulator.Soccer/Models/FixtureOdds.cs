namespace FantasySimulator.Simulator.Soccer
{
    public class FixtureOdds
    {
        public Fixture Fixture { get; set; }
        public Odds HomeWin { get; set; }
        public Odds Draw { get; set; }
        public Odds AwayWin { get; set; }
        public FixtureScore? PredictedScore { get; set; }

        public override string ToString()
        {
            return string.Format("1: {0}  |  X: {1}  |  2: {2}", HomeWin, Draw, AwayWin);
        }
    }
}
