using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FantasySimulator.Core;

namespace FantasySimulator.Simulator.Soccer.Analysers
{
    public class ChanceOfPlayerNextFixturePlayerAnalyser : PlayerAnalyserBase
    {
        public ChanceOfPlayerNextFixturePlayerAnalyser()
        {
            PointsRange = new List<PointRangeMapping>
            {
                new PointRangeMapping
                {
                    Points = -10,
                    Predicates = new List<RangePredicate>
                    {
                        new RangePredicate
                        {
                            Value = 0d,
                            Operator = ComparisonOperator.LessOrEqualThan
                        },
                    }.ToArray(),
                },
                new PointRangeMapping
                {
                    Points = -3,
                    Predicates = new List<RangePredicate>
                    {
                        new RangePredicate
                        {
                            Value = 0.25d,
                            Operator = ComparisonOperator.LessOrEqualThan
                        },
                    }.ToArray(),
                },
                new PointRangeMapping
                {
                    Points = -2,
                    Predicates = new List<RangePredicate>
                    {
                        new RangePredicate
                        {
                            Value = 0.5d,
                            Operator = ComparisonOperator.LessOrEqualThan
                        },
                    }.ToArray(),
                },
                new PointRangeMapping
                {
                    Points = -1,
                    Predicates = new List<RangePredicate>
                    {
                        new RangePredicate
                        {
                            Value = 0.75d,
                            Operator = ComparisonOperator.LessOrEqualThan
                        },
                    }.ToArray(),
                },
            }.ToArray();

        }


        public PointRangeMapping[] PointsRange { get; set; }


        public override void Configure(XElement element)
        {
            base.Configure(element);

            var mappingElems = element.Elements("mapping").ToList();
            if (mappingElems.Any())
            {
                PointsRange = new PointRangeMapping[0];
                foreach (var elem in mappingElems)
                {
                    var map = new PointRangeMapping();
                    foreach (var e in elem.Elements())
                    {
                        if (e.Name.LocalName == "points")
                            map.Points = (e.GetAttributeValue("value") ?? e.Value).SafeConvert<int>();
                        else
                        {
                            var op = e.Name.LocalName.SafeConvert<ComparisonOperator>();
                            if (op != ComparisonOperator.None)
                            {
                                var predicate = new RangePredicate();
                                predicate.Operator = op;
                                predicate.Value = e.GetAttributeValue("value").SafeConvert<double>();
                                map.Predicates = map.Predicates.Concat(new[] {predicate}).ToArray();
                            }
                        }
                    }
                    PointsRange = PointsRange.Concat(new[] {map}).ToArray();
                }
            }
        }


        public override IEnumerable<PlayerRecommendation> Analyse(Player player, Fixture fixture, SimulationContext context)
        {
            var res = new PlayerRecommendation();
            res.Type = RecommendationType.ChanceOfPlaying;

            //// Negatives
            //if (player.Fantasy.ChanceOfPlayingNextFixture >= 0)
            //{
            //    if (player.Fantasy.ChanceOfPlayingNextFixture <= 0)
            //        res.Points = -10;
            //    else if (player.Fantasy.ChanceOfPlayingNextFixture <= 0.25)
            //        res.Points = -3;
            //    else if (player.Fantasy.ChanceOfPlayingNextFixture <= 0.50)
            //        res.Points = -2;
            //    else if (player.Fantasy.ChanceOfPlayingNextFixture <= 0.75)
            //        res.Points = -1;
            //}

            res.Points = 0;
            foreach (var mapping in PointsRange)
            {
                if (mapping == null)
                    continue;
                var predicate = mapping.Predicates.ToList().Find(x => x.Test(player.Fantasy.ChanceOfPlayingNextFixture));
                if (predicate != null)
                {
                    res.Points = mapping.Points;
                    break;
                }
            }
            yield return res;
        }


        public class RangeMapping
        {
            public RangeMapping()
            {
                Predicates = new RangePredicate[0];
            }

            public RangePredicate[] Predicates { get; set; }
        }

        public class PointRangeMapping : RangeMapping
        {
            public int Points { get; set; }
        }

        public class RangePredicate
        {
            public ComparisonOperator Operator { get; set; }
            public double Value { get; set; }

            public bool Test(double value)
            {
                var c = value.CompareTo(Value);
                switch (Operator)
                {
                    case ComparisonOperator.LessThan:
                        return c < 0;
                    case ComparisonOperator.LessOrEqualThan:
                        return c <= 0;
                    case ComparisonOperator.Equals:
                        return c == 0;
                    case ComparisonOperator.GreaterOrEqualThan:
                        return c >= 0;
                    case ComparisonOperator.GreaterThan:
                        return c > 0;
                    default:
                        return false;
                }
            }
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