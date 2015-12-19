namespace FantasySimulator.Simulator.Soccer.Structs
{
    public class RangeMapping : Mapping<RangePredicate>
    {
        protected override RangePredicate InstanciatePredicate()
        {
            return new RangePredicate();
        }
    }
}
