using System;
using System.Collections.Generic;
using System.Text;
using OSECore.Text;
using Xunit;

namespace OSECoreTest.Text
{
    public class TestStringExtensions
    {
        [Theory]
        [InlineData("({0},{1})",new Type[] { typeof(int), typeof(int) }, new object[] { 1, 2 })]
        [InlineData("{0:d5}", new Type[] { typeof(int) }, new object[] { 3 })]
        [InlineData("{0,10:f5}", new Type[] { typeof(float) }, new object[] { 300.44f })]
        [InlineData("{0,10:e5}", new Type[] { typeof(double) }, new object[] { 300.44e4 })]
        [InlineData("{0,10:g5}", new Type[] { typeof(double) }, new object[] { 300.44e4 })]
        [InlineData("({0} {1:f1}", new Type[] { typeof(string), typeof(double)}, new object[] { "BBX", 1.0})]
//        [InlineData("({0}{1:f1}", new Type[] { typeof(string), typeof(double) }, new object[] { "BBX", 1.0 })]
        [InlineData("({0}", new Type[] { typeof(string) }, new object[] { "BBX"})]
        public void TestParse(string t0, Type[] tl0, object[] dl0)
        {
            string s0 = String.Format(t0, dl0);

            object[] dl1 = s0.Parse(t0, tl0);
            Assert.Equal(dl0.Length, dl1.Length);
            for (int i = 0; i < dl0.Length; ++i)
            {
                Assert.Equal(dl0[i], dl1[i]);
            }
        }
    }
}
