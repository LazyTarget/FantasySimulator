namespace FantasySimulator.Simulator.Soccer.Models
{
    public class FixtureStatistics
    {
        public bool GameEnded { get; set; }

        public int PlayedMinutes { get; set; }

        public int GoalsForHomeTeam { get; set; }

        public int GoalsForAwayTeam { get; set; }


        public override string ToString()
        {
            if (GameEnded)
                return string.Format("{0}-{1}", GoalsForHomeTeam, GoalsForAwayTeam);
            else
                return string.Format("{0}-{1} (min {2})", GoalsForHomeTeam, GoalsForAwayTeam, PlayedMinutes);
        }
    }
}
