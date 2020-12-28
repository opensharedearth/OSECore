using OSECore.Object;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Xunit;

namespace OSECoreTest.Object
{
    public class TestObjectConverter
    {

        private ObjectConverter _oc = new ObjectConverter(typeof(System.Drawing.Size),
            new Type[]
            {
                typeof(int), typeof(int)
            },
            (d) =>
            {
                var s = (System.Drawing.Size) d;
                return new object[]
                {
                    s.Width, s.Height
                };
            },
            (ol) => new System.Drawing.Size((int) ol[0], (int) ol[1]),
            "({0},{1})"
        );
        [Fact]
        public void TestParser()
        {
            string s0 = "(10,20)";
            Size d0 = new Size(10, 20);
            Size d1 = (Size)_oc.Construct(s0);
            string s1 = _oc.Format(d1);
            Assert.Equal(d0, d1);
            Assert.Equal(s0, s1);
        }
        [Fact]
        public void TestConstruct()
        {
            Size d1 = (Size) _oc.Construct(new object[] {10, 20});
            object[] ol = _oc.Extract(d1);
            Assert.Equal(10, d1.Width);
            Assert.Equal(20, d1.Height);
            Assert.Equal(10, ol[0]);
            Assert.Equal(20, ol[1]);
        }
    }


}
