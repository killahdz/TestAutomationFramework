using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Library.WebDriver
{
    public class ThreadSafeDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
        private readonly object _mutex = new object();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _dictionary).GetEnumerator();
        }

        public TValue Get(TKey key)
        {
            lock (_mutex)
            {
                return _dictionary[key];
            }
        }

        public void Set(TKey key, TValue value)
        {
            lock (_mutex)
            {
                _dictionary[key] = value;
            }
        }

        public void Remove(TKey key)
        {
            lock (_mutex)
            {
                _dictionary.Remove(key);
            }
        }

        public int Count(TKey key)
        {
            lock (_mutex)
            {
                return _dictionary.Count;
            }
        }

        public bool ContainsKey(TKey key)
        {
            lock (_mutex)
            {
                return _dictionary.ContainsKey(key);
            }
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFunc)
        {
            lock (_mutex)
            {
                if (_dictionary.TryGetValue(key, out var value)) return value;
                value = valueFunc(key);
                _dictionary[key] = value;
                return value;
            }
        }
    }
}