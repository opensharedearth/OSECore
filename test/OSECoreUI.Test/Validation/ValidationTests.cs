using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSECoreUI.Validation;
using Xunit;

namespace OSECoreUI.Test.Validation
{
    public class ValidationTests
    {
        [Fact]
        public void ValidateObjectTest()
        {
            object d00 = null;
            object d01 = new Object();
            Assert.Equal(d01, BasicValidation.ValidateObject(d01, "Object"));
            Assert.Throws<ArgumentException>(() => BasicValidation.ValidateObject(d00, "Object"));
        }
        [Fact]
        public void ValidateNameTest()
        {
            string n0 = "asdf";
            string n1 = "1asdf";
            string n2 = null;
            string n3 = " asdf ";
            Assert.Equal(n0, BasicValidation.ValidateName(n0));
            Assert.Throws<ArgumentException>(() => BasicValidation.ValidateName(n1));
            Assert.Equal(n0, BasicValidation.ValidateName(n3));
            Assert.Throws<ArgumentException>(() => BasicValidation.ValidateName(n2));
            Assert.Throws<ArgumentException>(() => BasicValidation.ValidateName(n0, "Name", 3));
        }
        [Fact]
        public void ValidateIntTest()
        {
            string i0 = "123";
            string i1 = "123D";
            string i2 = null;
            string i3 = " 123 ";
            Assert.Equal(123, BasicValidation.ValidateInt(i0));
            Assert.Throws<ArgumentException>(() => BasicValidation.ValidateInt(i1));
            Assert.Equal(123, BasicValidation.ValidateInt(i3));
            Assert.Throws<ArgumentException>(() => BasicValidation.ValidateInt(i2));
        }
        public enum A { a0, a1, a2 };
        [Fact]
        public void ValidateEnumTest()
        {
            string e0 = "a0";
            string e1 = "b0";
            string e2 = null;
            string e3 = " a0 ";
            Assert.Equal(A.a0, BasicValidation.ValidateEnum<A>(e0));
            Assert.Throws<ArgumentException>(() => BasicValidation.ValidateEnum<A>(e1));
            Assert.Equal(A.a0, BasicValidation.ValidateEnum<A>(e3));
            Assert.Throws<ArgumentException>(() => BasicValidation.ValidateEnum<A>(e2));
        }
        [Fact]
        public void ValidateTypeTest()
        {
            string t0 = "System.Int32";
            string t1 = "int";
            string t2 = null;
            string t3 = " System.Int32 ";
            Assert.Equal(typeof(int), BasicValidation.ValidateType(t0, "Type"));
            Assert.Throws<ArgumentException>(() => BasicValidation.ValidateType(t1, "Type"));
            Assert.Equal(typeof(int), BasicValidation.ValidateType(t3, "Type"));
            Assert.Throws<ArgumentException>(() => BasicValidation.ValidateType(t2, "Type"));
        }
    }
}
