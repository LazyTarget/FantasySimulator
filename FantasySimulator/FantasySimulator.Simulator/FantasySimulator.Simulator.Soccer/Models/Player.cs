using System;
using System.Linq;

namespace FantasySimulator.Simulator.Soccer
{
    public class Player
    {
        public string ID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get { return string.Join(" ", new[] { FirstName, LastName }.Where(x => !string.IsNullOrWhiteSpace(x))); } }

        public string DisplayName { get; set; }

        public int Age { get; set; }

        public Rating Rating { get; set; }

        public double OriginalPrice { get; set; }

        public double CurrentPrice { get; set; }

        public double PriceChange { get { return CurrentPrice - OriginalPrice; } }

        public PlayerPosition Position { get; set; }

        public bool Unavailable { get; set; }

        public double ChanceOfPlayingNextFixture { get; set; }

        public string News { get; set; }

        public Team Team { get; set; }

        public PlayerStatistics Statistics { get; set; }


        public override string ToString()
        {
            return DisplayName;
        }
    }
}
