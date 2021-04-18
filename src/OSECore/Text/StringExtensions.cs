using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSECore.Text
{

    /// <summary>   String extensions defined as part of the core library. </summary>
    public static class StringExtensions
    {
        /// <summary>   A string extension method that parses objects from the input. </summary>
        ///
        /// <param name="s">        The intput string to act on. </param>
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="dts">      A variable-length parameters list containing the types expected in the input. </param>
        ///
        /// <returns>   The array of objects found in the input. </returns>
        public static object[] Parse(this string s, string format, params Type[] dts)
        {
            LineSegment[] fes = GetFormatSegments(format);
            List<object> dos = new List<object>();
            int i0 = 0;
            foreach(LineSegment fe in fes)
            {
                if (fe is FormatLiteral fl)
                {
                    if (!fl.Match(s, i0)) break;
                    i0 += fe.Length;
                }
                else if (fe is FormatSpec fs)
                {
                    int index = fs.Index;
                    while (index >= dos.Count && index < dts.Length) dos.Add(null);
                    if (index < dos.Count)
                    {
                        i0 = fs.Parse(s, i0, dts[index], out object d);
                        dos[index] = d;
                    }
                }
            }

            return dos.ToArray();
        }

        private static LineSegment[] GetFormatSegments(string format)
        {
            List<LineSegment> fs = new List<LineSegment>();
            int p = -1;
            int j = 0;
            for (int i = 0; i < format.Length; ++i)
            {
                char c = format[i];
                switch (c)
                {
                    case '{':
                        if (p < 0)
                            p = i;
                        else if (p == i - 1)
                        {
                            fs.Add(new FormatLiteral(format, j, i - j));
                            j = i + 1;
                            p = -1;
                        }
                        else
                            p = i;
                        break;
                    case '}':
                        if (p >= 0)
                        {
                            if (p > j) fs.Add(new FormatLiteral(format, j, p - j));
                            fs.Add(new FormatSpec(format, p, i - p + 1));
                            j = i + 1;
                        }
                        else if (i < format.Length && format[i] == '}')
                        {
                            fs.Add(new FormatLiteral(format, j, i - j + 1));
                            i++;
                            j = i + 1;
                        }

                        break;
                    default:
                        break;
                }
            }
            if(j < format.Length)fs.Add(new FormatLiteral(format,j,format.Length - j));
            return Normalize(fs.ToArray());
        }

        private static LineSegment[] Normalize(LineSegment[] fss)
        {
            List<LineSegment> fss1 = new List<LineSegment>();
            for (int i = 0; i < fss.Length; ++i)
            {
                if (i < fss.Length - 1 && fss[i] is FormatSpec fs && fss[i + 1] is FormatLiteral fl)
                {
                    fss1.Add(new FormatSpec(fs, fl));
                }
                else
                {
                    fss1.Add(fss[i]);
                }
            }

            return fss1.ToArray();
        }

        public static string SafeSubstring(this string s, int start, int length)
        {
            if (!String.IsNullOrEmpty(s) && start >= 0 && start < s.Length)
            {
                return s.Substring(start, Math.Min(s.Length - start, length));
            }

            return "";
        }

        public static char SafeIndex(this string s, int index)
        {
            if (!String.IsNullOrEmpty(s) && index >= 0 && index < s.Length)
                return s[index];
            return '\0';
        }

        public static string SafeString(this string s)
        {
            return s ?? "";
        }

        public static bool Match(this string s, int index, string t)
        {
            if (s != null && t != null && t.Length <= s.Length - index)
            {
                for (int i = index, j = 0; i < s.Length && j < t.Length; ++i, ++j)
                {
                    if (s[i] != t[j]) return false;
                }

                return true;
            }

            return false;
        }

        public static string[] SplitLines(this string s)
        {
            List<string> lines = new List<string>();
            List<char> line = new List<char>();
            if (s != null)
            {
                foreach(char c in s)
                {
                    switch (c)
                    {
                        case '\r':
                            continue;
                        case '\n':
                            lines.Add(new string(line.ToArray()));
                            break;
                        default:
                            line.Add(c);
                            break;
                    }
                }

                if (line.Count > 0) lines.Add(new string(line.ToArray()));
            }

            return lines.ToArray();
        }

        public static string[] SplitLine(this string s, string delimiters = " \t\r\n")
        {
            List<string> fields = new List<string>();
            if (s != null)
            {
                List<char> field = new List<char>();
                char[] dchar = delimiters.ToCharArray();
                foreach (char c in s)
                {
                    if (dchar.Contains(c))
                    {
                        if (field.Count > 0)
                        {
                            fields.Add((new string(field.ToArray())).Trim());
                            field.Clear();
                        }
                    }
                    else
                    {
                        field.Add(c);
                    }
                }

                if (field.Count > 0) fields.Add((new string(field.ToArray())).Trim());
            }

            return fields.ToArray();
        }
        public static T GetValue<T>(this string s, T defaultValue = default(T)) where T : struct
        {
            if (s == null)
            {
                return default(T);
            }
            else if (typeof(T) == typeof(string))
            {
                return (T)(object)s;
            }
            else if (typeof(T) == typeof(int) && int.TryParse(s, out int iv))
            {
                return (T)(object)iv;
            }
            else if (typeof(T) == typeof(float) && float.TryParse(s, out float fv))
            {
                return (T)(object)fv;
            }
            else if (typeof(T) == typeof(double) && double.TryParse(s, out double dv))
            {
                return (T)(object)dv;
            }
            else if (typeof(T) == typeof(bool) && bool.TryParse(s, out bool bv))
            {
                return (T)(object)bv;
            }
            else if (typeof(T) == typeof(DateTime) && DateTime.TryParse(s, out DateTime dtv))
            {
                return (T)(object)dtv;
            }
            else if (typeof(T).IsEnum && Enum.TryParse<T>(s, out T ev))
            {
                return (T)ev;
            }
            else
            {
                return defaultValue;
            }
        }
        public static string SetValue(object value)
        {
            if (value == null)
            {
                return null;
            }
            else if (value is int || value is bool)
            {
                return value.ToString();
            }
            else if (value is float)
            {
                return ((float)value).ToString("R");
            }
            else if (value is double)
            {
                return ((double)value).ToString("R");
            }
            else if (value is DateTime)
            {
                return ((DateTime)value).ToString("R");
            }
            else if (value.GetType().IsEnum)
            {
                return value.ToString();
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
