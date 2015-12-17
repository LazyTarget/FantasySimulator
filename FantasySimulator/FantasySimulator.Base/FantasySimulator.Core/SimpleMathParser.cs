using System;
using System.Linq;

namespace FantasySimulator.Core
{
    public class SimpleMathParser
    {
        public double Parse(string expression)
        {
            if(string.IsNullOrWhiteSpace(expression))
                return default(double);
            if (expression.Any(x => x == '(' || x == ')'))
                throw new NotSupportedException("Parantesis not supported by SimpleMathParser");

            var operators = new char[] { '+', '-', '*', '/', '%' };
            var parts = expression.Split(' ');

            double value = 0d;
            double left = 0d;
            string op = "+";
            bool lastWasVal = false;
            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                if (lastWasVal)
                {
                    lastWasVal = false;
                    op = part;
                }
                else
                {
                    lastWasVal = true;
                    left = part.SafeConvert<double>();
                    if (op == "+")
                        value += left;
                    else if (op == "-")
                        value -= left;
                    else if (op == "*")
                        value *= left;
                    else if (op == "/")
                        value /= left;
                    else if (op == "%")
                        value %= left;
                }
            }
            return value;
        }
    }
}
