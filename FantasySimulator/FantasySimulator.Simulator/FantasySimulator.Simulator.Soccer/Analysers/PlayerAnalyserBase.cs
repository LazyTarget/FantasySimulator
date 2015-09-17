using System.Collections.Generic;

namespace FantasySimulator.Simulator.Soccer.Analysers
{
    public abstract class PlayerAnalyserBase
    {
        protected PlayerAnalyserBase()
        {
            Properties = new Dictionary<string, object>();
        }

        public IDictionary<string, object> Properties { get; private set; }

        public abstract IEnumerable<PlayerRecommendation> Analyse(Player player, Fixture fixture, SimulationContext context);
    }
}