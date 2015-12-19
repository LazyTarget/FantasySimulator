using System.Xml.Linq;

namespace FantasySimulator.Simulator.Soccer.Structs
{
    public class RangeMapping : Mapping<RangePredicate>
    {
        protected override RangePredicate InstanciatePredicate(XElement elem)
        {
            return new RangePredicate();
        }
    }
}
