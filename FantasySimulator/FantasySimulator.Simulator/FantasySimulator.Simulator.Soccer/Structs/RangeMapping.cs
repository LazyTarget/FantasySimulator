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
    public class RangeMapping : IXmlConfigurable, IHasProperties
    {
        private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);

        public RangeMapping()
        {
            Properties = new DictionaryEx<string, object>();
            Predicates = new List<RangePredicate>();
            PredicateMode = PredicateMode.Any;
        }

        public IList<RangePredicate> Predicates { get; set; }

        public PredicateMode PredicateMode
        {
            get { return Properties["PredicateMode"].SafeConvert<PredicateMode>(); }
            set { Properties["PredicateMode"] = value; }
        }

        public IDictionary<string, object> Properties { get; private set; }


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


            var predicateElems = element.Elements("predicate").ToList();
            if (predicateElems.Any())
            {
                Predicates.Clear();
                foreach (var elem in predicateElems)
                {
                    var pred = new RangePredicate();
                    pred.Configure(elem);
                    Predicates.Add(pred);
                }
            }
        }


        public virtual bool Test(ValueMap values)
        {
            var result = true;
            if (Predicates.Any())
            {
                int numberOfValid = 0;
                foreach (var predicate in Predicates)
                {
                    Val value;
                    if (values.TryGetValue(predicate.Unit, out value) && value != null)
                    {
                        var val = value.Value.SafeConvert<double>();
                        var valid = predicate.Test(val);
                        if (valid)
                            numberOfValid++;

                        if (PredicateMode == PredicateMode.Any)
                        {
                            if (valid)
                                break;
                        }
                        else if (!valid)
                            break;
                    }
                    else
                    {
                        break;
                        return false;
                    }
                }

                if (PredicateMode == PredicateMode.Any)
                    result = numberOfValid > 0;
                else if (PredicateMode == PredicateMode.All)
                    result = numberOfValid == Predicates.Count;
                else
                {
                    result = false;     // not implemented
                    throw new NotSupportedException($"PredicateMode '{PredicateMode}' is not supported here");
                }
            }
            return result;
        }

    }
}
