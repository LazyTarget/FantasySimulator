using System.Collections;
using System.Collections.Generic;

namespace FantasySimulator.Core.Classes
{
    public class DictionaryEx<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> _data;

        public DictionaryEx()
        {
            _data = new Dictionary<TKey, TValue>();
        }

        public DictionaryEx(IDictionary<TKey, TValue> data)
        {
            _data = data;
        }


        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _data).GetEnumerator();
        }

        public virtual void Add(KeyValuePair<TKey, TValue> item)
        {
            _data.Add(item);
        }

        public virtual void Clear()
        {
            _data.Clear();
        }

        public virtual bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _data.Contains(item);
        }

        public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _data.CopyTo(array, arrayIndex);
        }

        public virtual bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return _data.Remove(item);
        }

        public virtual int Count
        {
            get { return _data.Count; }
        }

        public virtual bool IsReadOnly
        {
            get { return _data.IsReadOnly; }
        }

        public virtual bool ContainsKey(TKey key)
        {
            return _data.ContainsKey(key);
        }

        public virtual void Add(TKey key, TValue value)
        {
            _data.Add(key, value);
        }

        public virtual bool Remove(TKey key)
        {
            return _data.Remove(key);
        }

        public virtual bool TryGetValue(TKey key, out TValue value)
        {
            return _data.TryGetValue(key, out value);
        }

        public virtual TValue this[TKey key]
        {
            get
            {
                var res = default(TValue);
                if (ContainsKey(key))
                    res = _data[key];
                return res;
            }
            set { _data[key] = value; }
        }

        public virtual ICollection<TKey> Keys
        {
            get { return _data.Keys; }
        }

        public virtual ICollection<TValue> Values
        {
            get { return _data.Values; }
        }
    }
}
