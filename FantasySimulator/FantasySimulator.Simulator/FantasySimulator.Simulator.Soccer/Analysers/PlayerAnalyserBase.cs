using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using FantasySimulator.Core;
using FantasySimulator.Core.Classes;
using FantasySimulator.Core.Diagnostics;

namespace FantasySimulator.Simulator.Soccer.Analysers
{
    public abstract class PlayerAnalyserBase : IXmlConfigurable, IHasProperties
    {
        private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);

        protected PlayerAnalyserBase()
        {
            Enabled = true;
            Properties = new DictionaryEx<string, object>();
            ConfigureDefault();
        }

        public abstract string Name { get; }

        public bool Enabled { get; private set; }

        public IDictionary<string, object> Properties { get; private set; }

        public abstract IEnumerable<PlayerRecommendation> Analyse(Player player, Fixture fixture, SimulationContext context);


        public virtual void Configure(XElement element)
        {
            if (element.Attribute("enabled") != null)
                Enabled = element.GetAttributeValue("enabled").SafeConvert<bool>();

            foreach (var elem in element.Elements("property").Where(x => x != null))
            {
                var propertyName = elem.GetAttributeValue("name");
                if (string.IsNullOrWhiteSpace(propertyName))
                    continue;
                try
                {
                    object value = elem.InstantiateElement();
                    Properties[propertyName] = value;
                }
                catch (Exception ex)
                {
                    _log.Error($"Error instantiating property '{propertyName}'", ex);
                }
            }
        }

        public virtual void ConfigureDefault()
        {

        }

    }
}