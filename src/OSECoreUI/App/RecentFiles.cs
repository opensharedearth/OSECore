using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace OSECoreUI.App
{
    [Serializable]
    public class RecentFiles : IList<string>, ISerializable, IDirty
    {
        private readonly IList<string> _list;
        public const int DefaultMaxFiles = 4;
        public static int MaxFiles = DefaultMaxFiles;
        public RecentFiles()
        {
            _list = new List<string>();
            _isDirty = false;
        }

        public RecentFiles(SerializationInfo info, StreamingContext context)
        {
            try
            {
                string[] paths = (string[]) info.GetValue("Paths", typeof(string[]));
                _list = new List<string>(paths);
            }
            catch(Exception e)
            {
                Trace.WriteLine("Unable to deserialize recent files: " + e.Message);
                _list = new List<string>();
            }

            _isDirty = false;
        }


        public IEnumerator<string> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _list).GetEnumerator();
        }

        public void Add(string item)
        {
            if (!Contains(item))
            {
                _list.Insert(0,item);
                while (_list.Count > MaxFiles)
                {
                    _list.RemoveAt(_list.Count - 1);
                }
                Dirty();
            }
        }

        public void Clear()
        {
            _list.Clear();
            Dirty();
        }

        public bool Contains(string item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(string item)
        {
            bool result = _list.Remove(item);
            Dirty();
            return result;
        }

        public int Count => _list.Count;

        public bool IsReadOnly => _list.IsReadOnly;

        public int IndexOf(string item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            _list.Insert(index, item);
            Dirty();
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
            Dirty();
        }

        public string this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Paths",_list.ToArray());
        }

        private bool _isDirty;
        public void Dirty()
        {
            _isDirty = true;
        }

        public void Undirty()
        {
            _isDirty = false;
        }

        public bool IsDirty => _isDirty;
    }
}
