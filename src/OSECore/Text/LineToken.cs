using System;
using System.Collections.Generic;
using System.Text;

namespace OSECore.Text
{
    public class LineToken : LineSegment, IEquatable<LineToken>
    {
        public enum TokenType
        { 
            Null = 0,
            Unknown = 1,
            QuotedString = 2,
            Name = 3,
            Number = 4,
            Delimiter = 5,
            WhiteSpace = 6
        }
        public object Value { get; }
        public TokenType Type { get; }
        public LineToken(string line = null, int start = 0, int length = 0, TokenType type = TokenType.Null, object value = null)
            : base(line, start, length)
        {
            Type = type;
            Value = value;
        }

        public bool Equals(LineToken other)
        {
            throw new NotImplementedException();
        }
        public override bool Equals(object obj)
        {
            if(obj is LineToken other)
            {
                return Equals(other);
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Type.GetHashCode() ^ (Value?.GetHashCode() ?? 0);
        }
        public static bool operator==(LineToken a, LineToken b)
        {
            if ((object)a == null)
                return (object)b == null;
            else if ((object)b == null)
                return false;
            else
                return a.Equals(b);
        }
        public static bool operator!=(LineToken a, LineToken b)
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
