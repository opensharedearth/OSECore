﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace OSECore.Text
{
    /// <summary>
    /// Test parser.  A lightweight set of parsers that scan input and recognize basic types including names, integers, reals, delimiters
    /// and operators.
    /// </summary>
    public static class TextParser
    {
        private enum Stage
        {
            WhiteSpace,
            Sign,
            Digits,
            DecimalPoint,
            Fraction,
            ExponentIndicator,
            ExponentSign,
            ExponentDigits,
            Name,
            Operator,
            GroupSeparator,
            LeadingSign,
            TrailingSign,
            PercentSign,
            CurrencySign,
            Number,
            InterstitialWhiteSpace
        };

        /// <summary>
        /// Parse an integer from the input.
        /// </summary>
        /// <param name="line">The input line.</param>
        /// <param name="i0">Starting zero-based index in the input line.</param>
        /// <param name="intValue">Integer value of the parsed string token or the default value if not integer was found.</param>
        /// <param name="defaultValue">Default value of integer to be used if integer not found.</param>
        /// <returns>An index to the start of the next string token or the starting index if no integer was found.</returns>
        public static int ParseInteger<T>(string line, int i0, out T intValue, T defaultValue = default(T))
        {
            Stage s = Stage.WhiteSpace;
            for (int i = i0; i < line.Length; ++i)
            {
                char c = line[i];
                switch (s)
                {
                    case Stage.WhiteSpace:
                        if (c == '-')
                            s = Stage.Sign;
                        else if (char.IsDigit(c))
                            s = Stage.Digits;
                        else if (!char.IsWhiteSpace(c))
                            return Bad(i0, out intValue, defaultValue);
                        break;
                    case Stage.Sign:
                        if (char.IsDigit(c))
                            s = Stage.Digits;
                        else
                        {
                            intValue = defaultValue;
                            return i0;
                        }

                        break;
                    case Stage.Digits:
                        if (!char.IsDigit(c))
                            return Good(line, i0, i, out intValue, defaultValue);
                        break;
                }
            }

            if (s == Stage.Digits)
                return Good(line, i0, line.Length, out intValue, defaultValue);
            else
                return Bad(i0, out intValue, defaultValue);
        }

        private static int Good<T>(string line, int i0, int i, out T iv, T dv, NumberStyles styles = NumberStyles.Any, NumberFormatInfo info = null)
        {
            if (TryParse(line.Substring(i0, i - i0), styles, info, out iv))
                return i;
            else
            {
                iv = dv;
                return i0;
            }
        }

        private static int Bad<T>(int i0, out T iv, T dv)
        {
            iv = dv;
            return i0;
        }

        /// <summary>   Parse real from the input.</summary>
        ///
        /// <param name="line"> The input line. </param>
        /// <param name="i0">   Starting zero-based index in the input line. </param>
        /// <param name="rv">The returned real value or the default value if a real value was not found.</param>
        /// <param name="dv">The default real value to be used if no real value found.</param>
        ///
        /// <returns>   The index to the next string token or the starting index if no real number was found. </returns>
        /// <remarks>Real numbers are reconized in any of the following forms:
        ///          1, 1., 1.0, 1E10, 1.0E-10.</remarks>
        public static int ParseReal<T>(string line, int i0, out T rv, T dv = default(T))
        {
            Stage s = Stage.WhiteSpace;
            for (int i = i0; i < line.Length; ++i)
            {
                char c = line[i];
                switch (s)
                {
                    case Stage.WhiteSpace:
                        if (c == '-')
                            s = Stage.Sign;
                        else if (char.IsDigit(c))
                            s = Stage.Digits;
                        else if (!char.IsWhiteSpace(c))
                            return Bad(i0, out rv, dv);
                        break;
                    case Stage.Sign:
                        if (char.IsDigit(c))
                            s = Stage.Digits;
                        else
                            return Bad(i0, out rv, dv);
                        break;
                    case Stage.Digits:
                        if (c == '.')
                            s = Stage.DecimalPoint;
                        else if (c == 'E' || c == 'e')
                            s = Stage.ExponentIndicator;
                        else if (!char.IsDigit(c))
                            return Good(line, i0, i, out rv, dv);
                        break;
                    case Stage.DecimalPoint:
                        if (c == 'E' || c == 'e')
                            s = Stage.ExponentIndicator;
                        else if (char.IsDigit(c))
                            s = Stage.Fraction;
                        else
                            return Good(line, i0, i, out rv, dv);
                        break;
                    case Stage.Fraction:
                        if (c == 'E' || c == 'e')
                            s = Stage.ExponentIndicator;
                        else if (!char.IsDigit(c))
                            return Good(line, i0, i, out rv, dv);
                        break;
                    case Stage.ExponentIndicator:
                        if (c == '-' || c == '+')
                            s = Stage.ExponentSign;
                        else if (char.IsDigit(c))
                            s = Stage.ExponentDigits;
                        else
                            return Bad(i0, out rv, dv);
                        break;
                    case Stage.ExponentSign:
                        if (char.IsDigit(c))
                            s = Stage.ExponentDigits;
                        else
                            return Bad(i0, out rv, dv);
                        break;
                    case Stage.ExponentDigits:
                        if (!char.IsDigit(c))
                            return Good(line, i0, i, out rv, dv);
                        break;
                }
            }

            if (s == Stage.Digits || s == Stage.Fraction || s == Stage.ExponentDigits || s == Stage.DecimalPoint)
                return Good(line, i0, line.Length, out rv, dv);
            return Bad(i0, out rv, dv);
        }

        public static int ParseNumber<T>(string line, int i0, out T v, T dv = default(T), NumberFormatInfo info = null)
        {
            if (info == null) info = NumberFormatInfo.CurrentInfo;
            int i1 = ParseWhiteSpace(line, i0);
            int i2 = ParseProtocol(line, i1, new Stage[] {Stage.Number}, out T v1 , info);
            if (i2 > i1)
            {
                v = v1;
                return i2;
            }
            Stage[] negativeProtocol;
            switch (info.NumberNegativePattern)
            {
                case 0:
                    negativeProtocol = new Stage[] {Stage.LeadingSign, Stage.Number, Stage.TrailingSign};
                    break;
                default:
                case 1:
                    negativeProtocol = new Stage[] { Stage.Sign, Stage.Number};
                    break;
                case 2:
                    negativeProtocol = new Stage[] { Stage.Number, Stage.InterstitialWhiteSpace, Stage.Sign };
                    break;
                case 3:
                    negativeProtocol = new Stage[] { Stage.Number, Stage.Sign };
                    break;
                case 4:
                    negativeProtocol = new Stage[] {Stage.Sign, Stage.InterstitialWhiteSpace, Stage.Number};
                    break;
            }

            int i3 = ParseProtocol(line, i1, negativeProtocol, out T v2, info);
            if (i3 > i1)
            {
                v = v2;
                return i3;
            }

            v = dv;
            return i0;
        }
        private static int ParsePositiveNumber<T>(string line, int i0, out T v, NumberFormatInfo info)
        {
            char groupSeparator = info.NumberGroupSeparator[0];
            char decimalPoint = info.NumberDecimalSeparator[0];
            Stage s = Stage.Digits;
            for (int i = i0; i < line.Length; ++i)
            {
                char c = line[i];
                switch (s)
                {
                    case Stage.Digits:
                        if (c == decimalPoint)
                            s = Stage.DecimalPoint;
                        else if (c == groupSeparator)
                            s = Stage.GroupSeparator;
                        else if (!char.IsDigit(c))
                            return Good(line, i0, i, out v, default(T), NumberStyles.Number, info);
                        break;
                    case Stage.DecimalPoint:
                        if (char.IsDigit(c))
                            s = Stage.Fraction;
                        else
                            return Good(line, i0, i, out v, default(T), NumberStyles.Number, info);
                        break;
                    case Stage.Fraction:
                        if (!char.IsDigit(c))
                            return Good(line, i0, i, out v, default(T), NumberStyles.Number, info);
                        break;
                    case Stage.GroupSeparator:
                        if (!char.IsDigit(c))
                            return Good(line, i0, i - 1, out v, default(T), NumberStyles.Number, info);
                        s = Stage.Digits;
                        break;
                }
            }

            if (s == Stage.Digits || s == Stage.Fraction)
                return Good(line, i0, line.Length, out v, default(T), NumberStyles.Number, info);
            return Bad(i0, out v, default(T));
        }

        public static int ParseWhiteSpace(string line, int i0)
        {
            for (int i = i0; i < line.Length; ++i)
            {
                if (!char.IsWhiteSpace(line[i])) return i;
            }

            return line.Length;
        }
        public static int ParseCurrency<T>(string line, int i0, out T v, T dv = default(T), NumberFormatInfo info = null)
        {
            int i1 = ParseWhiteSpace(line, i0);
            if(info == null)info = NumberFormatInfo.CurrentInfo;
            NumberFormatInfo info2 = (NumberFormatInfo)info.Clone();
            info2.NumberDecimalSeparator = info.CurrencyDecimalSeparator;
            info2.NumberGroupSeparator = info.CurrencyGroupSeparator;
            Stage[] positiveProtocol;
            switch (info.CurrencyPositivePattern)
            {
                default:
                case 0:
                    positiveProtocol = new Stage[] { Stage.CurrencySign, Stage.Number };
                    break;
                case 1:
                    positiveProtocol = new Stage[] { Stage.Number, Stage.CurrencySign };
                    break;
                case 2:
                    positiveProtocol = new Stage[] { Stage.CurrencySign, Stage.InterstitialWhiteSpace, Stage.Number };
                    break;
                case 3:
                    positiveProtocol = new Stage[] { Stage.Number, Stage.InterstitialWhiteSpace, Stage.CurrencySign };
                    break;
            }

            int i2 = ParseProtocol(line, i1, positiveProtocol, out T v0, info2);
            if (i2 > i1)
            {
                v = v0;
                return i2;
            }
            Stage[] negativeProtocol;

            switch (info.CurrencyNegativePattern)
            {
                default:
                case 0:
                    negativeProtocol = new Stage[] { Stage.LeadingSign, Stage.CurrencySign, Stage.Number, Stage.TrailingSign};
                    break;
                case 1:
                    negativeProtocol = new Stage[] {Stage.Sign, Stage.CurrencySign, Stage.Number};
                    break;
                case 2:
                    negativeProtocol = new Stage[] {Stage.CurrencySign, Stage.Sign, Stage.Number};
                    break;
                case 3:
                    negativeProtocol = new Stage[] {Stage.CurrencySign, Stage.Sign, Stage.Number};
                    break;
                case 4:
                    negativeProtocol = new Stage[] { Stage.LeadingSign, Stage.Number, Stage.CurrencySign, Stage.TrailingSign };
                    break;
                case 5:
                    negativeProtocol = new Stage[] {Stage.Sign, Stage.Number, Stage.CurrencySign};
                    break;
                case 6:
                    negativeProtocol = new Stage[] {Stage.CurrencySign, Stage.Sign, Stage.Number};
                    break;
                case 7:
                    negativeProtocol = new Stage[] {Stage.Number, Stage.CurrencySign, Stage.Sign};
                    break;
                case 8:
                    negativeProtocol = new Stage[] {Stage.Sign, Stage.Number, Stage.InterstitialWhiteSpace, Stage.CurrencySign};
                    break;
                case 9:
                    negativeProtocol = new Stage[] {Stage.Sign, Stage.CurrencySign, Stage.InterstitialWhiteSpace, Stage.Number};
                    break;
                case 10:
                    negativeProtocol = new Stage[] {Stage.Number, Stage.InterstitialWhiteSpace, Stage.CurrencySign, Stage.Sign};
                    break;
                case 11:
                    negativeProtocol = new Stage[] {Stage.CurrencySign, Stage.InterstitialWhiteSpace, Stage.Sign, Stage.Number};
                    break;
                case 12:
                    negativeProtocol = new Stage[] {Stage.CurrencySign, Stage.InterstitialWhiteSpace, Stage.Sign, Stage.Number};
                    break;
                case 13:
                    negativeProtocol = new Stage[] {Stage.Number, Stage.Sign, Stage.InterstitialWhiteSpace, Stage.CurrencySign};
                    break;
                case 14:
                    negativeProtocol = new Stage[] { Stage.LeadingSign, Stage.CurrencySign, Stage.InterstitialWhiteSpace, Stage.Number, Stage.TrailingSign };
                    break;
                case 15:
                    negativeProtocol = new Stage[] { Stage.LeadingSign, Stage.Number, Stage.InterstitialWhiteSpace, Stage.CurrencySign, Stage.TrailingSign };
                    break;
            }

            int i3 = ParseProtocol(line, i1, negativeProtocol, out T v1, info2);
            if (i3 > i1)
            {
                v = v1;
                return i3;
            }

            v = dv;
            return i0;
        }

        private static int ParseProtocol<T>(string line, int i0, Stage[] protocol, out T v, NumberFormatInfo info)
        {
            int i = i0;
            T v1 = default(T);
            if(info == null)info = NumberFormatInfo.CurrentInfo;
            Stack<Stage> protocolStack = new Stack<Stage>(protocol);
            bool negate = false;
            while (protocolStack.Count > 0)
            {
                Stage s = protocolStack.Pop();
                switch (s)
                {
                    case Stage.LeadingSign:
                        if (line.SafeIndex(i++) != '(') return Bad(i0, out v, default(T));
                        negate = true;
                        break;
                    case Stage.InterstitialWhiteSpace:
                        if (!char.IsWhiteSpace(line.SafeIndex(i++))) return Bad(i0, out v, default(T));
                        break;
                    case Stage.TrailingSign:
                        if (line.SafeIndex(i++) != ')') return Bad(i0, out v, default(T));
                        break;
                    case Stage.CurrencySign:
                        if (!line.Match(i, info.CurrencySymbol)) return Bad(i0, out v, default(T));
                        i += info.CurrencySymbol.Length;
                        break;
                    case Stage.Sign:
                        if (!line.Match(i, info.NegativeSign)) return Bad(i0, out v, default(T));
                        i += info.NegativeSign.Length;
                        negate = true;
                        break;
                    case Stage.Number:
                        int i2 = ParsePositiveNumber<T>(line, i, out v1, info);
                        if (i2 == i) return Bad(i0, out v, default(T));
                        i = i2;
                        break;
                }

            }

            v = negate ? Negate(v1) : v1;
            return i;
        }

        private static readonly Dictionary<Type, Delegate> s_negators = new Dictionary<Type, Delegate>();
        public static T Negate<T>(T d0)
        {
            Type t = typeof(T);
            if (s_negators.TryGetValue(t, out Delegate l0))
            {
                return ((Func<T, T>) l0)(d0);
            }
            else
            {
                Expression a = Expression.Parameter(typeof(T), "a");
                Expression n = Expression.Negate(a);
                Expression<Func<T, T>> negator = Expression.Lambda<Func<T, T>>(n);
                Func<T, T> l1 = negator.Compile();
                s_negators[t] = l1;
                return l1(d0);
            }
        }
        public static int ParsePercent<T>(string line, int i0, out T v, T dv = default(T))
        {
            int i1 = ParseWhiteSpace(line, i0);
            NumberFormatInfo info = NumberFormatInfo.CurrentInfo;
            Stage[] positiveProtocol;
            switch (info.CurrencyPositivePattern)
            {
                default:
                case 0:
                    positiveProtocol = new Stage[] { Stage.CurrencySign, Stage.Number };
                    break;
                case 1:
                    positiveProtocol = new Stage[] { Stage.Number, Stage.CurrencySign };
                    break;
                case 2:
                    positiveProtocol = new Stage[] { Stage.CurrencySign, Stage.InterstitialWhiteSpace, Stage.Number };
                    break;
                case 3:
                    positiveProtocol = new Stage[] { Stage.Number, Stage.InterstitialWhiteSpace, Stage.CurrencySign };
                    break;
            }

            NumberFormatInfo info2 = (NumberFormatInfo)info.Clone();
            int i2 = ParseProtocol(line, i1, positiveProtocol, out T v1, info2);
            if (i2 > i1)
            {
                v = v1;
                return i2;
            }
            Stage[] negativeProtocol;

            switch (info.CurrencyNegativePattern)
            {
                default:
                case 0:
                    negativeProtocol = new Stage[] { Stage.LeadingSign, Stage.CurrencySign, Stage.Number, Stage.TrailingSign };
                    break;
                case 1:
                    negativeProtocol = new Stage[] { Stage.Sign, Stage.CurrencySign, Stage.Number };
                    break;
                case 2:
                    negativeProtocol = new Stage[] { Stage.CurrencySign, Stage.Sign, Stage.Number };
                    break;
                case 3:
                    negativeProtocol = new Stage[] { Stage.CurrencySign, Stage.Sign, Stage.Number };
                    break;
                case 4:
                    negativeProtocol = new Stage[] { Stage.LeadingSign, Stage.Number, Stage.CurrencySign, Stage.TrailingSign };
                    break;
                case 5:
                    negativeProtocol = new Stage[] { Stage.Sign, Stage.Number, Stage.CurrencySign };
                    break;
                case 6:
                    negativeProtocol = new Stage[] { Stage.CurrencySign, Stage.Sign, Stage.Number };
                    break;
                case 7:
                    negativeProtocol = new Stage[] { Stage.Number, Stage.CurrencySign, Stage.Sign };
                    break;
                case 8:
                    negativeProtocol = new Stage[] { Stage.Sign, Stage.Number, Stage.InterstitialWhiteSpace, Stage.CurrencySign };
                    break;
                case 9:
                    negativeProtocol = new Stage[] { Stage.Sign, Stage.CurrencySign, Stage.InterstitialWhiteSpace, Stage.Number };
                    break;
                case 10:
                    negativeProtocol = new Stage[] { Stage.Number, Stage.InterstitialWhiteSpace, Stage.CurrencySign, Stage.Sign };
                    break;
                case 11:
                    negativeProtocol = new Stage[] { Stage.CurrencySign, Stage.InterstitialWhiteSpace, Stage.Sign, Stage.Number };
                    break;
                case 12:
                    negativeProtocol = new Stage[] { Stage.CurrencySign, Stage.InterstitialWhiteSpace, Stage.Sign, Stage.Number };
                    break;
                case 13:
                    negativeProtocol = new Stage[] { Stage.Number, Stage.Sign, Stage.InterstitialWhiteSpace, Stage.CurrencySign };
                    break;
                case 14:
                    negativeProtocol = new Stage[] { Stage.LeadingSign, Stage.CurrencySign, Stage.InterstitialWhiteSpace, Stage.Number, Stage.TrailingSign };
                    break;
                case 15:
                    negativeProtocol = new Stage[] { Stage.LeadingSign, Stage.Number, Stage.InterstitialWhiteSpace, Stage.CurrencySign, Stage.TrailingSign };
                    break;
            }
            int i3 = ParseProtocol(line, i1, negativeProtocol, out T v2, info2);
            if (i3 > i1)
            {
                v = v2;
                return i3;
            }

            v = dv;
            return i0;
        }

        /// <summary>   Parse name from the input. </summary>
        ///
        /// <param name="line">             The input line. </param>
        /// <param name="i0">               Starting zero-based index in the input line. </param>
        /// <param name="name">The name found in the input or empty string if no name was found.</param>
        /// <param name="firstCharacters">  (Optional) The first characters allowed in the name.  Letters are assumed. </param>
        /// <param name="nextCharacters">   (Optional) The characters following the first that are allowed.  Letters and digits are assumed. </param>
        ///
        /// <returns>   The index to the token following the name if name was found or else the starting index if no name was found. </returns>
        public static int ParseName(string line, int i0, out string name, string firstCharacters = "_",
        string nextCharacters = "_")
        {
            Stage s = Stage.WhiteSpace;
            for (int i = i0; i < line.Length; i++)
            {
                char c = line[i];
                switch (s)
                {
                    case Stage.WhiteSpace:
                        if (!char.IsWhiteSpace(c))
                        {
                            if (char.IsLetter(c) || firstCharacters.IndexOf(c) >= 0)
                                s = Stage.Name;
                            else
                            {
                                name = "";
                                return i0;
                            }
                        }

                        break;
                    case Stage.Name:
                        if (!char.IsLetterOrDigit(c) && nextCharacters.IndexOf(c) < 0)
                        {
                            name = line.Substring(i0, i - i0).Trim();
                            return i;
                        }

                        break;
                }

            }

            name = line.Substring(i0).Trim();
            return line.Length;
        }

        /// <summary>   Parse delimiter from the input. </summary>
        ///
        /// <param name="line">         The input line. </param>
        /// <param name="i0">           Starting zero-based index in the input line. </param>
        /// <param name="delimiter">The delimiter found in the input or empty string if no delimiter found.</param>
        /// <param name="delimiters">   (Optional) The delimiters to be recognized in the input.  By default comma and semi-colon. </param>
        ///
        /// <returns>   Index of string following delimiter or string index if no delimiter was found.</returns>
        public static int ParseDelimiter(string line, int i0, out string delimiter, string delimiters = ",;")
        {
            for (int i = i0; i < line.Length; ++i)
            {
                char c = line[i];
                if (!char.IsWhiteSpace(c))
                {
                    if (delimiters.IndexOf(c) >= 0)
                    {
                        delimiter = line.Substring(i0, i - i0 + 1).Trim();
                        return i + 1;
                    }
                    else
                        break;
                }
            }

            delimiter = "";
            return i0;
        }

        /// <summary>   Parse operator from the input. </summary>
        ///
        /// <param name="line">         The input line. </param>
        /// <param name="i0">           Starting zero-based index in the input line. </param>
        /// <param name="op">The operation that was found in the input.</param>
        /// <param name="operators">    The array of valid operator strings.  Each operator might be any length but are typically one or two characters. </param>
        ///
        /// <returns>   Index of the token following the operator if an operator was found or the starting index if operator was not found.</returns>
        /// <remarks>The longest possible operator string is returned.  Therefore if the list of operators includes
        ///          both "+" and "++", "++" will be returned if found.</remarks>
        public static int ParseOperator(string line, int i0, out string op, string[] operators)
        {
            string opchars = String.Join("", operators);
            int maxop = operators.Max(l => l.Length);
            string t = "";
            string co = "";
            Stage s = Stage.WhiteSpace;
            for (int i = i0; i < line.Length; ++i)
            {
                char c = line[i];
                switch (s)
                {
                    case Stage.WhiteSpace:
                        if (!char.IsWhiteSpace(c))
                        {
                            if (opchars.IndexOf(c) >= 0)
                            {
                                s = Stage.Operator;
                                t = new string(c, 1);
                                if (operators.Contains(t))
                                {
                                    if (maxop == 1) return Good(line, i0, i + 1, out op);
                                    co = t;
                                }
                            }
                            else
                                return Bad(i0, out op);
                        }

                        break;
                    case Stage.Operator:
                        if (opchars.IndexOf(c) >= 0)
                        {
                            t += c;
                            if (operators.Contains(t))
                            {
                                if (maxop == t.Length) return Good(line, i0, i + 1, out op);
                                co = t;
                            }
                            else if (maxop == t.Length)
                            {
                                return co.Length > 0 ? Good(line, i0, i, out op) : Bad(i0, out op);
                            }
                        }
                        else if (co.Length > 0)
                        {
                            return Good(line, i0, i - t.Length + co.Length, out op);
                        }

                        break;
                }

            }

            return co.Length > 0 ? Good(line, i0, line.Length, out op) : Bad(i0, out op);
        }

        private static int Good(string line, int i0, int i, out string op)
        {
            op = line.Substring(i0, i - i0).Trim();
            return i;
        }

        private static int Bad(int i0, out string op)
        {
            op = "";
            return i0;
        }

        public static int ParseCharacter(string line, int i0, out char c, char dv = '\0')
        {
            int i = i0;
            if (!String.IsNullOrEmpty(line))
            {
                char c0 = line[i++];
                if (c0 != '\\')
                {
                    c = c0;
                    return i;
                }

                if (i < line.Length)
                {
                    char c1 = line[i++];
                    if (c1 == 'u')
                    {
                        i += 4;
                    }
                    else if (c1 == 'x')
                    {
                        int j0 = i;
                        for (int j = j0; j < j0 + 4 && j < line.Length; j++)
                        {
                            if ("0123456789abcdefABCDEF".IndexOf(line[i]) < 0) break;
                            i++;
                        }
                    }

                    c = TextSupport.GetEscapedCharacter(line.Substring(i0, i - i0));
                    return i;
                }
            }

            c = '\0';
            return i0;
        }

        public static int ParseHex<T>(string line, int i0, out T d) where T : struct
        {
            Stage s = Stage.WhiteSpace;
            int digits = Marshal.SizeOf(typeof(T)) * 2;
            if (!String.IsNullOrEmpty(line))
            {
                for (int i = i0; i < line.Length; ++i)
                {
                    char c = line[i];
                    switch (c)
                    {
                        case ' ':
                        case '\t':
                            break;
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                        case 'a':
                        case 'A':
                        case 'b':
                        case 'B':
                        case 'c':
                        case 'C':
                        case 'd':
                        case 'D':
                        case 'e':
                        case 'E':
                        case 'f':
                        case 'F':
                            s = Stage.Digits;
                            if (digits-- == 0)
                            {
                                return TryParseHex(line.Substring(i0, i - i0), out d) ? i : i0;
                            }
                            break;
                        default:
                            if (s == Stage.Digits)
                            {
                                return TryParseHex(line.Substring(i0, i - i0), out d) ? i : i0;
                            }
                            d = default(T);
                            return i0;
                    }
                }
            }
            return TryParseHex(line.Substring(i0), out d) ? line.Length : i0;
        }
        private static bool TryParseHex<T>(string hex, out T val)
        {
            Type t = typeof(T);
            MethodInfo mi = t.GetMethod("TryParse", new Type[] { typeof(string), typeof(NumberStyles), typeof(IFormatProvider), typeof(T).MakeByRefType() });
            if (mi != null)
            {
                object[] parameters = new object[] { hex, NumberStyles.AllowHexSpecifier, null, null };
                if((bool) mi.Invoke(null, parameters))
                {
                    val = (T)parameters[3];
                    return true;
                }
            }

            val = default(T);
            return false;
        }
        private static bool TryParse<T>(string input, out T val)
        {
            Type t = typeof(T);
            MethodInfo mi = t.GetMethod("TryParse", new Type[] { typeof(string), typeof(T).MakeByRefType() });
            if (mi != null)
            {
                object[] parameters = new object[] { input, null };
                if((bool) mi.Invoke(null, parameters))
                {
                    val = (T)parameters[1];
                    return true;
                }
            }

            val = default(T);
            return false;
        }
        private static bool TryParse<T>(string input, NumberStyles styles, IFormatProvider fp, out T val)
        {
            Type t = typeof(T);
            MethodInfo mi = t.GetMethod("TryParse", new Type[] { typeof(string), typeof(NumberStyles), typeof(IFormatProvider), typeof(T).MakeByRefType() });
            if (mi != null)
            {
                object[] parameters = new object[] { input, styles, fp, null };
                if ((bool)mi.Invoke(null, parameters))
                {
                    val = (T)parameters[3];
                    return true;
                }
            }

            val = default(T);
            return false;
        }

    }
}
