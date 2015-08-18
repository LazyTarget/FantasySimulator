using System.Collections.Generic;
using System.Linq;

namespace FantasySimulator.Simulator.Soccer
{
    public class SoccerSimulationPlayerResult
    {
        public SoccerSimulationPlayerResult()
        {
            Recommendations = new Dictionary<RecommendationType, int>();
        }

        public SoccerSimulationPlayerResult(Player player, IDictionary<RecommendationType, int> recommendations)
            : this()
        {
            Player = player;
            Recommendations = recommendations;
        }


        public Player Player { get; set; }

        public IDictionary<RecommendationType, int> Recommendations { get; set; } 

        public int EstimatedPoints { get; set; }

        public int RecommendationPoints
        {
            get { return Recommendations?.Where(x => x.Key != RecommendationType.None).Sum(x => x.Value) ?? 0; }
        }


        public void AddRecommendation(RecommendationType type, int value)
        {
            if (!Recommendations.ContainsKey(type))
                Recommendations.Add(type, 0);
            Recommendations[type] += value;
        }


        public override string ToString()
        {
            return string.Format("{0}, rec.pts: {1}", Player?.DisplayName ?? "", RecommendationPoints);
        }
    }
}