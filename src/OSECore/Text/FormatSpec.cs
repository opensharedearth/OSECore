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
        private static Dictionary<string, Func<string, int, DateTimeFormatInfo, (int, DateTimeArgs)>> _dateTimeFormats = null;
        private int _index = 0;
        private int _alignment = 0;
        private string _format = "";
        private string _delimiter = "";
        private bool _isValid = false;
        private static void InitializeDateTimeFormats()
        {
            if (_dateTimeFormats == null)
            {
                _dateTimeFormats = new Dictionary<string, Func<string, int, DateTimeFormatInfo, (int, DateTimeArgs)>>();
                _dateTimeFormats["d"] = (string line, int i0, DateTimeFormatInfo dtfi) =>
                 {
                     int i1 = TextParser.ParseInteger<int>(line, i0, out int iv);
                     return (i1, new DateTimeArgs() { Day = iv });
                 };
                _dateTimeFormats["dd"] = _dateTimeFormats["d"];
                _dateTimeFormats["ddd"] = (string line, int i0, DateTimeFormatInfo dtfi) =>
                {
                    int i1 = TextParser.ParseName(line, i0, out string name, TextParser.NameChars, TextParser.NameChars);
                    return (i1, new DateTimeArgs());
                };
                _dateTimeFormats["dddd"] = _dateTimeFormats["ddd"];
                _dateTimeFormats["f"] = (string line, int i0, DateTimeFormatInfo dtfi) =>
                {
                    int i1 = TextParser.ParseInteger(line, i0, out int iv);
                    return (i1, new DateTimeArgs() { FracSecond = iv * 1000000 });
                };
                _dateTimeFormats["ff"] = (string line, int i0, DateTimeFormatInfo dtfi) =>
                {
                    int i1 = TextParser.ParseInteger(line, i0, out int iv);
                    return (i1, new DateTimeArgs() { FracSecond = iv * 100000 });
                };
                _dateTimeFormats["fff"] = (string line, int i0, DateTimeFormatInfo dtfi) =>
                {
                    int i1 = TextParser.ParseInteger(line, i0, out int iv);
                    return (i1, new DateTimeArgs() { FracSecond = iv * 10000});
                };
                _dateTimeFormats["ffff"] = (string line, int i0, DateTimeFormatInfo dtfi) =>
                {
                    int i1 = TextParser.ParseInteger(line, i0, out int iv);
                    return (i1, new DateTimeArgs() { FracSecond = iv * 1000 });
                };
                _dateTimeFormats["fffff"] = (string line, int i0, DateTimeFormatInfo dtfi) =>
                {
                    int i1 = TextParser.ParseInteger(line, i0, out int iv);
                    return (i1, new DateTimeArgs() { FracSecond = iv * 100 });
                };
                _dateTimeFormats["ffffff"] = (string line, int i0, DateTimeFormatInfo dtfi) =>
                {
                    int i1 = TextParser.ParseInteger(line, i0, out int iv);
                    return (i1, new DateTimeArgs() { FracSecond = iv * 10 });
                };
                _dateTimeFormats["fffffff"] = (string line, int i0, DateTimeFormatInfo dtfi) =>
                {
                    int i1 = TextParser.ParseInteger(line, i0, out int iv);
                    return (i1, new DateTimeArgs() { FracSecond = iv });
                };
                _dateTimeFormats["F"] = _dateTimeFormats["f"];
                _dateTimeFormats["FF"] = _dateTimeFormats["ff"];
                _dateTimeFormats["FFF"] = _dateTimeFormats["fff"];
                _dateTimeFormats["FFFF"] = _dateTimeFormats["ffff"];
                _dateTimeFormats["FFFFF"] = _dateTimeFormats["fffff"];
                _dateTimeFormats["FFFFFF"] = _dateTimeFormats["ffffff"];
                _dateTimeFormats["FFFFFFF"] = _dateTimeFormats["fffffff"];
                _dateTimeFormats["g"] = (string line, int i0, DateTimeFormatInfo dtfi) =>
                {
                    throw new NotSupportedException("The 'g' custom date/time format is not supported");
                };
                _dateTimeFormats["gg"] = (string line, int i0, DateTimeFormatInfo dtfi) =>
                {
                    throw new NotSupportedException("The 'gg' custom date/time format is not supported");
                };
                _dateTimeFormats["h"] = (string line, int i0, DateTimeFormatInfo dtfi) =>
                {
                    int i1 = TextParser.ParseInteger(line, i0, out int iv);
                    return (i1, new DateTimeArgs() { Hour = iv });
                };
                _dateTimeFormats["hh"] = _dateTimeFormats["h"];
                _dateTimeFormats["H"] = _dateTimeFormats["h"];
                _dateTimeFormats["HH"] = _dateTimeFormats["h"];
                _dateTimeFormats["K"] = (string line, int i0, DateTimeFormatInfo dtfi) =>
                {
                    throw new NotSupportedException("The 'K' custom date/time format is not supported");
                };
                _dateTimeFormats["m"] = (string line, int i0, DateTimeFormatInfo dtfi) =>
                {
                    int i1 = TextParser.ParseInteger(line, i0, out int iv);
                    return (i1, new DateTimeArgs() { Minute = iv });
                };
                _dateTimeFormats["mm"] = _dateTimeFormats["m"];
                _dateTimeFormats["M"] = (string line, int i0, DateTimeFormatInfo dtfi) =>
                {
                    int i1 = TextParser.ParseInteger(line, i0, out int iv);
                    return (i1, new DateTimeArgs() { Month = iv });
                };
                _dateTimeFormats["MM"] = _dateTimeFormats["M"];
                _dateTimeFormats["MMM"] = (string line, int i0, DateTimeFormatInfo dtfi) =>
                {
                    int i1 = TextParser.ParseName(line, i0, out string name, TextParser.NameChars, TextParser.NameChars);
                    if (i1 > i0)
                    {
                        int iv = Array.FindIndex(dtfi.AbbreviatedMonthNames, x => String.Compare(x, name, true) == 0);
                        if (iv >= 0)
                        {
                            return (i1, new DateTimeArgs() { Month = iv + 1 });
                        }
                    }
                    return (i0, new DateTimeArgs());
                };
                _dateTimeFormats["MMMM"] = (string line, int i0, DateTimeFormatInfo dtfi) =>
                {
                    int i1 = TextParser.ParseName(line, i0, out string name, TextParser.NameChars, TextParser.NameChars);
                    if (i1 > i0)
                    {
                        int iv = Array.FindIndex(dtfi.MonthNames, x => String.Compare(x, name, true) == 0);
                        if (iv >= 0)
                        {
                            return (i1, new DateTimeArgs() { Month = iv + 1 });
                        }
                    }
                    return (i0, new DateTimeArgs());
                };
                _dateTimeFormats["s"] = (string line, int i0, DateTimeFormatInfo dtfi) =>
                {
                    int i1 = TextParser.ParseInteger(line, i0, out int iv);
                    return (i1, new DateTimeArgs() { Second = iv });
                };
                _dateTimeFormats["ss"] = _dateTimeFormats["s"];
                _dateTimeFormats["S"] = _dateTimeFormats["s"];
                _dateTimeFormats["SS"] = _dateTimeFormats["s"];
                _dateTimeFormats["t"] = (string line, int i0, DateTimeFormatInfo dtfi) =>
                {
                    int i1 = TextParser.ParseCharacter(line, i0, out char c);
                    switch (c)
                    {
                        case 'a':
                        case 'A':
                            return (i1, new DateTimeArgs() { AMPM = DateTimeArgs.AMPMSpec.AM });
                        case 'p':
                        case 'P':
                            return (i1, new DateTimeArgs() { AMPM = DateTimeArgs.AMPMSpec.PM });
                        default:
                            return (i0, new DateTimeArgs());
                    }
                };
                _dateTimeFormats["tt"] = (string line, int i0, DateTimeFormatInfo dtfi) =>
                {
                    int i1 = TextParser.ParseCharacters(line, i0, 2, out string s);
                    switch (s)
                    {
                        case "am":
                        case "AM":
                            return (i1, new DateTimeArgs() { AMPM = DateTimeArgs.AMPMSpec.AM });
                        case "pm":
                        case "PM":
                            return (i1, new DateTimeArgs() { AMPM = DateTimeArgs.AMPMSpec.PM });
                        default:
                            return (i0, new DateTimeArgs());
                    }
                };
                _dateTimeFormats["y"] = (string line, int i0, DateTimeFormatInfo dtfi) =>
                {
                    int i1 = TextParser.ParseInteger(line, i0, out int iv);
                    return (i1, new DateTimeArgs() { Year = iv > 50 ? 1900 + iv : 2000 + iv });
                };
                _dateTimeFormats["yy"] = _dateTimeFormats["y"];
                _dateTimeFormats["yyy"] = (string line, int i0, DateTimeFormatInfo dtfi) =>
                {
                    int i1 = TextParser.ParseInteger(line, i0, out int iv);
                    return (i1, new DateTimeArgs() { Year = iv });
                };
                _dateTimeFormats["yyyy"] = _dateTimeFormats["yyy"];
                _dateTimeFormats["yyyyy"] = _dateTimeFormats["yyy"];
                _dateTimeFormats["z"] = (string line, int i0, DateTimeFormatInfo dtfi) =>
                {
                    throw new NotSupportedException("The 'z' custom date/time format is not supported");
                };
                _dateTimeFormats["zz"] = (string line, int i0, DateTimeFormatInfo dtfi) =>
                {
                    throw new NotSupportedException("The 'zz' custom date/time format is not supported");
                };
                _dateTimeFormats["zzz"] = (string line, int i0, DateTimeFormatInfo dtfi) =>
                {
                    throw new NotSupportedException("The 'zzz' custom date/time format is not supported");
                };
            }
        }
        /// <summary>   Constructor taking line start and length. </summary>
        ///
        /// <param name="line">     The line containing the format specfier. </param>
        /// <param name="start">    The zero-based starting index of the specifier. </param>
        /// <param name="length">   The length of the specifier. </param>
        public FormatSpec(string line, int start, int length)
            : base(line, start, length)
        {
            if (!base.IsNull && Length >= 3 && line[Start] == '{' && line[End] == '}')
            {
                int i0 = start + 1;
                i0 = TextParser.ParseInteger(line, i0, out _index, 0);
                i0 = TextParser.ParseDelimiter(line, i0, out string delimiter, ",");
                if (delimiter == ",") i0 = TextParser.ParseInteger(line, i0, out _alignment, 0);
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

        public FormatSpec(string line, FormatSpec fs, FormatLiteral fl)
        : base(line, fs.Start, fs.Length)
        {
            _index = fs.Index;
            _alignment = fs.Alignment;
            _format = fs.Format;
            _delimiter = fl.Literal;
            Validate();
        }

        private void Validate()
        {
            _isValid = Index >= 0 && !IsNull && Index < Length;
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
        public bool IsValid => !base.IsNull && _isValid;

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
                    (int i1, DateTime d1) = ParseDateTime(s, i0, CultureInfo.CurrentCulture.DateTimeFormat);
                    d = d1;
                    return i1;
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
            bool negative = false;
            int i = i0;
            i = TextParser.ParseInteger(s, i, out int hours);
            if(hours < 0)
            {
                negative = true;
                hours = -hours;
            }
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
                var ts = new TimeSpan(days, hours, minutes, seconds, fractions);
                if(negative)
                {
                    ts = -ts;
                }
                d = ts;
                return i;
            }

            d = null;
            return i0;
        }

        private (int, DateTime) ParseDateTime(string s, int i0, DateTimeFormatInfo dtfi)
        {
            InitializeDateTimeFormats();
            int i = i0;
            int j = i0;
            int k = i0;
            DateTimeArgs dta = new DateTimeArgs();
            switch (StandardFormat)
            {
                case "":
                    {
                        (int i1, DateTimeArgs dta1) = ParseDateTime(s, i0, dtfi, dtfi.ShortDatePattern);
                        dta.Union(dta1);
                        (int i2, DateTimeArgs dta2) = ParseDateTime(s, i1, dtfi, dtfi.LongTimePattern);
                        dta.Union(dta2);
                        if (i1 == i0 || i2 == i1) return (i0, DateTime.MinValue);
                        i = i2;
                    }
                    break;
                case "f":
                    {
                        (int i1, DateTimeArgs dta1) = ParseDateTime(s, i0, dtfi, dtfi.LongDatePattern);
                        dta.Union(dta1);
                        (int i2, DateTimeArgs dta2) = ParseDateTime(s, i1, dtfi, dtfi.ShortTimePattern);
                        dta.Union(dta2);
                        if (i1 == i0 || i2 == i1) return (i0, DateTime.MinValue);
                        i = i2;
                    }
                    break;
                case "F":
                    {
                        (int i1, DateTimeArgs dta1) = ParseDateTime(s, i0, dtfi, dtfi.LongDatePattern);
                        dta.Union(dta1);
                        (int i2, DateTimeArgs dta2) = ParseDateTime(s, i1, dtfi, dtfi.LongTimePattern);
                        dta.Union(dta2);
                        if (i1 == i0 || i2 == i1) return (i0, DateTime.MinValue);
                        i = i2;
                    }
                    break;
                case "g":
                    {
                        (int i1, DateTimeArgs dta1) = ParseDateTime(s, i0, dtfi, dtfi.ShortDatePattern);
                        dta.Union(dta1);
                        (int i2, DateTimeArgs dta2) = ParseDateTime(s, i1, dtfi, dtfi.ShortTimePattern);
                        dta.Union(dta2);
                        if (i1 == i0 || i2 == i1) return (i0, DateTime.MinValue);
                        i = i2;
                    }
                    break;
                case "G":
                    {
                        (int i1, DateTimeArgs dta1) = ParseDateTime(s, i0, dtfi, dtfi.ShortDatePattern);
                        dta.Union(dta1);
                        (int i2, DateTimeArgs dta2) = ParseDateTime(s, i1, dtfi, dtfi.LongTimePattern);
                        dta.Union(dta2);
                        if (i1 == i0 || i2 == i1) return (i0, DateTime.MinValue);
                        i = i2;
                    }
                    break;
                case "R":
                    {
                        (int i1, DateTimeArgs dta1) = ParseDateTime(s, i0, dtfi, dtfi.RFC1123Pattern);
                        dta.Union(dta1);
                        dta.TimeZone = DateTimeArgs.TimeZoneSpec.Zulu;
                        if (i1 == i0) return (i0, DateTime.MinValue);
                        i = i1;
                    }
                    break;
                case "u":
                    {
                        (int i1, DateTimeArgs dta1) = ParseDateTime(s, i0, dtfi, dtfi.UniversalSortableDateTimePattern);
                        dta.Union(dta1);
                        if (i1 == i0) return (i0, DateTime.MinValue);
                        i = i1;
                    }
                    break;
                case "U":
                    {
                        (int i1, DateTimeArgs dta1) = ParseDateTime(s, i0, dtfi, dtfi.LongDatePattern);
                        dta.Union(dta1);
                        if (i1 == i0) return (i0, DateTime.MinValue);
                        (int i2, DateTimeArgs dta2) = ParseDateTime(s, i1, dtfi, dtfi.LongTimePattern);
                        dta.Union(dta2);
                        if (i2 == i1) return (i0, DateTime.MinValue);
                        i = i2;
                    }
                    break;
                case "O":
                    {
                        (int i1, DateTimeArgs dta1) = ParseRFC3339DateTime(s, i0, dtfi);
                        dta.Union(dta1);
                        if (i1 == i0) return (i0, DateTime.MinValue);
                        i = i1;
                    }
                    break;
                case "s":
                    {
                        (int i1, DateTimeArgs dta1) = ParseDateTime(s, i0, dtfi, dtfi.SortableDateTimePattern);
                        dta.Union(dta1);
                        if (i1 == i0) return (i0, DateTime.MinValue);
                        i = i1;
                    }
                    break;
                case "D":
                    {
                        (int i1, DateTimeArgs dta1) = ParseDateTime(s, i0, dtfi, dtfi.LongDatePattern);
                        dta.Union(dta1);
                        if (i1 == i0) return (i0, DateTime.MinValue);
                        i = i1;
                    }
                    break;
                case "d":
                    {
                        (int i1, DateTimeArgs dta1) = ParseDateTime(s, i0, dtfi, dtfi.ShortDatePattern);
                        dta.Union(dta1);
                        if (i1 == i0) return (i0, DateTime.MinValue);
                        i = i1;
                    }
                    break;
                case "M":
                    {
                        (int i1, DateTimeArgs dta1) = ParseDateTime(s, i0, dtfi, dtfi.MonthDayPattern);
                        dta.Union(dta1);
                        if (i1 == i0) return (i0, DateTime.MinValue);
                        i = i1;
                    }
                    break;
                case "Y":
                    {
                        (int i1, DateTimeArgs dta1) = ParseDateTime(s, i0, dtfi, dtfi.YearMonthPattern);
                        dta.Union(dta1);
                        if (i1 == i0) return (i0, DateTime.MinValue);
                        i = i1;
                    }
                    break;
                case "t":
                    {
                        (int i1, DateTimeArgs dta1) = ParseDateTime(s, i0, dtfi, dtfi.ShortTimePattern);
                        dta.Union(dta1);
                        if (i1 == i0) return (i0, DateTime.MinValue);
                        i = i1;
                    }
                    break;
                case "T":
                    {
                        (int i1, DateTimeArgs dta1) = ParseDateTime(s, i0, dtfi, dtfi.LongTimePattern);
                        dta.Union(dta1);
                        if (i1 == i0) return (i0, DateTime.MinValue);
                        i = i1;
                    }
                    break;
                default:
                    return (i0, DateTime.MinValue);
            }
            return (i, DateTimeArgs.CreateDateTime(DateTimeArgs.Normalize(dta)));
        }

        private static (int, DateTimeArgs) ParseRFC3339DateTime(string s, int i0, DateTimeFormatInfo dtfi)
        {
            DateTimeArgs dta = new DateTimeArgs();
            (int i1, DateTimeArgs dta1) = ParseDateTime(s, i0, dtfi, "yyyy-MM-dd'T'HH:mm:ss");
            if (i1 == i0) return (i0, dta);
            dta.Union(dta1);
            (int i2, DateTimeArgs dta2) = ParseFracSecond(s, i1);
            if (i2 == i1) return (i1, dta);
            dta.Union(dta2);
            (int i3, DateTimeArgs dta3) = ParseTimeZone(s, i2);
            dta.Union(dta3);
            return (i3, dta);
        }
        private static (int, DateTimeArgs) ParseTimeZone(string s, int i0)
        {
            DateTimeArgs dta = new DateTimeArgs();
            int i1 = TextParser.ParseCharacter(s, i0, out char c);
            if (i1 == i0) return (i0, dta);
            switch (c)
            {
                case 'Z': dta.TimeZone = DateTimeArgs.TimeZoneSpec.Zulu; return (i1, dta);
                case '+': dta.TimeZone = DateTimeArgs.TimeZoneSpec.Ahead; break;
                case '-': dta.TimeZone = DateTimeArgs.TimeZoneSpec.Behind; break;
                default: return (i0, dta);
            }
            int i2 = TextParser.ParseInteger(s, i1, out int tzh);
            if (i2 == i1) return (i1, dta);
            dta.ZoneHour = tzh;
            int i3 = TextParser.ParseCharacter(s, i2, out char d);
            if (i3 > i2 && d == ':')
            {
                int i4 = TextParser.ParseInteger(s, i3, out int tzm);
                if (i4 > i3)
                {
                    dta.ZoneMinute = tzm;
                    return (i4, dta);
                }
            }
            return (i2, dta);
        }

        private static (int, DateTimeArgs) ParseFracSecond(string s, int i0)
        {
            DateTimeArgs dta = new DateTimeArgs();
            int i1 = TextParser.ParseCharacter(s, i0, out char dot);
            if (i1 > i0 && dot == '.')
            {
                int i2 = TextParser.ParseInteger(s, i1, out int fracsec);
                if (i2 > i1 && i2 - i1 < 8)
                {
                    dta.FracSecond = fracsec * (int)Math.Pow(10, 7 - i2 + i1);
                    return (i2, dta);
                }
            }
            return (i0, dta);
        }

        private static (int, DateTimeArgs) ParseDateTime(string s, int i0, DateTimeFormatInfo dtfo, string pattern)
        {
            int j = 0;
            int i = i0;
            DateTimeArgs dta = new DateTimeArgs();
            while (j < pattern.Length)
            {
                LineToken t = TextParser.Parse(pattern, j);
                switch (t.Type)
                {
                    case LineToken.TokenType.Name:
                        {
                            if (_dateTimeFormats.TryGetValue(t.Value as string, out Func<string, int, DateTimeFormatInfo, (int, DateTimeArgs)> df))
                            {
                                (int i1, DateTimeArgs dta1) = df(s, i, dtfo);
                                if (i1 > i)
                                {
                                    dta.Union(dta1);
                                    i = i1;
                                }
                                else
                                {
                                    return (i0, new DateTimeArgs());
                                }
                            }
                        }
                        break;
                    case LineToken.TokenType.QuotedString:
                        {
                            string ls = t.Value as string;
                            int i1 = TextParser.ParseCharacters(s, i, ls.Length, out string ls1);
                            if (i1 > i && ls == ls1)
                            {
                                i = i1;
                            }
                            else
                            {
                                return (i0, new DateTimeArgs());
                            }
                        }
                        break;
                    case LineToken.TokenType.Delimiter:
                        {
                            char c = (char)t.Value;
                            int i1 = TextParser.ParseCharacter(s, i, out char c1);
                            if (c == c1)
                            {
                                i = i1;
                            }
                            else
                            {
                                return (i0, new DateTimeArgs());
                            }
                        }
                        break;
                    case LineToken.TokenType.WhiteSpace:
                        {
                            int i1 = TextParser.ParseWhiteSpace(s, i);
                            if (i1 == i)return (i0, new DateTimeArgs());
                            i = i1;
                        }
                        break;
                    default:
                        return (i0, new DateTimeArgs());
                }
                j = t.Next;
            }
            return (i, dta);
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
                d = (float)rv;
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
