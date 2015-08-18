namespace FantasySimulator.Simulator.Soccer
{
    public struct FixtureStatistics
    {
        public bool GameStarted { get { return PlayedMinutes > 0; } }

        public bool GameFinished { get; set; }

        public int PlayedMinutes { get; set; }

        public FixtureScore Score { get; set; }


        public override string ToString()
        {
            if (GameFinished)
                return string.Format("{0}", Score);
            else if (!GameStarted)
                return "Not started";
            else
                return string.Format("{0} (min {1})", Score, PlayedMinutes);
        }
    }
}
