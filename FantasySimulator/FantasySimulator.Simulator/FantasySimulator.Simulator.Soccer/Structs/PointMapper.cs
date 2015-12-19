using System;
using System.Xml.Linq;
using FantasySimulator.Interfaces;

namespace FantasySimulator.Simulator.Soccer.Structs
{
    public class PointMapper : Mapper<PointMapping>
    {
        protected override IMappingGroup<PointMapping> InstanciateMappingGroup(XElement elem)
        {
            var obj = elem.InstantiateElement();
            if (!(obj is IXmlConfigurable))
            {
                obj = new MappingGroup<PointMapping>(this).InstantiateConfigurable(elem);
            }
            var mapGroup = (IMappingGroup<PointMapping>) obj;
            return mapGroup;
        }

        protected internal override PointMapping InstanciateMapping(XElement elem)
        {
            var obj = elem.InstantiateElement();
            if (!(obj is IXmlConfigurable))
            {
                obj = new PointMapping().InstantiateConfigurable(elem);
            }
            var mapGroup = (PointMapping) obj;
            return mapGroup;
        }
    }
}