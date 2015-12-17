using System;
using System.Linq;
using System.Xml.Linq;
using FantasySimulator.Core;

namespace FantasySimulator.Simulator.Soccer.Structs
{
    public class RangePredicate : ValuePredicate<double>
    {
        //public ComparisonOperator Operator { get; set; }
        //public double Value { get; set; }
        //public string Unit { get; set; }

        
        public override void Configure(XElement element)
        {
            base.Configure(element);

            foreach (var e in element.Elements())
            {
                if (string.Equals(e.Name.LocalName, "operator", StringComparison.OrdinalIgnoreCase))
                    Operator = (e.GetAttributeValue("value") ?? e.Value).SafeConvert<ComparisonOperator>();
                else if (string.Equals(e.Name.LocalName, "value", StringComparison.OrdinalIgnoreCase))
                {
                    var parser = new SimpleMathParser();
                    var expression = (e.GetAttributeValue("value") ?? e.Value);
                    Value = parser.Parse(expression);
                }
                //else if (string.Equals(e.Name.LocalName, "unit", StringComparison.OrdinalIgnoreCase))
                //    Unit = (e.GetAttributeValue("value") ?? e.Value).SafeConvert<string>();
            }
        }


        public override bool Test(double value)
        {
            var c = value.CompareTo(Value);
            switch (Operator)
            {
                case ComparisonOperator.LessThan:
                    return c < 0;
                case ComparisonOperator.LessOrEqualThan:
                    return c <= 0;
                case ComparisonOperator.Equals:
                    return c == 0;
                case ComparisonOperator.GreaterOrEqualThan:
                    return c >= 0;
                case ComparisonOperator.GreaterThan:
                    return c > 0;
                default:
                    return false;
            }
        }
    }
}