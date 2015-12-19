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
    public abstract class Mapping : IXmlConfigurable, IHasProperties
    {
        private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);

        protected Mapping()
        {
            Properties = new DictionaryEx<string, object>();
            PredicateMode = PredicateMode.Any;
        }

        public IDictionary<string, object> Properties { get; private set; }

        public PredicateMode PredicateMode
        {
            get { return Properties["PredicateMode"].SafeConvert<PredicateMode>(); }
            set { Properties["PredicateMode"] = value; }
        }

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
        }
        
        public abstract bool Test(ValueMap values);

    }



    public abstract class Mapping<TPredicate> : Mapping
        where TPredicate : ValuePredicate
    {
        private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);

        public Mapping()
        {
            Predicates = new List<TPredicate>();
        }

        public IList<TPredicate> Predicates { get; private set; }


        protected abstract TPredicate InstanciatePredicate(XElement elem);


        public override void Configure(XElement element)
        {
            base.Configure(element);

            var predicateElems = element.Elements("predicate").ToList();
            if (predicateElems.Any())
            {
                Predicates.Clear();
                foreach (var elem in predicateElems)
                {
                    var pred = InstanciatePredicate(elem);
                    pred.Configure(elem);
                    Predicates.Add(pred);
                }
            }
        }


        public override bool Test(ValueMap values)
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
                        var val = value.Value;
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
