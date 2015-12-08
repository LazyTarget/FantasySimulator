using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FantasySimulator.Simulator.Soccer.Structs
{
    public class Range<TMapping> : IXmlConfigurable
        where TMapping : RangeMapping, new()
    {
        public Range()
        {
            Properties = new Dictionary<string, object>();
            Mappings = new TMapping[0];
        }

        public TMapping[] Mappings { get; set; }

        public IDictionary<string, object> Properties { get; private set; }


        protected virtual TMapping InstanciateMapping()
        {
            return new TMapping();
        }
        
        public void Configure(XElement element)
        {
            var mappingElems = element.Elements("mapping").ToList();
            if (mappingElems.Any())
            {
                Mappings = new TMapping[0];
                foreach (var elem in mappingElems)
                {
                    var map = InstanciateMapping();
                    map.Configure(elem);
                    Mappings = Mappings.Concat(new[] {map}).ToArray();
                }
            }
        }
    }
}
