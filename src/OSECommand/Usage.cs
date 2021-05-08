using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSECommand
{
    public class Usage : UsageElement, IList<UsageElement>
    {
        public static Usage Null = new Usage();
        List<UsageElement> _list = new List<UsageElement>();
        public Usage(params UsageElement[] elements)
            : base("")
        {
            _list.AddRange(elements);
        }
        public Usage(string description = "", params UsageElement[] elements)
            : base(description)
        {
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
        public string GetProto()
        {
            StringBuilder sb = new StringBuilder();
            foreach(var ue in this.OfType<UsageProto>())
            {
                sb.AppendLine(ue.Proto);
            }
            return sb.ToString();
        }
        public string GetWhere()
        {
            StringBuilder sb = new StringBuilder();
            foreach(var uw in this.OfType<UsageWhere>())
            {
                sb.AppendLine($"{uw.Name}\t{uw.Description}");
            }
            return sb.ToString();
        }
        public override UsageType GetUsageType()
        {
            return UsageType.Usage;
        }
        public void Merge(UsageElement ue)
        {
            if(ue is Usage u)
            {
                _list.AddRange(u);
            }
            else if(ue != null)
            {
                _list.Add(ue);
            }
        }
        public void Normalize()
        {
            HashSet<UsageElement> hash = new HashSet<UsageElement>(_list);
            _list.Clear();
            foreach(var ue in hash)
            {
                _list.Add(ue);
            }
            _list.Sort();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            UsageType current = UsageType.Unkonwn;
            foreach(var ue in this)
            {
                if(current != ue.GetUsageType())
                {
                    sb.AppendLine();
                    sb.AppendLine(ue.GetHeading());
                    current = ue.GetUsageType();
                }
                sb.AppendLine(ue.ToString());
            }
            return sb.ToString();
        }
        public static Usage CreateUsage(string name, char mnemonic, UsageElement ue)
        {
            if(ue == null)
            {
                return new Usage(new UsageSwitch(name, mnemonic, ""));
            }
            else if(ue is Usage u)
            {
                var u1 = new Usage(new UsageSwitch(name, mnemonic, u.Description));
                u1.Merge(u);
                return u1;
            }
            else
            {
                return new Usage(new UsageSwitch(name, mnemonic, ue.Description));
            }
        }
        public static Usage CreateUsage(string name, UsageElement ue)
        {
            if(ue == null)
            {
                return new Usage(new UsageCommand(name, ""));
            }
            else if(ue is Usage u)
            {
                var u1 = new Usage();
                if(u.HasDescription)
                {
                    u1.Merge(new UsageCommand(name, u.Description));
                }
                u1.Merge(u);
                return u1;
            }
            else
            {
                return new Usage(ue);
            }
        }
    }
}
