using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using FantasySimulator.Core;
using FantasySimulator.Core.Classes;
using FantasySimulator.Core.Diagnostics;
using FantasySimulator.Interfaces;

namespace FantasySimulator.Simulator.Soccer.Structs
{
    public abstract class Mapper<TMapping> : IXmlConfigurable, IHasProperties
        where TMapping : Mapping
    {
        private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);

        protected Mapper()
        {
            Properties = new DictionaryEx<string, object>();
            MappingMode = MappingMode.FirstMatch;
            Mappings = new List<TMapping>();
        }

        public IDictionary<string, object> Properties { get; private set; }

        public MappingMode MappingMode
        {
            get { return Properties["MappingMode"].SafeConvert<MappingMode>(); }
            set { Properties["MappingMode"] = value; }
        }

        public IList<TMapping> Mappings { get; private set; }


        protected abstract TMapping InstanciateMapping(XElement elem);

        
        public virtual void Configure(XElement element)
        {
            var propertyElems = element.Elements("property").Where(x => x != null).ToList();
            if (propertyElems.Any())
            {
                foreach (var elem in propertyElems)
                {
                    var propertyName = elem.GetAttributeValue("name");
                    if (string.IsNullOrWhiteSpace(propertyName))
                        continue;
                    try
                    {
                        object value = elem.InstantiateElement();
                        Properties[propertyName] = value;
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"Error instantiating property '{propertyName}'", ex);
                    }
                }
            }


            var mappingElems = element.Elements("mapping").ToList();
            if (mappingElems.Any())
            {
                Mappings.Clear();
                foreach (var elem in mappingElems)
                {
                    try
                    {
                        var map = InstanciateMapping(elem);
                        map.Configure(elem);
                        Mappings.Add(map);
                    }
                    catch (Exception ex)
                    {
                        _log.Error("Error instantiating mapping", ex);
                    }
                }
            }
        }


        public virtual IEnumerable<TMapping> Test(ValueMap values)
        {
            if (Mappings.Any())
            {
                var validMappings = new List<TMapping>();
                foreach (var map in Mappings)
                {
                    var valid = map.Test(values);
                    if (valid)
                    {
                        validMappings.Add(map);
                    }
                }
                
                if (validMappings.Any())
                {
                    if (MappingMode == MappingMode.AllMatching)
                    {
                        foreach (var map in validMappings)
                        {
                            yield return map;
                        }
                    }
                    else if (MappingMode == MappingMode.FirstMatch)
                        yield return validMappings.First();
                    else if (MappingMode == MappingMode.LastMatch)
                        yield return validMappings.Last();
                    else
                    {
                        throw new NotSupportedException($"MappingMode '{MappingMode}' is not supported here");
                    }
                }
                else
                {
                    
                }
            }
        }

    }
}
