using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using FantasySimulator.Core;
using FantasySimulator.Core.Diagnostics;

namespace FantasySimulator.Simulator.Soccer.Structs
{
    public class PointRangeMapping : Mapping<ValuePredicate>
    {
        private static readonly ILog _log = Log.GetLog(MethodBase.GetCurrentMethod().DeclaringType);

        public PointMapper PointMapper
        {
            get { return Properties["PointMapper"].SafeConvert<PointMapper>(); }
            set { Properties["PointMapper"] = value; }
        }
        

        protected override ValuePredicate InstanciatePredicate(XElement elem)
        {
            ValuePredicate pred;
            var predicateType = elem.GetAttributeValue("type");
            if (string.IsNullOrWhiteSpace(predicateType))
            {
                throw new Exception($"{nameof(RangePredicate)} requires you to specify the predicate type, as: <predicate type=\"{nameof(RangePredicate)}\">");
            }
            var obj = elem.InstantiateElement();
            pred = (ValuePredicate) obj;
            return pred;
        }

    }
}