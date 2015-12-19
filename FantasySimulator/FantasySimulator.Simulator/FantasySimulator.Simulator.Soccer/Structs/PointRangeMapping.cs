using System;
using System.Linq;
using System.Xml.Linq;
using FantasySimulator.Core;

namespace FantasySimulator.Simulator.Soccer.Structs
{
    public class PointRangeMapping : Mapping<ValuePredicate>
    {
        public PointMapper Mapper
        {
            get { return Properties["Mapper"].SafeConvert<PointMapper>(); }
            set { Properties["Mapper"] = value; }
        }
        

        protected override ValuePredicate InstanciatePredicate(XElement elem)
        {
            var obj = elem.InstantiateElement();
            var pred = (ValuePredicate) obj;
            return pred;
        }

    }
}