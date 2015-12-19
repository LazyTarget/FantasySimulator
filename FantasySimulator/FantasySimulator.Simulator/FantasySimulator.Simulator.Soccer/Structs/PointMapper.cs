namespace FantasySimulator.Simulator.Soccer.Structs
{
    public class PointMapper : Mapper<PointMapping>
    {
        protected override PointMapping InstanciateMapping()
        {
            return new PointMapping();
        }
    }
}