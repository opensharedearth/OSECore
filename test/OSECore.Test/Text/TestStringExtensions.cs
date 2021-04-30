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
        static MethodInfo getValue2ArgMethod = GetMethod(typeof(StringExtensions), "GetValue", BindingFlags.Static | BindingFlags.Public, 2);
        static MethodInfo getValue1ArgMethod = GetMethod(typeof(StringExtensions), "GetValue", BindingFlags.Static | BindingFlags.Public, 1);
        enum E { a, b, c };
        [Theory]
        [InlineData("123",typeof(int))]
        [InlineData("2.3",typeof(float))]
        [InlineData("2.456",typeof(double))]
        [InlineData("2021-04-21T05:23:10.0000000",typeof(DateTime))]
        [InlineData("a",typeof(E))]
        public void GetSetValueTest(string s0, Type t0)
        {
            var gm = getValue1ArgMethod.MakeGenericMethod(t0);
            var v1 = gm.Invoke(null, new object[] { s0 });
            Assert.Equal(v1.GetType(), t0);
            var s1 = StringExtensions.SetValue(v1);
            Assert.Equal(s0, s1);
        }
        [Theory]
        [InlineData("123", typeof(int), 456)]
        [InlineData("2.3", typeof(float), 4.5f)]
        [InlineData("2.456", typeof(double), 5.6)]
        [InlineData("2021-04-21T05:23:10.0000000", typeof(DateTime),null)]
        [InlineData("a", typeof(E), E.a)]
        public void GetSetValueWithDefaultTest(string s0, Type t0, object d0)
        {
            var gm = getValue2ArgMethod.MakeGenericMethod(t0);
            var v1 = gm.Invoke(null, new object[] { s0, d0 });
            Assert.Equal(v1.GetType(), t0);
            var s1 = StringExtensions.SetValue(v1);
            Assert.Equal(s0, s1);
        }

        private static MethodInfo GetMethod(Type type, string name, BindingFlags bindingFlags, int narg)
        {
            MemberInfo[] members = type.GetMember(name, bindingFlags);
            foreach(var m in members)
            {
                if(m is MethodInfo mi)
                {
                    if (mi.GetParameters().Length == narg)
                        return mi;
                }
            }
            return null;
        }
    }
}
