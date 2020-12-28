using System;
using System.Collections.Generic;
using System.Text;
using OSECore.Text;
using Xunit;

namespace OSECoreTest.Text
{
    public class TestFormatLiteral
    {
        [Theory]
        [InlineData("abcd",0,3,"abc")]
        public void TestCtor(string s0, int i0, int l0, string r1)
        {
            FormatLiteral fl = new FormatLiteral(s0, i0, l0);
            Assert.Equal(r1, fl.Literal);
        }
        [Theory]
        [InlineData("abcd", 0, 3, "abc",true)]
        [InlineData("abcd", 0, 3, "cde", false)]
        public void TestMatch(string s0, int i0, int l0, string r0, bool v0)
        {
            FormatLiteral fl = new FormatLiteral(s0, i0, l0);
            bool v1 = fl.Match(r0);
            Assert.Equal(v0, v1);
        }
    }
}
