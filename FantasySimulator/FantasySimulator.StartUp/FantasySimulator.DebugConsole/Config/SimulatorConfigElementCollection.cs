using System.Configuration;

namespace FantasySimulator.DebugConsole.Config
{
    public class SimulatorConfigElementCollection : ConfigurationElementCollection
    {
        public SimulatorConfigElement this[int index]
        {
            get { return (SimulatorConfigElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(SimulatorConfigElement serviceConfig)
        {
            BaseAdd(serviceConfig);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SimulatorConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SimulatorConfigElement)element).Type;
        }

        public void Remove(SimulatorConfigElement serviceConfig)
        {
            BaseRemove(serviceConfig.Type);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }
    }
}
