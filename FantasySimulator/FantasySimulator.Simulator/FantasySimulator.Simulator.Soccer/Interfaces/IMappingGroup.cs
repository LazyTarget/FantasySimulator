using System.Collections.Generic;
using FantasySimulator.Interfaces;
using FantasySimulator.Simulator.Soccer.Structs;

namespace FantasySimulator.Simulator.Soccer
{
    public interface IMappingGroup<TMapping> : IXmlConfigurable, IHasProperties
        where TMapping : Mapping
    {
        MappingMode MappingMode { get; set; }

        IList<TMapping> Mappings { get; }

        IEnumerable<TMapping> Test(ValueMap values);
    }
}
