using System.Collections.Generic;
using System.Linq;
using FantasySimulator.Simulator.Soccer.Analysers;

namespace FantasySimulator.Simulator.Soccer
{
    public class SoccerSimulationPlayerResult
    {
        public SoccerSimulationPlayerResult()
        {
            PlayerRecommendations = new List<PlayerRecommendation>();
            TeamRecommendations = new List<TeamRecommendation>();
        }


        //public SoccerSimulationPlayerResult(Player player, IDictionary<PlayerRecommendationType, int> playerRecommendations)
        //    : this()
        //{
        //    Player = player;
        //    PlayerRecommendations = playerRecommendations;
        //}


        public Player Player { get; set; }

        public IList<PlayerRecommendation> PlayerRecommendations { get; private set; }

        public IList<TeamRecommendation> TeamRecommendations { get; private set; }


        public int EstimatedPoints { get; set; }

        public double RecommendationPoints
        {
            get
            {
                var player = PlayerRecommendations?.Sum(x => x.Points) ?? 0;
                var team = TeamRecommendations?.Sum(x => x.Points) ?? 0;
                var sum = player + team;
                return sum;
            }
        }


        public void AddRecommendation(PlayerRecommendation recommendation)
        {
            //if (!PlayerRecommendations.ContainsKey(type))
            //    PlayerRecommendations.Add(type, 0);
            //PlayerRecommendations[type] += value;

            PlayerRecommendations.Add(recommendation);
        }

        public void AddRecommendation(TeamRecommendation recommendation)
        {
            TeamRecommendations.Add(recommendation);
        }


        public override string ToString()
        {
            return string.Format("{0}, rec.pts: {1}", Player?.DisplayName ?? "", RecommendationPoints);
        }
    }
}