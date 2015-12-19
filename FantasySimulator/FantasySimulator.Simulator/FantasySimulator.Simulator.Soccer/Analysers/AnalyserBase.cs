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
    public abstract class AnalyserBase : IXmlConfigurable, IHasProperties
    {
        private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);

        protected AnalyserBase()
        {
            Properties = new DictionaryEx<string, object>();
            Enabled = true;
            ConfigureDefault();
        }

        public abstract string Name { get; }

        public bool Enabled
        {
            get { return Properties["Enabled"].SafeConvert<bool>(); }
            set { Properties["Enabled"] = value; }
        }

        public IDictionary<string, object> Properties { get; private set; }
        

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