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
            var propertyElems = element.Elements("property").Where(x => x != null).ToList();
            if (propertyElems.Any())
            {
                foreach (var elem in propertyElems)
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