using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FantasySimulator.Core;
using FantasySimulator.Simulator.Soccer.Structs;

namespace FantasySimulator.Simulator.Soccer.Analysers
{
    public class ChanceOfPlayingNextFixturePlayerAnalyser : PlayerAnalyserBase
    {
        public ChanceOfPlayingNextFixturePlayerAnalyser()
        {
            
        }


        public override string Name { get { return nameof(ChanceOfPlayingNextFixturePlayerAnalyser); } }
        
        public PointMapper PointMapper
        {
            get { return Properties["PointMapper"].SafeConvert<PointMapper>(); }
            set { Properties["PointMapper"] = value; }
        }


        public override IEnumerable<PlayerRecommendation> Analyse(Player player, Fixture fixture, SimulationContext context)
        {
            if (PointMapper == null)
                throw new ArgumentException("Invalid property", nameof(PointMapper));


            var res = new PlayerRecommendation();
            res.Type = PlayerRecommendationType.ChanceOfPlaying;


            // old:
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


            var valueMap = new ValueMap();
            valueMap["percentage"] = player.Fantasy.ChanceOfPlayingNextFixture;

            res.Points = PointMapper.Test(valueMap).Sum(x => x.Points);
            yield return res;
        }


        public override void Configure(XElement element)
        {
            base.Configure(element);
        }


        public override void ConfigureDefault()
        {
            base.ConfigureDefault();

            
            var mappings = new List<PointMapping>
            {
                new PointMapping
                {
                    Points = -10,
                    Predicates = new List<RangePredicate>
                    {
                        new RangePredicate
                        {
                            Value = 0d,
                            Operator = ComparisonOperator.LessOrEqualThan,
                            Unit = "percentage",
                        },
                    }.ToArray(),
                },
                new PointMapping
                {
                    Points = -3,
                    Predicates = new List<RangePredicate>
                    {
                        new RangePredicate
                        {
                            Value = 0.25d,
                            Operator = ComparisonOperator.LessOrEqualThan,
                            Unit = "percentage",
                        },
                    }.ToArray(),
                },
                new PointMapping
                {
                    Points = -2,
                    Predicates = new List<RangePredicate>
                    {
                        new RangePredicate
                        {
                            Value = 0.5d,
                            Operator = ComparisonOperator.LessOrEqualThan,
                            Unit = "percentage",
                        },
                    }.ToArray(),
                },
                new PointMapping
                {
                    Points = -1,
                    Predicates = new List<RangePredicate>
                    {
                        new RangePredicate
                        {
                            Value = 0.75d,
                            Operator = ComparisonOperator.LessOrEqualThan,
                            Unit = "percentage",
                        },
                    }.ToArray(),
                },
            }.ToArray();

            var range = new PointMapper();
            range.Mappings = mappings;
        }
        
    }
}