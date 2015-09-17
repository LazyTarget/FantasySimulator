using System.Collections.Generic;
using FantasySimulator.Core;

namespace FantasySimulator.Simulator.Soccer.Analysers
{
    public class HomeTeamAdvantagePlayerAnalyser : PlayerAnalyserBase
    {
        public HomeTeamAdvantagePlayerAnalyser()
        {
            Points = 1;
        }

        public int Points
        {
            get { return Properties["Points"].SafeConvert<int>(); }
            set { Properties["Points"] = value; }
        }

        public override IEnumerable<PlayerRecommendation> Analyse(Player player, Fixture fixture, SimulationContext context)
        {
            var res = new PlayerRecommendation();
            res.Type = RecommendationType.HomeTeamAdvantage;

            var homeTeamAdvantage = player.HasHomeTeamAdvantage(fixture);
            if (homeTeamAdvantage)
                res.Points = Points;
            yield return res;
        }
    }
}