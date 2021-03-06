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
        
        public PointMapper Mapper
        {
            get { return Properties["Mapper"].SafeConvert<PointMapper>(); }
            set { Properties["Mapper"] = value; }
        }


        public override IEnumerable<PlayerRecommendation> Analyse(Player player, Fixture fixture, SimulationContext context)
        {
            if (Mapper == null)
                throw new ArgumentException("Invalid property", nameof(Mapper));


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

            res.Points = Mapper.Test(valueMap).Sum(x => x.Points);
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