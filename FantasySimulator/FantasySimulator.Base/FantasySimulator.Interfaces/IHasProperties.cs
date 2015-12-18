using System.Collections.Generic;

namespace FantasySimulator.Interfaces
{
    public interface IHasProperties
    {
        IDictionary<string, object> Properties { get; }
    }
}
