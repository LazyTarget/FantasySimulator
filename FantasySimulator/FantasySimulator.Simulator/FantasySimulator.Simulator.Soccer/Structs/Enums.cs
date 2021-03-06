﻿namespace FantasySimulator.Simulator.Soccer.Structs
{
    public enum ComparisonOperator
    {
        None,
        LessThan,
        LessOrEqualThan,
        Equals,
        GreaterOrEqualThan,
        GreaterThan,
    }

    public enum PredicateMode
    {
        None,
        All,
        Any,
    }

    public enum MappingMode
    {
        None,
        FirstMatch,
        LastMatch,
        AllMatching,
    }
}