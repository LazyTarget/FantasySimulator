namespace FantasySimulator.Simulator.Soccer.Models
{
    public class Player
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public Rating Rating { get; set; }

        public double Price { get; set; }

        public PlayerPosition Position { get; set; }

        public Team Team { get; set; }

        public PlayerStatistics Statistics { get; set; }


        public override string ToString()
        {
            return Name;
        }
    }
}
