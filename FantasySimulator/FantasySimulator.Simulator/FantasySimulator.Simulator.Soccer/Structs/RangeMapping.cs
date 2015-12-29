using System;
using System.Linq;
using System.Xml.Linq;
using FantasySimulator.Core;
using FantasySimulator.Interfaces;

namespace FantasySimulator.Simulator.Soccer.Structs
{
    public class RangeMapping : Mapping<RangePredicate>
    {
        protected override RangePredicate InstanciatePredicate(XElement elem)
        {
            var obj = elem.InstantiateElement();
            if (!(obj is IXmlConfigurable))
            {
                obj = new RangePredicate().InstantiateConfigurable(elem);
            }
            var pred = (RangePredicate) obj;
            return pred;
        }
    }
}
