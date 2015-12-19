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

            var propertyElems = element.Elements("property").Where(x => x != null).ToList();
            if (propertyElems.Any())
            {
                foreach (var elem in propertyElems)
                {
                    var propertyName = elem.GetAttributeValue("name");
                    if (string.IsNullOrWhiteSpace(propertyName))
                        continue;
                    if (propertyName == "value")
                    {
                        // Custom implementation...
                        var parser = new SimpleMathParser();
                        var expression = elem.GetAttributeValue("value");
                        Value = parser.Parse(expression);
                    }
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