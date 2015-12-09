using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using FantasySimulator.Core;
using FantasySimulator.Interfaces;

namespace FantasySimulator.Simulator.Soccer.Analysers
{
    public abstract class PlayerAnalyserBase
    {
        protected PlayerAnalyserBase()
        {
            Enabled = true;
            Properties = new Dictionary<string, object>();
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
                try
                {
                    var propertyName = elem.GetAttributeValue("name");
                    if (string.IsNullOrWhiteSpace(propertyName))
                        continue;
                    object value = elem.InstantiateElement();
                    Properties[propertyName] = value;
                }
                catch (Exception ex)
                {
                    
                }
            }
        }

        public virtual void ConfigureDefault()
        {

        }

    }
}