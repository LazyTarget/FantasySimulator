using System.Collections.Generic;
using FantasySimulator.Interfaces;
using FantasySimulator.Simulator.Soccer.Structs;

namespace FantasySimulator.Simulator.Soccer
{
    public interface IHasMappingGroups<TMapping> : IXmlConfigurable, IHasProperties
        where TMapping : Mapping
    {
        IList<IMappingGroup<TMapping>> MappingGroups { get; }

        IEnumerable<TMapping> Test(ValueMap values);
    }
}
