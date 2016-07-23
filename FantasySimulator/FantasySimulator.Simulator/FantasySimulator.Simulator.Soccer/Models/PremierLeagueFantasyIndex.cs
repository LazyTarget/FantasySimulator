using Lux;

namespace FantasySimulator.Simulator.Soccer
{
    public class PremierLeagueFantasyIndex
    {
        public Assignable<double> EaIndex { get; set; }

        public Assignable<double> Influence { get; set; }

        public Assignable<double> Creativity { get; set; }

        public Assignable<double> Threat { get; set; }
    }
}
