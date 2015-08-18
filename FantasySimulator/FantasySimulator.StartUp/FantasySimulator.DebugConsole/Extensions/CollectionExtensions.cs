using System.Collections;
using System.Linq;

namespace FantasySimulator.DebugConsole
{
    public static class CollectionExtensions
    {
        public static TObj[] Append<TObj>(this TObj[] array, TObj obj)
        {
            var list = (array ?? new TObj[0]).ToList();
            list.Add(obj);
            var res = list.ToArray();
            return res;
        }


        public static TObj CastElementAt<TObj>(this IList list, int index)
        {
            var obj = list[index];
            var res = (TObj) obj;
            return res;
        }

    }
}
