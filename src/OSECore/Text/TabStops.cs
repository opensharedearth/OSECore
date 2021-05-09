using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSECore.Text
{
    public class TabStops : IList<int>, ICollection<int>, IEquatable<TabStops>
    {
        public TabStops()
        {

        }
        public TabStops(TabStops d)
        {
            _list.AddRange(d);
        }
        public TabStops(string s)
        {
            try
            {
                _list.AddRange(TextParser.ParseCollection<int>(s));
                Normalize();
            }
            catch(Exception ex)
            {
                throw new ArgumentException("Invalid tab stop string argument", ex);
            }
        }
        public TabStops(IEnumerable<int> tabstops)
        {
            _list.AddRange(from i in tabstops where i > 0 select i);
            Normalize();
        }

        private void Normalize()
        {
            HashSet<int> hash = new HashSet<int>(_list);
            _list = new List<int>(hash);
            _list.Sort();
        }

        List<int> _list = new List<int>();
        public int this[int index] 
        { 
            get => _list[index]; 
            set => throw new NotImplementedException(); 
        }

        public int Count => _list.Count;

        public bool IsReadOnly => true;

        public void Add(int item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(int item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(int[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<int> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(int item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, int item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(int item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }
        public int GetTabColumn(int tabstop)
        {
            int index = Math.Min(Math.Max(tabstop - 1,0), _list.Count - 1);
            if (index >= 0)
                return _list[index];
            else
                return 1;
        }
        public void AddTabStop(int column)
        {
            if(column > 0)
            {
                _list.Add(column);
                Normalize();
            }
        }
        public void RemoveTabstop(int column)
        {
            _list.Remove(column);
        }
        public string GetPadding(int tabstop, int column = 1)
        {
            int tabColumn = GetTabColumn(tabstop);
            int padding = tabColumn > column ? tabColumn - column : 1;
            return new string(' ', padding);
        }

        public bool Equals(TabStops other)
        {
            if(other != null)
            {
                if(Count == other.Count)
                {
                    for (int i = 0; i < Count; ++i)
                        if (this[i] != other[i])
                            return false;
                    return true;
                }
            }
            return false;
        }
        public override bool Equals(object obj)
        {
            if(obj is TabStops ts)
                return Equals(ts);
            else
                return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            int hash = 0;
            foreach (var i in this)
                hash ^= i.GetHashCode();
            return hash;
        }
        public static bool operator==(TabStops a, TabStops b)
        {
            if ((object)a == null)
                return (object)b == null;
            return a.Equals(b);
        }
        public static bool operator!=(TabStops a, TabStops b)
        {
            if ((object)a == null)
                return (object)b != null;
            return !a.Equals(b);
        }
        public override string ToString()
        {
            return TextFormatter.FormatCollection(this);
        }
    }
}
