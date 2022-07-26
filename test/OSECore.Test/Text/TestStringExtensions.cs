using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using OSECore.Text;
using Xunit;

namespace OSECoreTest.Text
{
    public struct Point3D
    {
        public long X;
        public long Y;
        public long Z;
        public override string ToString()
        {
            return $"({X},{Y},{Z})";
        }
        public static Point3D Parse(string s)
        {
            if (TryParse(s, out Point3D p))
                return p;
            else
                throw new FormatException();
        }
        public static bool TryParse(string s, out Point3D p)
        {
            if (!String.IsNullOrEmpty(s) && s.Length >= "(0.0,0)".Length && s[0] == '(' && s[s.Length - 1] == ')')
            {
                string[] ss = s.Substring(1, s.Length - 2).Split(',');
                if (ss.Length == 3)
                {
                    try
                    {
                        p = new Point3D() { X = ss[0].GetValue<long>(), Y = ss[1].GetValue<long>(), Z = ss[2].GetValue<long>() };
                        return true;
                    }
                    catch { }
                }
            }
            p = new Point3D();
            return false;
        }
    }

    public class PartNumber
    {
        public enum Series { A, B, C };
        public Series PartSeries { get; set; }
        public string PartType { get; set; }
        public int Version { get; set; }
        public override string ToString()
        {
            return $"{PartSeries}-{PartType}-{Version}";
        }

        public static PartNumber Parse(string s)
        {
            if (TryParse(s, out PartNumber pn))
                return pn;
            else
                throw new FormatException();

        }
        public static bool TryParse(string s, out PartNumber pn)
        {
            if (!String.IsNullOrEmpty(s))
            {
                string[] ss = s.Split('-');
                if (ss.Length == 3)
                {
                    try
                    {
                        pn = new PartNumber() { PartSeries = ss[0].GetValue<Series>(), PartType = ss[1], Version = ss[2].GetValue<int>() };
                        return true;
                    }
                    catch { }
                }
            }
            pn = null;
            return false;
        }
    }
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
        [InlineData("(1,2,3)", typeof(Point3D))]
        [InlineData("A-BX00-1", typeof(PartNumber))]
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
        [InlineData("(4,5,6)", typeof(Point3D), null)]
        public void GetSetValueWithDefaultTest(string s0, Type t0, object d0)
        {
            var gm = getValue2ArgMethod.MakeGenericMethod(t0);
            var v1 = gm.Invoke(null, new object[] { s0, d0 });
            Assert.Equal(v1.GetType(), t0);
            var s1 = StringExtensions.SetValue(v1);
            Assert.Equal(s0, s1);
        }
        [Fact]
        public void NullStringTest()
        {
            string s = null;
            Assert.Throws<ArgumentNullException>(() => { int i = s.GetValue<int>(); });
            int i0 = 5;
            int i1 = s.GetValue<int>(i0);
            Assert.Equal(i0, i1);
            string t = StringExtensions.SetValue(null);
        }
        [Fact]
        public void BadEnumTest()
        {
            string s = "d";
            Assert.Throws<FormatException>(() => { E e = s.GetValue<E>(); });
            E e0 = E.a;
            E e1 = s.GetValue<E>(e0);
            Assert.Equal(e0, e1);
        }
        [Fact]
        public void BadTypeTest()
        {
            string s = "";
            Assert.Throws<ArgumentException>(() => { Type t = s.GetValue<Type>(); });
            Type t0 = typeof(int);
            Type t1 = s.GetValue<Type>(t0);
            Assert.Equal(t0, t1);
        }
        [Fact]
        public void BadValueTest()
        {
            string s = "d123";
            int i0 = 123;
            int i1 = s.GetValue<int>(i0);
            Assert.Equal(i0, i1);
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
