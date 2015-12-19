using System;
using System.Xml.Linq;
using FantasySimulator.Core;

namespace FantasySimulator.Simulator.Soccer.Structs
{
    public class PointRangeMapper : Mapper<PointRangeMapping>
    {
        protected override PointRangeMapping InstanciateMapping(XElement elem)
        {
            var predicateType = elem.GetAttributeValue("type");
            if (!string.IsNullOrWhiteSpace(predicateType))
            {
                throw new Exception($"Type '{nameof(PointRangeMapping)}' doesn't support specifying the mapping type, as: <mapping type=\"{nameof(PointRangeMapping)}\">");
            }
            return new PointRangeMapping();
        }
    }
}