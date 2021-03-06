﻿using System;
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
    public abstract class Mapper<TMapping> : IHasMappingGroups<TMapping>, IXmlConfigurable, IHasProperties
        where TMapping : Mapping
    {
        private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);

        protected Mapper()
        {
            Properties = new DictionaryEx<string, object>();
            MappingMode = MappingMode.FirstMatch;
            MappingGroups = new List<IMappingGroup<TMapping>>();
        }

        public IDictionary<string, object> Properties { get; private set; }

        public MappingMode MappingMode
        {
            get { return Properties["MappingMode"].SafeConvert<MappingMode>(); }
            set { Properties["MappingMode"] = value; }
        }

        public IList<IMappingGroup<TMapping>> MappingGroups { get; private set; }


        protected abstract IMappingGroup<TMapping> InstanciateMappingGroup(XElement elem);

        protected internal abstract TMapping InstanciateMapping(XElement elem);

        
        public virtual void Configure(XElement element)
        {
            var mappingElems = element.Elements("mapping").ToList();
            var mappingGroupElems = element.Elements("mappingGroup").ToList();
            if (mappingElems.Any() || mappingGroupElems.Any())
                MappingGroups.Clear();

            if (mappingElems.Any())
            {
                var group = new MappingGroup<TMapping>(this);
                group.MappingMode = MappingMode;
                MappingGroups.Add(group);

                foreach (var elem in mappingElems)
                {
                    try
                    {
                        var map = InstanciateMapping(elem);
                        group.Mappings.Add(map);
                    }
                    catch (Exception ex)
                    {
                        _log.Error("Error instantiating mapping", ex);
                    }
                }
            }
            if (mappingGroupElems.Any())
            {
                foreach (var elem in mappingGroupElems)
                {
                    try
                    {
                        var group = InstanciateMappingGroup(elem);
                        MappingGroups.Add(group);
                    }
                    catch (Exception ex)
                    {
                        _log.Error("Error instantiating mappingGroup", ex);
                    }
                }
            }
        }


        public virtual IEnumerable<TMapping> Test(ValueMap values)
        {
            if (MappingGroups.Any())
            {
                var validMappings = MappingGroups.SelectMany(x => x.Test(values)).ToList();

                //var validMappings = new List<TMapping>();
                //foreach (var map in mappings)
                //{
                //    var valid = map.Test(values);
                //    if (valid)
                //    {
                //        validMappings.Add(map);
                //    }
                //}
                
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
