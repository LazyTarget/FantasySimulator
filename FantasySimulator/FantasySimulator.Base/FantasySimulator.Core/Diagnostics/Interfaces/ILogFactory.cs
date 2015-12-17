using System;

namespace FantasySimulator.Core.Diagnostics
{
    public interface ILogFactory
    {
        ILog GetLog(string name);
        ILog GetLog(Type type);
    }
}