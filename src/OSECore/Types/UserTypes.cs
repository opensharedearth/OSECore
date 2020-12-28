using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace OSECore.Types
{
    public class UserTypes : IDictionary<string, UserType>
    {
        private static UserTypes s_userTypes = new UserTypes();
        public static UserTypes Instance => s_userTypes;

        private readonly IDictionary<string, UserType> _dictionary = new Dictionary<string, UserType>();
        public IEnumerator<KeyValuePair<string, UserType>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _dictionary).GetEnumerator();
        }

        public void Add(KeyValuePair<string, UserType> item)
        {
            _dictionary.Add(item);
        }

        public static void Add(UserType item)
        {
            if (item != null && !String.IsNullOrEmpty(item.Name))
            {
                Instance[item.Name] = item;
            }
        }

        public static UserType Find(string name)
        {
            if (!String.IsNullOrEmpty(name) && Instance.TryGetValue(name, out UserType ut))
            {
                return ut;
            }

            return null;
        }

        public static bool IsDefined(string name)
        {
            return Instance.ContainsKey(name);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, UserType> item)
        {
            return _dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, UserType>[] array, int arrayIndex)
        {
            _dictionary.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, UserType> item)
        {
            return _dictionary.Remove(item);
        }

        public int Count => _dictionary.Count;

        public bool IsReadOnly => _dictionary.IsReadOnly;

        public void Add(string key, UserType value)
        {
            _dictionary.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return _dictionary.Remove(key);
        }

        public bool TryGetValue(string key, out UserType value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public UserType this[string key]
        {
            get => _dictionary[key];
            set => _dictionary[key] = value;
        }

        public ICollection<string> Keys => _dictionary.Keys;

        public ICollection<UserType> Values => _dictionary.Values;
    }
}
