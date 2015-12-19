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
    }
}