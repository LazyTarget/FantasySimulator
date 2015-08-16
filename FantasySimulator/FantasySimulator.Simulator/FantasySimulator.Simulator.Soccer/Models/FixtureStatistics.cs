namespace FantasySimulator.Simulator.Soccer
{
    public struct FixtureStatistics
    {
        public bool GameFinished { get; set; }

        public int PlayedMinutes { get; set; }

        public FixtureScore Score { get; set; }


        public override string ToString()
        {
            if (GameFinished || PlayedMinutes <= 0)
                return "Not started";
            else
                return string.Format("{0} (min {1})", Score, PlayedMinutes);
        }
    }
}
