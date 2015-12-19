using System.Collections.Generic;
using FantasySimulator.Interfaces;
using FantasySimulator.Simulator.Soccer.Structs;

namespace FantasySimulator.Simulator.Soccer
{
    public interface IPredicateGroup<TPredicate> : IXmlConfigurable, IHasProperties
        where TPredicate : ValuePredicate
    {
        PredicateMode PredicateMode { get; set; }

        IList<TPredicate> Predicates { get; }

        bool Test(ValueMap values);
    }
}