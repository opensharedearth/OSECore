using System;
using System.Collections.Generic;
using System.Text;

namespace OSECore.Text
{
    public class LineMargins : IEquatable<LineMargins>
    {
        private int _leftMargin;
        private int _rightMargin;
        private int _hangingIndent;
        public LineMargins(int leftMargin = 1, int rightMargin = 80, int hangingIndent = 10)
        {
            if (leftMargin < 1) throw new ArgumentException("Left margin cannot be less than 1");
            if (leftMargin >= rightMargin) throw new ArgumentException("Left margin must be less than right margin");
            if (hangingIndent < leftMargin || hangingIndent >= rightMargin) throw new AccessViolationException("Invalid hanging indent");
            _leftMargin = leftMargin;
            _rightMargin = rightMargin;
            _hangingIndent = hangingIndent;
        }
        public LineMargins(string s)
        {
            try
            {
                int[] parms = (int[])TextParser.ParseCollection<int>(s);
                _leftMargin = parms[0];
                _rightMargin = parms[1];
                _hangingIndent = parms[2];
            }
            catch(Exception ex)
            {
                throw new ArgumentException("Invalid line margin specfiication", ex);
            }
        }
        public int LeftMargin => _leftMargin;
        public int RightMargin => _rightMargin;
        public int HangingIndent => _hangingIndent;
        public string GetLeftPadding()
        {
            return new string(' ', LeftMargin - 1);
        }
        public string GetHangingIndentPadding()
        {
            return new string(' ', HangingIndent - 1);
        }
        public override string ToString()
        {
            return $"({LeftMargin},{RightMargin},{HangingIndent})";
        }

        public bool Equals(LineMargins other)
        {
            if(other != null)
            {
                if (LeftMargin == other.LeftMargin
                    && RightMargin == other.RightMargin
                    && HangingIndent == other.HangingIndent)
                    return true;
            }
            return false;
        }
        public override bool Equals(object obj)
        {
            if (obj is LineMargins lm)
                return Equals(lm);
            else
                return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return LeftMargin.GetHashCode()
                ^ RightMargin.GetHashCode()
                & HangingIndent.GetHashCode();
        }
        public static bool operator==(LineMargins a, LineMargins b)
        {
            if ((object)a == null)
                return (object)b == null;
            else
                return a.Equals(b);
        }
        public static bool operator !=(LineMargins a, LineMargins b)
        {
            if ((object)a == null)
                return (object)b != null;
            else
                return !a.Equals(b);
        }

    }
}
