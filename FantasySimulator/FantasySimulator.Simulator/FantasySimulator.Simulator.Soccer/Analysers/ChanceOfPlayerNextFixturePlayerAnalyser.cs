using System.Collections.Generic;

namespace FantasySimulator.Simulator.Soccer.Analysers
{
    public class ChanceOfPlayerNextFixturePlayerAnalyser : PlayerAnalyserBase
    {
        public ChanceOfPlayerNextFixturePlayerAnalyser()
        {
            
        }

        public PointsRange PointsRange
        {
            get { return Properties["Points"].SafeConvert<int>(); }
            set { Properties["Points"] = value; }
        }

        public override IEnumerable<PlayerRecommendation> Analyse(Player player, Fixture fixture, SimulationContext context)
        {
            var res = new PlayerRecommendation();
            res.Type = RecommendationType.LoweredChanceOfPlaying;

            // Negatives
            if (player.Fantasy.ChanceOfPlayingNextFixture >= 0)
            {
                if (player.Fantasy.ChanceOfPlayingNextFixture <= 0)
                    res.Points = -10;
                else if (player.Fantasy.ChanceOfPlayingNextFixture <= 0.25)
                    res.Points = -3;
                else if (player.Fantasy.ChanceOfPlayingNextFixture <= 0.50)
                    res.Points = -2;
                else if (player.Fantasy.ChanceOfPlayingNextFixture <= 0.75)
                    res.Points = -1;
            }
            yield return res;
        }

        public class RangeMapping
        {
            public RangePredicate[] Predicates { get; set; }
        }

        public class PointsRange : RangeMapping
        {
            public int Points { get; set; }
        }

        public class RangePredicate
        {
            public ComparisonOperator Operator { get; set; }
            public double Value { get; set; }
        }

        public enum ComparisonOperator
        {
            None,
            LessThan,
            LessOrEqualThan,
            Equals,
            GreaterOrEqualThan,
            GreaterThan,
        }
    }
}