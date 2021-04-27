using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace OSEConfig.Test
{
    public class ConfigVariableTests
    {
        [Fact]
        public void CtorDefaultTest()
        {
            var d = new ConfigVariable();
            Assert.Equal("", d.Name);
            Assert.Null(d.Value);
            Assert.False(d.IsDirty);
            Assert.True(d.IsNull);
            Assert.Equal(ConfigVariable.Null, d);
        }
        [Fact]
        public void Ctor2ArgTest()
        {
            string n0 = "a";
            int v0 = 123;
            var d0 = new ConfigVariable(n0, v0);
            Assert.Equal(n0, d0.Name);
            Assert.Equal(v0.ToString(), d0.Value);
            Assert.Throws<ArgumentException>(() => new ConfigVariable("123a", null));
        }
        enum E { a, b, c };
        [Theory]
        [InlineData(typeof(string), "a",false)]
        [InlineData(typeof(int), 123, false)]
        [InlineData(typeof(float), 123f, false)]
        [InlineData(typeof(double), 123.0, false)]
        [InlineData(typeof(bool), true, false)]
        [InlineData(typeof(DateTime), new object[]{2021, 4, 21, 6, 23, 0}, true)]
        [InlineData(typeof(E), E.a, false)]
        public void GetSetTest(Type t0, object v0, bool construct)
        {
            object u0 = v0;
            if(construct)
            {
                object[] ca = null;
                Type[] ts = null;
                if(v0.GetType().IsArray)
                {
                    List<Type> tl = new List<Type>();
                    List<object> al = new List<object>();
                    foreach(var v in v0 as IEnumerable)
                    {
                        al.Add(v);
                        tl.Add(v.GetType());
                    }
                    ca = al.ToArray();
                    ts = tl.ToArray();
                }
                else
                {
                    ts = new Type[1];
                    ts[0] = v0.GetType();
                    ca = new object[1];
                    ca[0] = v0;
                }
                var ctor = t0.GetConstructor(ts);
                u0 = ctor.Invoke(ca);
            }
            var d0 = new ConfigVariable("v", null);
            d0.SetValue(u0);
            var m = d0.GetType().GetMethod("GetValue");
            var gm = m.MakeGenericMethod(t0);
            var v1 = gm.Invoke(d0, new object[] { null });
            Assert.Equal(u0, v1);
        }
        [Fact]
        public void GetTest()
        {
            var d0 = new ConfigVariable("a", null);
            var d1 = new ConfigVariable("b", "c");
            Assert.Null(d0.GetValue<string>());
            Assert.Equal(1, d0.GetValue<int>(1));
            Assert.Equal(0, d1.GetValue<int>());
            Assert.Equal(1, d1.GetValue<int>(1));
        }
    }
}
