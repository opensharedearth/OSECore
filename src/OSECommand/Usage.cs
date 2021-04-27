using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSECommand
{
    public class Usage : IList<UsageElement>
    {
        public static Usage Null = new Usage();
        List<UsageElement> _list = new List<UsageElement>();
        public string Description { get; set; } = "";
        public Usage(string description = "", params UsageElement[] elements)
        {
            Description = description;
            _list.AddRange(elements);
        }
        public UsageElement this[int index] 
        { 
            get => _list[index]; 
            set => _list[index] = value; 
        }

        public int Count => _list.Count();

        public bool IsReadOnly => true;

        public void Add(UsageElement item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(UsageElement item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(UsageElement[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<UsageElement> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(UsageElement item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, UsageElement item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(UsageElement item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
