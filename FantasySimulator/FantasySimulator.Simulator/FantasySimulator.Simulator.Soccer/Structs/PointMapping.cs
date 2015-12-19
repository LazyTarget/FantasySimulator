using System.Xml.Linq;
using FantasySimulator.Core;

namespace FantasySimulator.Simulator.Soccer.Structs
{
    public class PointMapping : RangeMapping
    {
        public double Points
        {
            get { return Properties["Points"].SafeConvert<double>(); }
            set { Properties["Points"] = value; }
        }


        public override void Configure(XElement element)
        {
            base.Configure(element);

            foreach (var e in element.Elements())
            {
                if (e.Name.LocalName == "points")
                    Points = (e.GetAttributeValue("value") ?? e.Value).SafeConvert<double>();
            }
        }
    }
}