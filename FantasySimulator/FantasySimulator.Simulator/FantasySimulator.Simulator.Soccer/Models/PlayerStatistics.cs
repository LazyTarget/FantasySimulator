namespace FantasySimulator.Simulator.Soccer.Models
{
    public class PlayerStatistics
    {
        /// <summary>
        /// Times on the field
        /// </summary>
        public int Appearances { get; set; }

        /// <summary>
        /// Times jumped in as a substitute
        /// </summary>
        public int Substitutions { get; set; }
        
        public int PlayedMinutes { get; set; }

        public int Goals { get; set; }

        public double GoalsPerMinute
        {
            get { return (double)Goals / PlayedMinutes; }
        }

        public int Assists { get; set; }

        public double AssistsPerMinute
        {
            get { return (double)Assists / PlayedMinutes; }
        }

        public int YellowCards { get; set; }

        public int RedCards { get; set; }

        public int Shots { get; set; }

        public int Crosses { get; set; }

        public int Offsides { get; set; }

        public int PenaltiesSaved { get; set; }

        public int PenaltiesShoot { get { return PenaltiesScored + PenaltiesMissed; } }

        public int PenaltiesScored { get; set; }

        public int PenaltiesMissed { get; set; }

        public double PenaltyScorePercentage
        {
            get { return (double) PenaltiesScored/PenaltiesShoot; }
        }

    }
}
