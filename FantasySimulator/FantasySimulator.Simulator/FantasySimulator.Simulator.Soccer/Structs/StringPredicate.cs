using System;
using System.Linq;
using System.Xml.Linq;
using FantasySimulator.Core;

namespace FantasySimulator.Simulator.Soccer.Structs
{
    public class StringPredicate : ValuePredicate<string>
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
                        
                    }
                }
            }
        }


        public override bool Test(object value)
        {
            var val = value.SafeConvert<string>();
            var res = Test(val);
            return res;
        }
        
        public override bool Test(string value)
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