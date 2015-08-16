namespace FantasySimulator.Simulator.Soccer.Models
{
    public class TeamStatistics
    {
		public int PlayedGames { get; set; }

		public int WonGames { get; set; }

		public int DrawGames { get; set; }

		public int LostGames { get; set; }

		public int Points { get { return (WonGames*3) + (DrawGames*1) + (LostGames*0); } }

		public int GoalsFor { get; set; }

		public int GoalsConceded { get; set; }

        public int GoalsDifferance
        {
            get { return GoalsFor - GoalsConceded; }
        }


        public double GoalsPerMatch
        {
            get { return (double) GoalsFor/PlayedGames; }
        }

        public double GoalsConcededPerMatch
        {
            get { return (double) GoalsConceded/PlayedGames; }
        }

		public int LongestUnbeatenStreak { get; set; }

		public double CleanSheetPercentage { get; set; }
    }
}
