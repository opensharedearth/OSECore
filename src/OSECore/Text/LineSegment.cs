using System;
using System.Collections.Generic;
using System.Text;

namespace OSECore.Text
{

    /// <summary>   A line segment. Specifies a range of a text line. </summary>
    public class LineSegment : IEquatable<LineSegment>
    {

        /// <summary>   Constructor taking start and length.  Without both arguments the line segment is zero length
        ///             which is invalid. </summary>
        ///
        /// <param name="start">    (Optional) The zero-based starting character position of the line segment. By default zero.</param>
        /// <param name="length">   (Optional) The length (number of characters) of the line segment. By default zero.</param>
        public LineSegment(string line = null, int start = 0, int length = 0)
        {
            Line = line;
            if(line != null)
            {
                Start = Math.Max(0, Math.Min(start, line.Length));
                Length = Math.Max(0, Math.Min(length, line.Length - Start));
            }
            else
            {
                Start = 0;
                Length = 0;
            }
        }
        public string Line { get; }
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
        public virtual bool IsNull => Length == 0;
        public string Text => IsNull ? "" : Line.Substring(Start, Length);

        public bool Equals(LineSegment other)
        {
            if(other != null)
            {
                return Text == other.Text && Start == other.Start && Length == other.Length;
            }
            return false;
        }
        public override bool Equals(object obj)
        {
            if(obj is LineSegment other)
            {
                return Equals(other);
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return Text.GetHashCode() ^ Start.GetHashCode() ^ Length.GetHashCode();
        }
        public override string ToString()
        {
            return Text;
        }
        public static bool operator==(LineSegment a, LineSegment b)
        {
            if ((object)a == null)
                return (object)b == null;
            else if ((object)b == null)
                return false;
            else
                return a.Equals(b);
        }
        public static bool operator!=(LineSegment a, LineSegment b)
        {
            if ((object)a == null)
                return (object)b != null;
            else if ((object)b == null)
                return true;
            else
                return !a.Equals(b);
        }
    }
}
