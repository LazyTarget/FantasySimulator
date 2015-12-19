using System;
using System.Linq;
using System.Xml.Linq;
using FantasySimulator.Core;

namespace FantasySimulator.Simulator.Soccer.Structs
{
    public class RangePredicate : ValuePredicate<double>
    {
        public override void Configure(XElement element)
        {
            base.Configure(element);

            foreach (var e in element.Elements())
            {
                if (string.Equals(e.Name.LocalName, "value", StringComparison.OrdinalIgnoreCase) ||
                    (string.Equals(e.Name.LocalName, "property", StringComparison.OrdinalIgnoreCase) &&
                     string.Equals(e.GetAttributeValue("name") ?? "", "value", StringComparison.OrdinalIgnoreCase)))
                {
                    var parser = new SimpleMathParser();
                    var expression = (e.GetAttributeValue("value") ?? e.Value);
                    Value = parser.Parse(expression);
                }
            }
        }

        public override bool Test(object value)
        {
            var val = (double) value;
            //var val = value.SafeConvert<double>();
            var res = Test(val);
            return res;
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