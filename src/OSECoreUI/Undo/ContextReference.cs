using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSECoreUI.Undo
{
    public class ContextReference : ICollection
    {
        object[] _references;
        public ContextReference(params object[] references)
        {
            _references = references;
        }
        public object this[int index]
        {
            get
            {
                if (index >= 0 && index < _references.Length)
                {
                    return _references[index];
                }
                else
                {
                    return null;
                }
            }
        }

        public int Count
        {
            get
            {
                return _references.Length;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public object SyncRoot
        {
            get
            {
                return this;
            }
        }

        public void CopyTo(Array array, int index)
        {
            _references.CopyTo(array, index);
        }

        public override bool Equals(object obj)
        {
            ContextReference d1 = obj as ContextReference;
            if(d1 != null && Count == d1.Count)
            {
                for(int i = 0; i < Count; i++)
                {
                    if (this[i] != d1[i]) return false;
                }
                return true;
            }
            return false;
        }
        public override int GetHashCode()
        {
            int code = 0;
            foreach(object d in this)
            {
                if (d != null) code += d.GetHashCode();
            }
            return code;
        }
        public override string ToString()
        {
            if(Count == 0)
            {
                return "<Empty>";
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("(");
                foreach (object d in this)
                {
                    if (d == null)
                    {
                        sb.Append("<null>");
                    }
                    else
                    {
                        sb.Append(d.ToString());
                    }
                    sb.Append(",");
                }
                sb.Length--;
                sb.Append(")");
                return sb.ToString();
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _references.GetEnumerator();
        }
        public static bool operator==(ContextReference d0, ContextReference d1)
        {
            return (object)d0 != null && d0.Equals(d1);
        }
        public static bool operator!=(ContextReference d0, ContextReference d1)
        {
            return (object)d0 != null ? !d0.Equals(d1) : (object)d1 != null;
        }
    }
}
