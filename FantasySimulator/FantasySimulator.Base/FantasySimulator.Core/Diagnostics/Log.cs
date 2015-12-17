using System;

namespace FantasySimulator.Core.Diagnostics
{
    public class Log
    {
        public static ILogFactory Factory { get; set; }

        static Log()
        {
            Factory = new Log4NetLogFactory();
        }


        public ILog GetLog(string name)
        {
            if (Factory == null)
                throw new InvalidOperationException("Invalid factory instance");
            var log = Factory.GetLog(name);
            return log;
        }

        public ILog GetLog(Type type)
        {
            if (Factory == null)
                throw new InvalidOperationException("Invalid factory instance");
            var log = Factory.GetLog(type);
            return log;
        }

    }
}
