using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FantasySimulator.Core;

namespace FantasySimulator.Simulator.Soccer.Structs
{
    public class RangeMapping : IXmlConfigurable
    {
        public RangeMapping()
        {
            Properties = new Dictionary<string, object>();
            Predicates = new RangePredicate[0];
            PredicateMode = PredicateMode.All;
        }

        public RangePredicate[] Predicates { get; set; }

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
                    try
                    {
                        var propertyName = elem.GetAttributeValue("name");
                        if (string.IsNullOrWhiteSpace(propertyName))
                            continue;
                        object value = elem.InstantiateElement();
                        Properties[propertyName] = value;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }


            var predicateElems = element.Elements("predicate").ToList();
            if (predicateElems.Any())
            {
                Predicates = new RangePredicate[0];
                foreach (var elem in predicateElems)
                {
                    var pred = new RangePredicate();
                    pred.Configure(elem);
                    Predicates = Predicates.Concat(new[] { pred }).ToArray();
                }
            }
        }


        public virtual bool Test(ValueMap values)
        {
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
                                return true;
                        }
                        else if (!valid)
                            return false;
                    }
                    else
                        return false;
                }

                if (PredicateMode == PredicateMode.Any)
                    return numberOfValid > 0;
                if (PredicateMode == PredicateMode.All)
                    return numberOfValid == Predicates.Length;
            }
            return true;
        }

    }
}
