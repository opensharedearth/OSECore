using System;
using System.Collections.Generic;
using System.Text;
using OSECore.Text;
using Xunit;

namespace OSECoreTest.Text
{
    public class TestLineSegment
    {
        [Theory]
        [InlineData(0, 0, false)]
        [InlineData(10, 100, true)]
        public void TestCtor(int s0, int l0, bool v0)
        {
            LineSegment ls = new LineSegment(s0, l0);
            Assert.Equal(s0, ls.Start);
            Assert.Equal(l0, ls.Length);
            Assert.Equal(v0, ls.IsValid);
            Assert.Equal(s0 + l0 - 1, ls.End);
            Assert.Equal(s0 + l0, ls.Next);
        }

        [Theory]
        [InlineData("abcd",0,0,"")]
        [InlineData("abcd",1,1,"b")]
        [InlineData("abcd",2,4,"cd")]
        public void TestGet(string s0, int i0, int l0, string r0)
        {
            LineSegment ls = new LineSegment(i0, l0);
            string r1 = ls.Get(s0);
            Assert.Equal(r0, r1);
        }
    }
}
