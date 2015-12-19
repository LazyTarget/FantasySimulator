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

        public PointMapper PointMapper
        {
            get { return Properties["PointRange"].SafeConvert<PointMapper>(); }
            set { Properties["PointRange"] = value; }
        }

        public Mapper<Mapping<RangePredicate>> Mapper
        {
            get { return Properties["Mapper"].SafeConvert<Mapper<Mapping<RangePredicate>>>(); }
            set { Properties["Mapper"] = value; }
        }


        public override IEnumerable<PlayerRecommendation> Analyse(Player player, Fixture fixture, SimulationContext context)
        {
            if (PointMapper == null)
                throw new ArgumentException("Invalid property", nameof(PointMapper));

            var res = new PlayerRecommendation();
            res.Type = PlayerRecommendationType.PlayerPlaytime;

            var playerTeam = player.GetLeagueTeam(fixture);
            double teamPlayedMinutes = playerTeam.GetPlayedMinutesBeforeFixtureForTeam(fixture);
            double playerMinutes = player.GetPlayedMinutesBeforeFixtureForPlayer(fixture);
            var percentage = teamPlayedMinutes > 0
                ? playerMinutes / teamPlayedMinutes
                : 0;

            var valueMap = new ValueMap();
            valueMap["minutes"] = playerMinutes;
            valueMap["percentage"] = percentage;
            //valueMap["playedgames"] = ;
            //valueMap["substitutes-in"] = ;
            //valueMap["substitutes-out"] = ;
            // todo: add more data points (subs, recent playtime (5 last team games), recent subs)

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
        }
        
    }
}
