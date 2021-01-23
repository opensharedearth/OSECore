using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace OSECoreUI.Document
{
    public class DocTypeRegistry : ICollection<DocType>
    {
        public static DocTypeRegistry Instance { get; } = new DocTypeRegistry();

        private Dictionary<string, DocType> _dictionary = new Dictionary<string, DocType>();
        public IEnumerator<KeyValuePair<string, DocType>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public DocTypeRegistry()
        {
            Add(new DocType("OSE", typeof(BaseDocument), ".osedoc", "Open Shared Earth Document", "(Untitled)"));
        }

        public void Clear()
        {
            _dictionary.Clear();
        }


        public int Count => _dictionary.Count;

        public bool IsReadOnly => false;

        public void Add(DocType item)
        {
            _dictionary[item.Name] = item;
        }

        public bool Contains(DocType item)
        {
            return _dictionary.ContainsKey(item.Name);
        }

        public void CopyTo(DocType[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(DocType item)
        {
            return _dictionary.ContainsKey(item.Name) && _dictionary.Remove(item.Name);
        }

        IEnumerator<DocType> IEnumerable<DocType>.GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }

        public DocType this[string key]
        {
            get => _dictionary.ContainsKey(key) ? _dictionary[key] : new DocType();
            set => _dictionary[key] = value;
        }
    }
}
