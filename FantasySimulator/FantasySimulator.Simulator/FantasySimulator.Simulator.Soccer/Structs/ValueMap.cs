using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FantasySimulator.Simulator.Soccer.Structs
{
    public class ValueMap : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly IDictionary<string, Val> _data = new Dictionary<string, Val>();
        

        public object this[string key]
        {
            get
            {
                return _data[key];
            }
            set
            {
                Val val;
                if (!TryGetValue(key, out val))
                    val = new Val();
                val.Unit = key;
                val.Value = value;
                _data[key] = val;
            }
        }

        public int Count { get { return _data.Count; } }

        public bool ContainsKey(string key)
        {
            return _data.ContainsKey(key);
        }

        public void Add(string key, object value)
        {
            this[key] = value;
        }

        public bool TryGetValue(string key, out Val value)
        {
            return _data.TryGetValue(key, out value);
        }

        public bool Remove(string key)
        {
            return _data.Remove(key);
        }

        public void Clear()
        {
            _data.Clear();
        }


        #region IEnumerable

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _data.Select(x => new KeyValuePair<string, object>(x.Key, x.Value.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }


    public class Val
    {
        public string Unit { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Unit))
                return string.Format("[{0}]: '{1}'", Unit, Value);
            if (Value != null)
                return Value.ToString();
            return base.ToString();
        }
    }
}
