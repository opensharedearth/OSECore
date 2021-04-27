using System;
using System.Collections.Generic;
using System.Reflection;
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
        enum E { a, b, c };
        [Theory]
        [InlineData("123",typeof(int))]
        [InlineData("2.3",typeof(float))]
        [InlineData("2.456",typeof(double))]
        [InlineData("2021-04-21T05:23:10.0000000",typeof(DateTime))]
        [InlineData("a",typeof(E))]
        public void GetSetValueTest(string s0, Type t0)
        {
            var m = typeof(StringExtensions).GetMethod("GetValue", BindingFlags.Static | BindingFlags.Public);
            var gm = m.MakeGenericMethod(t0);
            var v1 = gm.Invoke(null, new object[] { s0, null });
            Assert.Equal(v1.GetType(), t0);
            var s1 = StringExtensions.SetValue(v1);
            Assert.Equal(s0, s1);
        }
    }
}
