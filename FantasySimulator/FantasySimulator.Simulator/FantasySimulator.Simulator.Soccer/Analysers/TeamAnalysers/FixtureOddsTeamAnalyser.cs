﻿using System;
using FantasySimulator.Core;
using FantasySimulator.Simulator.Soccer.Structs;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using FantasySimulator.Core.Diagnostics;

namespace FantasySimulator.Simulator.Soccer.Analysers
{
    public class FixtureOddsTeamAnalyser : TeamAnalyserBase
    {
        private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);

        public FixtureOddsTeamAnalyser()
        {
            
        }

        public override string Name { get { return nameof(FixtureOddsTeamAnalyser); } }
        

        public PointMapper PointMapper
        {
            get { return Properties["Mapper"].SafeConvert<PointMapper>(); }
            set { Properties["Mapper"] = value; }
        }


        public override IEnumerable<TeamRecommendation> Analyse(Player player, Fixture fixture, SimulationContext context)
        {
            if (PointMapper == null)
                throw new ArgumentException("Invalid property", nameof(PointMapper));

            var res = new TeamRecommendation();
            res.Type = TeamRecommendationType.FixtureOdds;

            var playerTeam = player.GetLeagueTeam(fixture);
            
            var valueMap = new ValueMap();
            if (fixture.Odds == null)
            {
                _log.Info($"No odds for fixture {fixture.HomeTeam.Team.ShortName} vs {fixture.AwayTeam.Team.ShortName} (GW #{fixture.Gameweek.Number}, cannot continue analyse");
                yield break;
            }
            else if (fixture.HomeTeam.Team.ID == playerTeam.Team.ID)
            {
                valueMap["odds-win"] = fixture.Odds.HomeWin.Value;
                valueMap["odds-draw"] = fixture.Odds.Draw.Value;
                valueMap["odds-loss"] = fixture.Odds.AwayWin.Value;
            }
            else if (fixture.AwayTeam.Team.ID == player.Team.ID)
            {
                valueMap["odds-win"] = fixture.Odds.AwayWin.Value;
                valueMap["odds-draw"] = fixture.Odds.Draw.Value;
                valueMap["odds-loss"] = fixture.Odds.HomeWin.Value;
            }
            else
            {
                _log.Warn($"Player '{player.DisplayName}' doesn't play for any of the fixture teams: {fixture.HomeTeam.Team.ShortName} vs {fixture.AwayTeam.Team.ShortName} (GW #{fixture.Gameweek.Number}");
                yield break;
            }

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
