using System;

namespace FantasySimulator.Core.Diagnostics
{
    public static class Log
    {
        public static ILogFactory Factory { get; set; }

        static Log()
        {
            Factory = new Log4NetLogFactory();
        }


        public static ILog GetLog(string name)
        {
            if (Factory == null)
                throw new InvalidOperationException("Invalid factory instance");
            var log = Factory.GetLog(name);
            return log;
        }

        public static ILog GetLog(Type type)
        {
            if (Factory == null)
                throw new InvalidOperationException("Invalid factory instance");
            var log = Factory.GetLog(type);
            return log;
        }

    }
}
