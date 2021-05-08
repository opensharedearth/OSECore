using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSECommand
{
    public class UsageElement : IEquatable<UsageElement>, IComparable<UsageElement>
    {
        public const int ColumnWidth = 20;
        public UsageElement(string description)
        {
            Description = description;
        }
        public string Description { get; private set; } = null;
        public static string GetPadding(int column)
        {
            int padding = Math.Max(ColumnWidth - column,1);
            return new string(' ', padding);
        }

        public bool Equals(UsageElement other)
        {
            if((object)other != null)
            {
                return GetType() == other.GetType() && ToString() == other.ToString();
            }
            return false;
        }
        public override bool Equals(object obj)
        {
            if (obj is UsageElement ue)
                return Equals(ue);
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public static bool operator==(UsageElement a, UsageElement b)
        {
            if ((object)a == null)
                return (object)b == null;
            else if ((object)b == null)
                return false;
            else
                return a.Equals(b);
        }
        public static bool operator!=(UsageElement a, UsageElement b)
        {
            if ((object)a == null)
                return (object)b != null;
            else if ((object)b == null)
                return true;
            else return !a.Equals(b);
        }

        public int CompareTo(UsageElement other)
        {
            if((object)other != null)
            {
                if (GetUsageType() == other.GetUsageType())
                    return ToString().CompareTo(other.ToString());
                else
                    return GetUsageType().CompareTo(other.GetUsageType());
            }
            return 1;
        }
        public static bool operator<(UsageElement a, UsageElement b)
        {
            if ((object)a == null)
                return (object)b != null;
            else if ((object)b == null)
                return false;
            else
                return a.CompareTo(b) < 0;
        }
        public static bool operator>(UsageElement a, UsageElement b)
        {
            if ((object)a == null)
                return false;
            else if ((object)b == null)
                return (object)a != null;
            else
                return a.CompareTo(b) > 0;
        }
        public static bool operator<=(UsageElement a, UsageElement b)
        {
            if ((object)a == null)
                return true;
            else if ((object)b == null)
                return (object)a == null;
            else
                return a.CompareTo(b) <= 0;

        }
        public static bool operator>=(UsageElement a, UsageElement b)
        {
            if ((object)a == null)
                return (object)b == null;
            else if ((object)b == null)
                return false;
            else
                return a.CompareTo(b) >= 0;
        }
        public virtual UsageType GetUsageType()
        {
            return UsageType.Unkonwn;
        }
        public virtual string GetHeading()
        {
            return "";
        }

        public bool HasDescription => !String.IsNullOrEmpty(Description);
    }
}
