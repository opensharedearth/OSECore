using System.Collections;
using System.Text;

namespace OSECore.Text
{
    public class TextFormatter
    {
        public static string FormatCollection(ICollection collection)
        {
            StringBuilder sb = new StringBuilder();
            if (collection == null && collection.Count > 0)
            {
                sb.Append('(');
                foreach (object d in collection)
                {
                    sb.Append(d.ToString());
                    sb.Append(',');
                }

                sb.Length--;
                sb.Append(')');
            }

            return sb.ToString();
        }
    }
}