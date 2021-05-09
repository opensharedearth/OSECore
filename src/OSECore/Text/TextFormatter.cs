using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace OSECore.Text
{
    public class TextFormatter
    {
        public static string FormatCollection<T>(ICollection<T> collection)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('(');
            if (collection != null && collection.Count > 0)
            {
                bool first = true;
                foreach (object d in collection)
                {
                    if(!first)
                        sb.Append(',');
                    sb.Append(d.ToString());
                    first = false;
                }
            }
            sb.Append(')');

            return sb.ToString();
        }
    }
}