using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasySimulator.Core
{
    public class MacroResolver
    {
        public string Resolve(string input, IDictionary<string, Func<string, string, object>> macros, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            var result = input;
            while (true)
            {
                var startIndex = result.IndexOf('{');
                if (startIndex < 0)
                    break;
                var endIndex = result.IndexOf('}', startIndex);
                if (endIndex < 0)
                    break;
                var l = endIndex - startIndex - 1;
                var macroDefinition = result.Substring(startIndex + 1, l);

                string macro, format = null;
                var delimeterStartIndex = macroDefinition.IndexOf(':');
                if (delimeterStartIndex > 0)
                {
                    macro = macroDefinition.Substring(0, delimeterStartIndex);
                    format = macroDefinition.Substring(delimeterStartIndex + 1);
                }
                else
                {
                    macro = macroDefinition;
                }


                if (macros.Keys.Any(x => string.Equals(macro, x, stringComparison)))
                {
                    var macroFunc = macros.Where(x => string.Equals(macro, x.Key, stringComparison)).Select(x => x.Value).First();
                    var value = macroFunc?.Invoke(macro, format);
                    var str = ( value ?? "" ).ToString();

                    var tempA = result.Substring(0, startIndex);
                    var tempB = result.Substring(endIndex + 1);
                    var res = tempA + str + tempB;
                    result = res;
                }
                else
                {
                    throw new Exception($"Macro '{macro}' not found");
                }
            }
            return result;
        }

    }
}
