using System;

namespace FantasySimulator.Interfaces
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoConfigurePropertiesAttribute : Attribute
    {
        public AutoConfigurePropertiesAttribute()
        {
            AutoConfigure = true;
        }

        public bool AutoConfigure { get; set; }

    }
}
