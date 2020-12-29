using System;
using System.Collections.Generic;
using System.Text;
using OSECore.Text;
using Xunit;

namespace OSECoreTest.Text
{
    public class TextParserTests
    {
        [Theory]
        [InlineData("a1a", 0, 0, 5, 5)]
        [InlineData("a1a", 1, 2, 1, 5)]
        [InlineData("   123", 0, 6, 123, 5)]
        [InlineData("   -123xxx", 0, 7, -123, 5)]
        [InlineData("   -a", 0, 0,5,5)]
        public void ParseIntegerWithValueTest(string line, int i0, int j0, int v1, int dv)
        {
            int v0;
            int j1 = TextParser.ParseInteger(line, i0, out v0, dv);
            Assert.Equal(j0, j1);
            Assert.Equal(v0, v1);
        }
        [Theory]
        [InlineData("a1a", 0, 0, 5.0, 5.0)]
        [InlineData("a1a", 1, 2, 1.0, 5.0)]
        [InlineData("   123", 0, 6, 123.0, 5.0)]
        [InlineData("   -123xxx", 0, 7, -123.0, 5.0)]
        [InlineData("   -a", 0, 0, 5.0, 5.0)]
        [InlineData("1.0", 0, 3, 1.0, 5.0)]
        [InlineData("1.", 0, 2, 1.0, 5.0)]
        [InlineData("1..0", 0, 2, 1.0, 5.0)]
        [InlineData("1E2", 0, 3, 100.0, 5.0)]
        [InlineData("1.0E-2", 0, 6, 0.01, 5.0)]
        [InlineData("123.0E6;", 0, 7,123.0E6,5.0)]
        public void ParseRealWithValueTest(string line, int i0, int j0, double v0, double dv)
        {
            int j1 = TextParser.ParseReal(line, i0, out var v1, dv);
            Assert.Equal(j0, j1);
            Assert.Equal(v0, v1);
        }
        [Theory]
        [InlineData(" aab",0, 4, "aab", "", "")]
        [InlineData(" 123", 0, 0, "", "", "")]
        [InlineData("aa123-",0, 5, "aa123", "", "")]
        [InlineData("aa*bb",0, 5, "aa*bb","", "*")]
        [InlineData("---",0, 1, "-","-","")]
        public void ParseNameTest(string line, int i0, int j0, string name0, string fc, string nc)
        {
            string name1;
            int j1 = TextParser.ParseName(line, i0, out name1, fc, nc);
            Assert.Equal(j0, j1);
            Assert.Equal(name0, name1);
        }
        [Theory]
        [InlineData(" ,",0,2,",")]
        [InlineData(" a",0,0,"")]
        [InlineData(" ;a",0,2, ";")]
        public void ParseDelimiterTest(string line, int i0, int j0, string dl0)
        {
            string dl1;
            int j1 = TextParser.ParseDelimiter(line, i0, out dl1);
            Assert.Equal(j0, j1);
            Assert.Equal(dl0, dl1);
        }
        [Theory]
        [InlineData(" =",0,"=","=","=")]
        [InlineData(" <<",1,"<<", "<", "<<")]
        [InlineData(" a", 0, "", "", "")]
        [InlineData(" *", 0, "", "", "")]
        [InlineData("=+a", 0,"=","=", "=")]
        [InlineData(" ===",1, "==", "=", "===")]
        public void ParseOperatorTest(string line, int i0, string r0, string r0a, string r0b)
        {
            string[] oplist1 = new string[] { "<", "=", "+" };
            string[] oplist2 = new string[] { "<", "<<", "==", "=", "+=", "+", "=*"};
            string[] oplist3 = new string[] { "<", "<<", "===", "=", "+=", "+", "=*" };
            int j1 = TextParser.ParseOperator(line, i0, out string r1, oplist2);
            Assert.Equal(r0, r1);
            int j1a = TextParser.ParseOperator(line, i0, out string r1a, oplist1);
            Assert.Equal(r0a, r1a);
            int j1b = TextParser.ParseOperator(line, i0, out string r1b, oplist3);
            Assert.Equal(r0b, r1b);
        }
    }
}
