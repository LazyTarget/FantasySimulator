using System;
using System.Xml.Linq;
using FantasySimulator.Interfaces;

namespace FantasySimulator.Simulator.Soccer.Structs
{
    public class PointRangeMapper : Mapper<PointRangeMapping>
    {
        protected override IMappingGroup<PointRangeMapping> InstanciateMappingGroup(XElement elem)
        {
            var obj = elem.InstantiateElement();
            if (!(obj is IXmlConfigurable))
            {
                obj = new MappingGroup<PointRangeMapping>(this).InstantiateConfigurable(elem);
            }
            var mapGroup = (IMappingGroup<PointRangeMapping>)obj;
            return mapGroup;
        }

        protected internal override PointRangeMapping InstanciateMapping(XElement elem)
        {
            var obj = elem.InstantiateElement();
            if (!(obj is IXmlConfigurable))
            {
                obj = new PointRangeMapping().InstantiateConfigurable(elem);
            }
            var map = (PointRangeMapping) obj;
            return map;
        }
    }
}