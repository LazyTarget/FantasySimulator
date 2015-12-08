using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FantasySimulator.Core;

namespace FantasySimulator.Simulator.Soccer.Analysers
{
    public abstract class PlayerAnalyserBase
    {
        protected PlayerAnalyserBase()
        {
            Enabled = true;
            Properties = new Dictionary<string, object>();
        }

        public abstract string Name { get; }

        public bool Enabled { get; private set; }

        public IDictionary<string, object> Properties { get; private set; }


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
                    var propertyType = elem.GetAttributeValue("type");
                    var str = elem.GetAttributeValue("value") ?? elem.Value;
                    object value;
                    if (!string.IsNullOrWhiteSpace(propertyType))
                    {
                        var type = Type.GetType(propertyType);
                        value = str.SafeConvertDynamic(type);
                    }
                    else
                        value = str;
                    Properties[propertyName] = value;
                }
                catch (Exception ex)
                {
                    
                }
            }
        }

        public abstract IEnumerable<PlayerRecommendation> Analyse(Player player, Fixture fixture, SimulationContext context);
    }
}