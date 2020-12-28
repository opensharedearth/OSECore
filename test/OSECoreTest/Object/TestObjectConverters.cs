using OSECore.Object;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Xunit;

namespace OSECoreTest.Object
{
    public class TestObjectConverters
    {
        [Fact]
        public void TestAdd()
        {
            ObjectConverters.Add(new ObjectConverter(typeof(System.Drawing.Size),
                new Type[] { typeof(int), typeof(int) },
                (d) =>
                {
                    var s = (System.Drawing.Size)d;
                    return new object[] { s.Width, s.Height };
                },
                (ol) => new System.Drawing.Size((int)ol[0], (int)ol[1]),
                "({0},{1})"
            ));
            ObjectConverters.Add(new ObjectConverter(typeof(Complex),
                new Type[] { typeof(double), typeof(double) },
                (d) =>
                {
                    var c = (Complex)d;
                    return new object[] { c.Real, c.Imaginary };

                },
                (ol) => new Complex((double)ol[0], (double)ol[1]),
                "({0},{1})"));
            Assert.Equal(2, ObjectConverters.Instance.Count);
            Assert.Equal(typeof(Complex), ObjectConverters.Find(typeof(Complex)).Foundation);
        }
    }
}
