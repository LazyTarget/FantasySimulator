using System;
using FantasySimulator.Core;
using FantasySimulator.Simulator.Soccer.Structs;
using System.Collections.Generic;
using System.Xml.Linq;

namespace FantasySimulator.Simulator.Soccer.Analysers
{
    public class PlaytimePlayerAnalyser : PlayerAnalyserBase
    {
        public PlaytimePlayerAnalyser()
        {
            
        }

        public override string Name { get { return nameof(PlaytimePlayerAnalyser); } }

        public PointsRange PointsRange
        {
            get { return Properties["PointsRange"].SafeConvert<PointsRange>(); }
            set { Properties["PointsRange"] = value; }
        }


        public override IEnumerable<PlayerRecommendation> Analyse(Player player, Fixture fixture, SimulationContext context)
        {
            if (PointsRange == null)
                throw new ArgumentException("Invalid property", nameof(PointsRange));

            var res = new PlayerRecommendation();
            res.Type = RecommendationType.PlayerPlaytime;

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

            res.Points = 0;
            foreach (var mapping in PointsRange.Mappings)
            {
                if (mapping == null)
                    continue;

                var valid = mapping.Test(valueMap);
                if (valid)
                {
                    res.Points = mapping.Points;
                    break;
                }
            }
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
