using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Xml.Schema;

namespace OSECore.Text
{

    /// <summary>   A format specifier. This is the representation of a C# string format specifier.  All standard format specfiers are
    /// supported. </summary>
    public class FormatSpec : LineSegment
    {

        private int _index = 0;
        private int _alignment = 0;
        private string _format = "";
        private string _delimiter = "";
        private bool _isValid = false;
        /// <summary>   Constructor taking line start and length. </summary>
        ///
        /// <param name="line">     The line containing the format specfier. </param>
        /// <param name="start">    The zero-based starting index of the specifier. </param>
        /// <param name="length">   The length of the specifier. </param>
        public FormatSpec(string line, int start, int length)
            : base(start, length)
        {
            if (base.IsValid && Length >= 3 && line[Start] == '{' && line[End] == '}')
            {
                int i0 = start + 1;
                i0 = TextParser.ParseInteger(line, i0, out _index, 0);
                i0 = TextParser.ParseDelimiter(line, i0, out string delimiter, ",");
                if(delimiter == ",")i0 = TextParser.ParseInteger(line, i0, out _alignment, 0);
                i0 = TextParser.ParseDelimiter(line, i0, out string colon, ":");
                if (colon == ":")
                {
                    int l = End - i0;
                    if (l > 0)
                    {
                        _format = line.Substring(i0, l);
                        Validate();
                    }
                }
                else if (i0 == End)
                {
                    Validate();
                }
            }
        }

        public FormatSpec(FormatSpec fs, FormatLiteral fl)
        : base(fs.Start, fs.Length)
        {
            _index = fs.Index;
            _alignment = fs.Alignment;
            _format = fs.Format;
            _delimiter = fl.Literal;
            Validate();
        }

        private void Validate()
        {
            _isValid = Index >= 0 && Index < MaxLength && _alignment > -MaxLength && _alignment < MaxLength;
        }

        /// <summary>   Gets the zero-based index of the object to which the specifier is connected. </summary>
        ///
        /// <value> The index. </value>
        public int Index => _index;
        /// <summary>   Gets the alignment count.  This might be either positive or negative and is interpreted differently for
        ///             each format string. </summary>
        ///
        /// <value> The alignment. </value>
        public int Alignment => _alignment;
        /// <summary>   Gets the format string which controls how strings are formatted and parsed. </summary>
        ///
        /// <value> The format string. This cannot be interpreted until the type of the target object is determined.</value>
        public string Format => _format;

        public string StandardFormat => !String.IsNullOrEmpty(_format) ? _format.Substring(0, 1) : "";
        /// <summary>
        /// Gets a value indicating whether this format specifier is valid.  To be valid the entire format specifier must have
        /// been validated.
        /// </summary>
        ///
        /// <value> True if this object is valid, false if not. </value>
        public override bool IsValid => base.IsValid && _isValid;

        public string Delimiter
        {
            get { return _delimiter; }
        }

        /// <summary>   Parses an object from the input. </summary>
        ///
        /// <param name="s">    The input string. </param>
        /// <param name="i0">The zero-based staring index in the input string.</param>
        /// <param name="t">    The type of the obejct which is expected in the input. </param>
        /// <param name="d">    [out] the object which was found in the input or null if no object was found. </param>
        ///
        /// <returns>   The zero-based position of the character following the object which was found. </returns>
        public int Parse(string s, int i0, Type t, out object d)
        {
            if (!String.IsNullOrEmpty(s) && IsValid)
            {
                if (t == typeof(int))
                {
                    return ParseInt(s, i0, out d);
                }
                else if (t == typeof(float))
                {
                    return ParseFloat(s, i0, out d);
                }
                else if (t == typeof(double))
                {
                    return ParseDouble(s, i0, out d);
                }
                else if (t == typeof(DateTime))
                {
                    return ParseDateTime(s, i0, out d);
                }
                else if (t == typeof(TimeSpan))
                {
                    return ParseTimeSpan(s, i0, out d);
                }
                else if (t == typeof(String))
                {
                    return ParseString(s, i0, out d, Delimiter);
                }
            }

            d = null;
            return i0;
        }

        private int ParseString(string s, int i0, out object d, string delimiter)
        {
            int i = i0;
            if (String.IsNullOrEmpty(delimiter))
            {
                d = s.Substring(i0);
                i += s.Length - i0;
            }
            else
            {
                i = s.IndexOf(delimiter, i0, StringComparison.OrdinalIgnoreCase);
                if (i >= i0)
                {
                    d = s.Substring(i0, i - i0);
                }
                else
                {
                    d = "";
                }
            }
            return i;
        }

        private int ParseTimeSpan(string s, int i0, out object d)
        {
            int i = i0;
            i = TextParser.ParseInteger(s, i, out int hours);
            i = TextParser.ParseDelimiter(s, i, out string sep1, ".:");
            int days = 0;
            string sep2 = "";
            if (sep1 == ".")
            {
                days = hours;
                i = TextParser.ParseInteger(s, i, out hours);
                i = TextParser.ParseDelimiter(s, i, out sep2, ":");
            }
            else if (sep1 != ":")
            {
                d = null;
                return i0;
            }
            i = TextParser.ParseInteger(s, i, out int minutes);
            i = TextParser.ParseDelimiter(s, i, out string sep3, ":");
            i = TextParser.ParseInteger(s, i, out int seconds);
            i = TextParser.ParseDelimiter(s, i, out string sep4, ":.");
            if (sep4 == ":")
            {
                days = hours;
                hours = minutes;
                minutes = seconds;
                i = TextParser.ParseInteger(s, i, out seconds);
                i = TextParser.ParseDelimiter(s, i, out sep4, ".");
            }
            i = TextParser.ParseInteger(s, i, out int fractions);
            if (sep3 == ":" && (sep1 == ":" || sep2 == ":"))
            {
                if (TimeSpan.TryParse(s.Substring(i0, i - i0), null, out TimeSpan ts))
                {
                    d = ts;
                    return i;
                }
            }

            d = null;
            return i0;
        }

        private int ParseDateTime(string s, int i0, out object d)
        {
            int i = i0;
            int j = i0;
            int k = i0;
            switch (StandardFormat)
            {
                case "":
                case "f":
                case "F":
                case "g":
                case "R":
                case "u":
                case "U":
                    j = ParseDate(s, i0);
                    i = ParseTime(s, j);
                    if (j > i0 && i > j) break;
                    d = DateTime.MinValue;
                    return i0;
                case "O":
                case "s":
                    j = ParseDate(s, i0);
                    k = TextParser.ParseCharacter(s, j, out char sep);
                    i = ParseTime(s, k);
                    if (j > i0 && i > j && sep == 'T') break;
                    d = DateTime.MinValue;
                    return i0;
                case "D":
                case "d":
                case "M":
                case "Y":
                    i = ParseDate(s, i0);
                    break;
                case "t":
                case "T":
                    i = ParseTime(s, i0);
                    break;
            }

            if (i > i0 && DateTime.TryParse(s.Substring(i0, i - i0), null, DateTimeStyles.AdjustToUniversal, out DateTime dt))
            {
                d = dt;
                return i;
            }
            d = null;
            return i0;
        }

        private int ParseTime(string s, int i0)
        {
            int i = i0;
            i = TextParser.ParseInteger(s, i, out int hour);
            i = TextParser.ParseDelimiter(s, i, out string sep1, ":");
            i = TextParser.ParseInteger(s, i, out int minutes);
            i = TextParser.ParseDelimiter(s, i, out string sep2, ":");
            if (sep2 == ":")
            {
                i = TextParser.ParseInteger(s, i, out int seconds);
            }

            if (sep1 != ":") return i0;
            if (StandardFormat == "R")
            {
                i = TextParser.ParseName(s, i, out string gmt);
                if (gmt != "GMT") return i0;
            }
            else if (StandardFormat == "u")
            {
                i = TextParser.ParseCharacter(s, i, out char z);
                if (z != 'Z') return i0;
            }
            else if (StandardFormat == "O")
            {
                i = TextParser.ParseDelimiter(s, i, out string sep3, ".");
                i = TextParser.ParseInteger(s, i, out int ticks);
                int j = TextParser.ParseDelimiter(s, i, out string sep4, "-");
                int k = TextParser.ParseCharacter(s, i, out char z);
                if (j > i)
                {
                    i = j;
                    i = TextParser.ParseInteger(s, i, out int tzh);
                    i = TextParser.ParseDelimiter(s, i, out string sep5, ":");
                    i = TextParser.ParseInteger(s, i, out int tzm);
                    if (sep5 != ":") return i0;
                }
                else if (z == 'Z')
                {
                    i = k;
                }
            }
            else if((Format.Length == 1 && "fFgtTU".IndexOf(Format[0]) >= 0) || Format.Length == 0)
            {
                i = TextParser.ParseName(s, i, out string ampm);
                if (ampm != "AM" && ampm != "PM") return i0;
            }

            return i;
        }

        private int ParseDate(string s, int i0)
        {
            int i = i0;
            switch (Format)
            {
                case "":
                case "d":
                case "g":
                    i = TextParser.ParseInteger(s, i, out int month);
                    i = TextParser.ParseDelimiter(s, i, out string sep1, "/");
                    i = TextParser.ParseInteger(s, i, out int day);
                    i = TextParser.ParseDelimiter(s, i, out string sep2, "/");
                    i = TextParser.ParseInteger(s, i, out int year);
                    if (sep1 != "/" || sep2 != "/")
                    {
                        return i0;
                    }
                    break;
                case "M":
                case "Y":
                    i = TextParser.ParseName(s, i, out string month2);
                    i = TextParser.ParseInteger(s, i, out int monthOrYear);
                    break;
                case "D":
                case "f":
                case "F":
                case "U":
                    i = TextParser.ParseName(s, i, out string dayName);
                    i = TextParser.ParseDelimiter(s, i, out string sep3, ",");
                    i = TextParser.ParseName(s, i, out string monthName2);
                    i = TextParser.ParseInteger(s, i, out int month3);
                    i = TextParser.ParseDelimiter(s, i, out string sep4, ",");
                    i = TextParser.ParseInteger(s, i, out int year2);
                    break;
                case "O":
                case "s":
                case "u":
                    i = TextParser.ParseInteger(s, i, out int year3);
                    i = TextParser.ParseDelimiter(s, i, out string sep5, "-");
                    i = TextParser.ParseInteger(s, i, out int month4);
                    i = TextParser.ParseDelimiter(s, i, out string sep6, "-");
                    i = TextParser.ParseInteger(s, i, out int day2);
                    break;
                case "R":
                    i = TextParser.ParseName(s, i, out string dayName2);
                    i = TextParser.ParseDelimiter(s, i, out string sep7, ",");
                    i = TextParser.ParseInteger(s, i, out int day3);
                    i = TextParser.ParseName(s, i, out string month5);
                    i = TextParser.ParseInteger(s, i, out int year4);
                    break;
            }

            return i;

        }

        private int ParseDouble(string s, int i0, out object d)
        {
            int i = TextParser.ParseReal(s, i0, out double dv);
            if (i > 0)
            {
                d = dv;
                return i;
            }
            else
            {
                d = null;
                return i0;
            }
        }

        private int ParseFloat(string s, int i0, out object d)
        {
            int i = TextParser.ParseReal(s, i0, out double rv);
            if (i > 0)
            {
                d = (float) rv;
                return i;
            }
            else
            {
                d = null;
                return i0;
            }
        }

        private int ParseInt(string s, int i0, out object d)
        {
            int i = i0;
            int iv = 0;
            if (StandardFormat == "d" || StandardFormat == "D" || StandardFormat == "")
            {
                i = TextParser.ParseInteger(s, i0, out iv);
            }
            else if (StandardFormat == "x" || StandardFormat == "X")
            {
                i = TextParser.ParseHex(s, i0, out iv);
            }
            if (i > 0)
            {
                d = iv;
                return i;
            }
            else
            {
                d = null;
                return i0;
            }
        }
    }
}
