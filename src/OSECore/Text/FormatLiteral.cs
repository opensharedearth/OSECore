using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OSECore.Text
{

    /// <summary>   A format literal. Represents the part of a format that is matched character by character to the input
    ///             line. </summary>
    public class FormatLiteral : LineSegment
    {
        /// <summary>   Constructor taking line, start and length. </summary>
        ///
        /// <param name="line">     The line containing the literal format. </param>
        /// <param name="start">    The zero-based starting index of the literal. </param>
        /// <param name="length">   The length of the literal. </param>
        public FormatLiteral(string line, int start, int length)
            : base(start, length)
        {
            Literal = Get(line);
        }
        /// <summary>   Gets the literal string. </summary>
        ///
        /// <value> The literal. </value>
        public string Literal { get; }
        /// <summary>   Matches the literal string to the input.  The literal string must exactly match the input. </summary>
        ///
        /// <param name="s">    The input string to be tested. </param>
        /// <param name="i0">zero-based starting index</param>
        ///
        /// <returns>   True if the literal matches the input string. </returns>
        public bool Match(string s, int i0 = 0)
        {
            return !String.IsNullOrEmpty(s) && IsValid &&  String.Compare(s, i0, Literal, 0, Length) == 0;
        }
    }
}
