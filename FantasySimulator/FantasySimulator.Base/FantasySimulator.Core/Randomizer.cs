using System;

namespace FantasySimulator.Core
{
    public class Randomizer
    {
        public static readonly Randomizer Default = new Randomizer();

        public Randomizer()
            : this(new Random())
        {
            
        }

        public Randomizer(Random random)
        {
            Random = random;
        }

        public Random Random { get; private set; }


        public int Next(int maxValue)
        {
            return Random.Next(maxValue);
        }

        public int Next(int minValue, int maxValue)
        {
            return Random.Next(minValue, maxValue);
        }

    }
}
