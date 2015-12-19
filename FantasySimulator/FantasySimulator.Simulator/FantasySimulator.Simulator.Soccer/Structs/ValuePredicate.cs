using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using FantasySimulator.Core;
using FantasySimulator.Core.Diagnostics;
using FantasySimulator.Interfaces;

namespace FantasySimulator.Simulator.Soccer.Structs
{
    public abstract class ValuePredicate : IXmlConfigurable, IHasProperties
    {
        private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);

        protected ValuePredicate()
        {
            Properties = new Dictionary<string, object>();
        } 


        public IDictionary<string, object> Properties { get; private set; }
        
        public ComparisonOperator Operator
        {
            get { return Properties["Operator"].SafeConvert<ComparisonOperator>(); }
            set { Properties["Operator"] = value; }
        }

        public virtual object Value
        {
            get { return Properties["Value"].SafeConvert<object>(); }
            set { Properties["Value"] = value; }
        }

        public string Unit
        {
            get { return Properties["Unit"].SafeConvert<string>(); }
            set { Properties["Unit"] = value; }
        }


        public virtual void Configure(XElement element)
        {
            foreach (var e in element.Elements())
            {
                if (string.Equals(e.Name.LocalName, "operator", StringComparison.OrdinalIgnoreCase))
                    Operator = (e.GetAttributeValue("value") ?? e.Value).SafeConvert<ComparisonOperator>();
                else if (string.Equals(e.Name.LocalName, "value", StringComparison.OrdinalIgnoreCase))
                    Value = (e.GetAttributeValue("value") ?? e.Value).SafeConvert<object>();
                else if (string.Equals(e.Name.LocalName, "unit", StringComparison.OrdinalIgnoreCase))
                    Unit = (e.GetAttributeValue("value") ?? e.Value).SafeConvert<string>();
                else if (string.Equals(e.Name.LocalName, "property", StringComparison.OrdinalIgnoreCase))
                {
                    var propertyName = e.GetAttributeValue("name");
                    if (string.IsNullOrWhiteSpace(propertyName))
                        continue;
                    try
                    {
                        object value = e.InstantiateElement();
                        Properties[propertyName] = value;
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"Error instantiating property '{propertyName}'", ex);
                    }
                }
                else
                {
                    
                }
            }
        }

        public abstract bool Test(object value);
    }


    public abstract class ValuePredicate<TValue> : ValuePredicate
    {
        public new TValue Value
        {
            get { return base.Value.SafeConvert<TValue>(); }
            set { base.Value = value; }
        }

        public abstract bool Test(TValue value);
    }
}