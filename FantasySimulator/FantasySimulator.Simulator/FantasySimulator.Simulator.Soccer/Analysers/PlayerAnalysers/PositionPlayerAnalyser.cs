using System;
using FantasySimulator.Core;
using FantasySimulator.Simulator.Soccer.Structs;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FantasySimulator.Simulator.Soccer.Analysers
{
    public class PositionPlayerAnalyser : PlayerAnalyserBase
    {
        public PositionPlayerAnalyser()
        {
            
        }

        public override string Name { get { return nameof(PositionPlayerAnalyser); } }
        
        public PointRangeMapper Mapper
        {
            get { return Properties["Mapper"].SafeConvert<PointRangeMapper>(); }
            set { Properties["Mapper"] = value; }
        }


        public override IEnumerable<PlayerRecommendation> Analyse(Player player, Fixture fixture, SimulationContext context)
        {
            if (Mapper == null)
                throw new ArgumentException("Invalid property", nameof(Mapper));

            var res = new PlayerRecommendation();
            res.Type = PlayerRecommendationType.PlayerPosition;


            var valueMap = context.GeneratePlayerValueMap(player, fixture, this);

            //res.Points = Mapper.Test(valueMap).Sum(x => x.Points);

            var pointRanges = Mapper.Test(valueMap).ToList();
            var pointMappers = pointRanges.Select(x => x.Mapper).ToList();
            var pointMappings = pointMappers.SelectMany(x => x.Test(valueMap)).ToList();
            res.Points = pointMappings.Sum(x => x.Points);
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
