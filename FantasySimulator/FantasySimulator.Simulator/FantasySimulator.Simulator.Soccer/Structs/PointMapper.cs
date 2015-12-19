using System;
using System.Xml.Linq;
using FantasySimulator.Core;

namespace FantasySimulator.Simulator.Soccer.Structs
{
    public class PointMapper : Mapper<PointMapping>
    {
        protected override PointMapping InstanciateMapping(XElement elem)
        {
            var predicateType = elem.GetAttributeValue("type");
            if (!string.IsNullOrWhiteSpace(predicateType))
            {
                throw new Exception($"Type '{nameof(PointMapper)}' doesn't support specifying the mapping type, as: <mapping type=\"{nameof(PointMapping)}\">");
            }
            return new PointMapping();
        }
    }
}