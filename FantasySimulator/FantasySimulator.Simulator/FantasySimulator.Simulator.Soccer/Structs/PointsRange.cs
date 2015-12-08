using System.Linq;

namespace FantasySimulator.Simulator.Soccer.Structs
{
    public class PointsRange : Range<PointRangeMapping>
    {
        protected override PointRangeMapping InstanciateMapping()
        {
            return new PointRangeMapping();
        }
    }
}