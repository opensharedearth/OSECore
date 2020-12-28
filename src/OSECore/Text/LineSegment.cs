using System;
using System.Collections.Generic;
using System.Text;

namespace OSECore.Text
{

    /// <summary>   A line segment. Specifies a range of a text line. </summary>
    public class LineSegment
    {

        /// <summary>   The maximum length. Arbitrarily set to 65,536.</summary>
        public const int MaxLength = 1 << 16;
        /// <summary>   Constructor taking start and length.  Without both arguments the line segment is zero length
        ///             which is invalid. </summary>
        ///
        /// <param name="start">    (Optional) The zero-based starting character position of the line segment. By default zero.</param>
        /// <param name="length">   (Optional) The length (number of characters) of the line segment. By default zero.</param>
        public LineSegment(int start = 0, int length = 0)
        {
            Start = Math.Max(0, Math.Min(start, MaxLength));
            Length = Math.Max(0, Math.Min(length, MaxLength));
        }
        /// <summary>   Gets the start of the line segment. </summary>
        ///
        /// <value> The start. </value>
        public int Start { get; }
        /// <summary>   Gets the length of the line segment. </summary>
        ///
        /// <value> The length. </value>
        public int Length { get; }
        /// <summary>   Gets the end of the line segment.  This is the start plus length minus one. </summary>
        ///
        /// <value> The end character position of the line segment. </value>
        public int End => Start + Length - 1;
        /// <summary>   Gets the next character position following the line segment. </summary>
        ///
        /// <value> The next character position. </value>
        public int Next => Start + Length;
        /// <summary>   Gets a value indicating whether this object is valid. To be valid the start must be >= 0 and the length must be > 0. Both start and length
        ///             must be less than maximum length. </summary>
        ///
        /// <value> True if this object is valid, false if not. </value>
        public virtual bool IsValid => Start >= 0 && Length > 0;
        /// <summary>   Get the line segment from a line. </summary>
        ///
        /// <param name="line"> The line from which to get the segment. </param>
        ///
        /// <returns>   The specified segment of the line. </returns>
        public string Get(string line)
        {
            if (IsValid && !String.IsNullOrEmpty(line))
            {
                return line.Substring(Start, Math.Min(line.Length - Start, Length));
            }

            return "";
        }
    }
}
