namespace FantasySimulator.Simulator.Soccer
{
    public struct FantasyPlayer
    {
        public PlayerPosition Position { get; set; }

        public double OriginalPrice { get; set; }

        public double CurrentPrice { get; set; }

        public double PriceChange => CurrentPrice - OriginalPrice;

        public TransferDetails TransfersDetailsForSeason { get; set; }

        public TransferDetails TransfersDetailsForGW { get; set; }

        public double OwnagePercent { get; set; }

        public bool Unavailable { get; set; }

        /// <summary>
        /// Formatted in decimals, ex: 0.75 = 75 %
        /// </summary>
        public double ChanceOfPlayingNextFixture { get; set; }

        public string News { get; set; }

        public object IndexInfo { get; set; }
    }
}
