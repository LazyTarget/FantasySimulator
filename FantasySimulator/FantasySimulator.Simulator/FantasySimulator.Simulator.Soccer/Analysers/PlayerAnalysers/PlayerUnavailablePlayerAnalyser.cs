using System.Collections.Generic;
using System.Xml.Linq;
using FantasySimulator.Core;

namespace FantasySimulator.Simulator.Soccer.Analysers
{
    public class PlayerUnavailablePlayerAnalyser : PlayerAnalyserBase
    {
        public PlayerUnavailablePlayerAnalyser()
        {
            Points = -10;
        }

        public override string Name { get { return nameof(PlayerUnavailablePlayerAnalyser); } }

        public int Points
        {
            get { return Properties["Points"].SafeConvert<int>(); }
            set { Properties["Points"] = value; }
        }

        public override IEnumerable<PlayerRecommendation> Analyse(Player player, Fixture fixture, SimulationContext context)
        {
            var res = new PlayerRecommendation();
            res.Type = PlayerRecommendationType.PlayerUnavailable;

            var unavailable = player.Fantasy.Unavailable;
            if (unavailable)
                res.Points = Points;
            yield return res;
        }

        public override void Configure(XElement element)
        {
            base.Configure(element);
        }

        public override void ConfigureDefault()
        {
            base.ConfigureDefault();
        }
    }
}