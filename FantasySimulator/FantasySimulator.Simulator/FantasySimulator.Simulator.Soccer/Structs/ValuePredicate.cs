using System;
using System.Linq;
using System.Xml.Linq;
using FantasySimulator.Core;
using FantasySimulator.Interfaces;

namespace FantasySimulator.Simulator.Soccer.Structs
{
    public abstract class ValuePredicate<TValue> : IXmlConfigurable
    {
        public ComparisonOperator Operator { get; set; }
        public TValue Value { get; set; }
        public string Unit { get; set; }


        public virtual void Configure(XElement element)
        {
            foreach (var e in element.Elements())
            {
                if (string.Equals(e.Name.LocalName, "operator", StringComparison.OrdinalIgnoreCase))
                    Operator = (e.GetAttributeValue("value") ?? e.Value).SafeConvert<ComparisonOperator>();
                else if (string.Equals(e.Name.LocalName, "value", StringComparison.OrdinalIgnoreCase))
                    Value = (e.GetAttributeValue("value") ?? e.Value).SafeConvert<TValue>();
                else if (string.Equals(e.Name.LocalName, "unit", StringComparison.OrdinalIgnoreCase))
                    Unit = (e.GetAttributeValue("value") ?? e.Value).SafeConvert<string>();
            }
        }

        public abstract bool Test(TValue value);
    }
}