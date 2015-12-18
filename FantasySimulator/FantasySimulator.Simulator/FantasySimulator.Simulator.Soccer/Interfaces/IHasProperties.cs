using System.Collections.Generic;

namespace FantasySimulator.Simulator.Soccer
{
    public interface IHasProperties
    {
        IDictionary<string, object> Properties { get; }
    }
}
