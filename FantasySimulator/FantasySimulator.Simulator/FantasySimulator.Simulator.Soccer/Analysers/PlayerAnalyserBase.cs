using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using FantasySimulator.Core;
using FantasySimulator.Core.Classes;
using FantasySimulator.Core.Diagnostics;
using FantasySimulator.Interfaces;

namespace FantasySimulator.Simulator.Soccer.Analysers
{
    public abstract class PlayerAnalyserBase : AnalyserBase, IXmlConfigurable, IHasProperties
    {
        private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);

        protected PlayerAnalyserBase()
        {

        }


        // todo: implement Player analysers, which estimate what players that have the edge...
        public abstract IEnumerable<PlayerRecommendation> Analyse(Player player, Fixture fixture, SimulationContext context);


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