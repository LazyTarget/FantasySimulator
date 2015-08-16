namespace FantasySimulator.Simulator.Soccer
{
    public struct PlayerStatistics
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
            get { return PlayedMinutes > 0 ? (double)Goals / PlayedMinutes : 0; }
        }

        public int Assists { get; set; }

        public double AssistsPerMinute
        {
            get { return PlayedMinutes > 0 ? (double)Assists / PlayedMinutes : 0; }
        }

        public int OwnGoals { get; set; }

        public int CleanSheets { get; set; }

        public double Form { get; set; }

        public int BonusPoints { get; set; }

        public int TotalPoints { get; set; }

        public double PointsPerGame { get; set; }

        public int TimesInDreamteam { get; set; }

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
            get { return PenaltiesShoot > 0 ? (double) PenaltiesScored / PenaltiesShoot : 0; }
        }

    }
}
