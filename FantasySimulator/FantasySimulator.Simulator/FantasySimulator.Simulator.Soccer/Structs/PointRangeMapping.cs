using System.Xml.Linq;
using FantasySimulator.Core;

namespace FantasySimulator.Simulator.Soccer.Structs
{
    public class PointRangeMapping : RangeMapping
    {
        public int Points
        {
            get { return Properties["Points"].SafeConvert<int>(); }
            set { Properties["Points"] = value; }
        }


        public override void Configure(XElement element)
        {
            base.Configure(element);

            foreach (var e in element.Elements())
            {
                if (e.Name.LocalName == "points")
                    Points = (e.GetAttributeValue("value") ?? e.Value).SafeConvert<int>();
            }
        }
    }
}