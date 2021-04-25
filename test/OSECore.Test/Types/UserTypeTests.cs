using OSECore.Object;
using OSECore.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OSECore.Test.Types
{
    public class UserTypeTests
    {
        private ObjectConverter _oc = new ObjectConverter(typeof(System.Drawing.Size),
            new Type[]
            {
                typeof(int), typeof(int)
            },
            (d) =>
            {
                var s = (System.Drawing.Size)d;
                return new object[]
                {
                    s.Width, s.Height
                };
            },
            (ol) => new System.Drawing.Size((int)ol[0], (int)ol[1]),
            "({0},{1})"
        );
        [Fact]
        public void CtorTest()
        {
            string n0 = "user type";
            Type t0 = typeof(string);
            var t = new UserType(n0, t0, _oc);
            Assert.Equal(n0, t.Name);
            Assert.Equal(t0, t.Type);
            Assert.Equal(_oc, t.Converter);
        }
    }
}
