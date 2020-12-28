using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSECore.Text
{
    /// <summary>
    /// Various utilities for handling Unicode strings and characters.
    /// </summary>
    public static class UnicodeSupport
    {
        /// <summary>
        /// Get normal char.  For special Unicode superscript or subscript characters, returns the common equivalent.
        /// </summary>
        /// <param name="c">Unicode superscript or subscript character</param>
        /// <returns>Normal Unicode character</returns>
        /// <remarks>If character is neither a superscript or a subscript character, the character is simply returned.</remarks>
        public static char GetNormalChar(char c)
        {
            switch (c)
            {
                case '\x2212':
                case '\x207b':
                case '\x208b':
                    return '-';
                case '\x207A':
                case '\x208a':
                    return '+';
                case '\x22c5':
                    return '*';
                case '\x2215':
                    return '/';
                case '\x2070':
                case '\x2080':
                    return '0';
                case '\x00b9':
                case '\x2081':
                    return '1';
                case '\x00b2':
                case '\x2082':
                    return '2';
                case '\x00b3':
                case '\x2083':
                    return '3';
                case '\x2074':
                case '\x2084':
                    return '4';
                case '\x2075':
                case '\x2085':
                    return '5';
                case '\x2076':
                case '\x2086':
                    return '6';
                case '\x2077':
                case '\x2087':
                    return '7';
                case '\x2078':
                case '\x2088':
                    return '8';
                case '\x2079':
                case '\x2089':
                    return '9';
                default:
                    return c;
            }
        }
        /// <summary>
        /// Get a string that has special Unicode superscript and subscript characters converted to normal characters.
        /// </summary>
        /// <param name="input">The input string to be converted</param>
        /// <returns>Input string with Unicode superscripts and subscripts converted to normal characters.</returns>
        /// <remarks>If the input string contains no superscript or subscript characters, the input script is returned unchanged.</remarks>
        public static string GetNormalText(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach(char c in input)
            {
                sb.Append(GetNormalChar(c));
            }
            return sb.ToString();
        }
        /// <summary>
        /// Determines whether a Unicode character is a superscript character.
        /// </summary>
        /// <param name="c">Unicode character</param>
        /// <returns>True if Unicode character is a superscript character</returns>
        /// <seealso cref="GetSubscriptText"/>
        public static bool IsSuperscriptChar(char c)
        {
            switch (c)
            {
                case '\x2070':
                case '\x00b9':
                case '\x00b2':
                case '\x00b3':
                case '\x2074':
                case '\x2075':
                case '\x2076':
                case '\x2077':
                case '\x2078':
                case '\x2079':
                case '\x207b':
                case '\x207a':
                    return true;
                default:
                    return false;
            }

        }
        /// <summary>
        /// Get superscript character for Unicode character.
        /// </summary>
        /// <param name="c">The Unicode character to convert to a superscript.</param>
        /// <returns>The superscript Unicode character.</returns>
        /// <remarks>The following characters have Unicode superscript versions: 0,1,2,3,4,5,6,7,8,9,-,+.  If the character argument is not one of these, 
        /// the character is returned.</remarks>
        /// <seealso cref="GetSuperscriptText"/>
        public static char GetSuperscriptChar(char c)
        {
            switch (c)
            {
                case '0':
                    return '\x2070';
                case '1':
                    return '\x00b9';
                case '2':
                    return '\x00b2';
                case '3':
                    return '\x00b3';
                case '4':
                    return '\x2074';
                case '5':
                    return '\x2075';
                case '6':
                    return '\x2076';
                case '7':
                    return '\x2077';
                case '8':
                    return '\x2078';
                case '9':
                    return '\x2079';
                case '-':
                    return '\x207b';
                case '+':
                    return '\x207a';
                default:
                    return c;
            }
        }
        /// <summary>
        /// Converts characters in the input script to superscript characters.
        /// </summary>
        /// <remarks>
        /// The following characters have Unicode superscript versions:
        /// 0,1,2,3,4,5,6,7,8,9,-,+. Characters in the input string which are not one of
        /// these are unaffected.
        /// </remarks>
        /// <param name="input">Input Unicoe string</param>
        /// <returns>
        /// The input string with characters converted to superscript characters
        /// </returns>
        /// <example>
        /// string normalText = "1234567890-+";<br />
        ///  Console.OutputEncoding = System.Text.Encoding.UTF8;<br />
        ///  string superscriptText = UnicodeSupport.GetSuperscriptText(normalText);<br />
        ///  Console.WriteLine("Superscript version of '" + normalText +
        /// "' = '" + superscriptText + "'");<br />
        ///  // Result: "Superscript version of '1234567890-+' =
        /// '¹²³⁴⁵⁶⁷⁸⁹⁰⁻⁺'"<br />
        ///  Console.WriteLine("Superscript version of x2 = " +
        /// UnicodeSupport.GetSuperscriptText("x2"));<br />
        ///  // Result: "Superscript version of x2 = x²" string subscriptText =
        /// UnicodeSupport.GetSubscriptText(normalText);<br />
        ///  Console.WriteLine("Subscript version of '" + normalText +
        /// "' = '" + subscriptText + "'");<br />
        ///  // Result: "Subscript version of '1234567890-+' =
        /// '₁₂₃₄₅₆₇₈₉₀₋₊'"<br />
        ///  Console.WriteLine("Subscript version of H2O = " +
        /// UnicodeSupport.GetSubscriptText("H2O"));<br />
        ///  // Result: "Subscript version of H2O = H₂O"
        /// </example>
        public static string GetSuperscriptText(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                switch (c)
                {
                    case '0':
                        sb.Append('\x2070');
                        break;
                    case '1':
                        sb.Append('\x00b9');
                        break;
                    case '2':
                        sb.Append('\x00b2');
                        break;
                    case '3':
                        sb.Append('\x00b3');
                        break;
                    case '4':
                        sb.Append('\x2074');
                        break;
                    case '5':
                        sb.Append('\x2075');
                        break;
                    case '6':
                        sb.Append('\x2076');
                        break;
                    case '7':
                        sb.Append('\x2077');
                        break;
                    case '8':
                        sb.Append('\x2078');
                        break;
                    case '9':
                        sb.Append('\x2079');
                        break;
                    case '-':
                    case '\x2212':
                        sb.Append('\x207b');
                        break;
                    case '+':
                        sb.Append('\x207a');
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// Determines whether Unicode character is a subscript.
        /// </summary>
        /// <param name="c">The Unicode character to be tested.</param>
        /// <returns>True if Unicode character is a subscript character.</returns>
        /// <seealso cref="GetSubscriptText"/>
        public static bool IsSubscriptChar(char c)
        {
            switch (c)
            {
                case '\x2080':
                case '\x2081':
                case '\x2082':
                case '\x2083':
                case '\x2084':
                case '\x2085':
                case '\x2086':
                case '\x2087':
                case '\x2088':
                case '\x2089':
                case '\x208b':
                case '\x208a':
                    return true;
                default:
                    return false;
            }

        }
        /// <summary>
        /// Convert Unicode character to its subscript equivalent.
        /// </summary>
        /// <param name="c">Unicode character to be converted.</param>
        /// <returns>Subscript equivalent of Unicode character.</returns>
        /// <seealso cref="GetSubscriptText"/>
        public static char GetSubscriptChar(char c)
        {
            switch (c)
            {
                case '0':
                    return '\x2080';
                case '1':
                    return '\x2081';
                case '2':
                    return '\x2082';
                case '3':
                    return '\x2083';
                case '4':
                    return '\x2084';
                case '5':
                    return '\x2085';
                case '6':
                    return '\x2086';
                case '7':
                    return '\x2087';
                case '8':
                    return '\x2088';
                case '9':
                    return '\x2089';
                case '-':
                    return '\x208b';
                case '+':
                    return '\x208a';
                default:
                    return c;
            }
        }
        /// <summary>
        /// Converter characters in string to Unicode subscript versions.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The input strings with characters converted to Unicode subscript equivalents.</returns>
        /// <remarks>If Unicode character has no subscript equivalent, the Unicode character is returned.</remarks>
        /// <remarks>The following characters have Unicode subscript versions: 0,1,2,3,4,5,6,7,8,9,-,+.  Characters in the input string which are
        /// not one of these are unaffected.</remarks>
        /// <example>
        ///    string normalText = "1234567890-+";
        ///    Console.OutputEncoding = System.Text.Encoding.UTF8;
        ///
        ///    string subscriptText = UnicodeSupport.GetSubscriptText(normalText);
        ///    Console.WriteLine("Subscript version of '" + normalText + "' = '" + subscriptText + "'");
        ///    // Result: "Subscript version of '1234567890-+' = '₁₂₃₄₅₆₇₈₉₀₋₊'"
        ///
        ///    Console.WriteLine("Subscript version of H2O = " + UnicodeSupport.GetSubscriptText("H2O"));
        ///    // Result: "Subscript version of H2O = H₂O"
        /// </example>
        public static string GetSubscriptText(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                switch (c)
                {
                    case '0':
                        sb.Append('\x2080');
                        break;
                    case '1':
                        sb.Append('\x2081');
                        break;
                    case '2':
                        sb.Append('\x2082');
                        break;
                    case '3':
                        sb.Append('\x2083');
                        break;
                    case '4':
                        sb.Append('\x2084');
                        break;
                    case '5':
                        sb.Append('\x2085');
                        break;
                    case '6':
                        sb.Append('\x2086');
                        break;
                    case '7':
                        sb.Append('\x2087');
                        break;
                    case '8':
                        sb.Append('\x2088');
                        break;
                    case '9':
                        sb.Append('\x2089');
                        break;
                    case '-':
                        sb.Append('\x208b');
                        break;
                    case '+':
                        sb.Append('\x208a');
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }
    }
}
