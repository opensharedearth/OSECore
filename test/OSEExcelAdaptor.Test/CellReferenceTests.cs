using System;
using Xunit;
using OSEExcelAdaptor;

namespace OSEExcelAdaptor.Test
{
    public class CellReferenceTests
    {
        [Fact]
        public void CtorDefaultTest()
        {
            var d = new CellReference();
            Assert.Equal("", d.Column);
            Assert.Equal(0, d.Row);
            Assert.Equal(CellReference.Null, d);
            Assert.True(d.IsNull);
        }
        [Fact]
        public void Ctor2ArgTest()
        {
            var d0 = new CellReference(1, "A");
            var d1 = new CellReference(1, 1);
            var d2 = new CellReference(2, "A");
            Assert.True(d0 == d1);
            Assert.True(d0 != d2);
            Assert.ThrowsAny<Exception>(() => new CellReference(-1, "A"));
            Assert.ThrowsAny<Exception>(() => new CellReference(1, "AAA"));
            Assert.ThrowsAny<Exception>(() => new CellReference(1, -1));
            Assert.ThrowsAny<Exception>(() => new CellReference(-1, 1));
        }
        [Theory]
        [InlineData("A",true)]
        [InlineData("z", true)]
        [InlineData("Aa", true)]
        [InlineData("zz", true)]
        [InlineData("aaa", false)]
        [InlineData("a1", false)]
        [InlineData("1a", false)]
        [InlineData("z1", false)]
        [InlineData("1z", false)]
        public void IsValidColumnTest(string n0, bool b0)
        {
            Assert.Equal(b0, CellReference.IsValidColumn(n0));
        }
        [Theory]
        [InlineData(1,true)]
        [InlineData(1000,true)]
        [InlineData(-1, false)]
        [InlineData(0, false)]
        public void IsValidRowTest(int v0, bool b0)
        {
            Assert.Equal(b0, CellReference.IsValidRow(v0));
        }
        [Theory]
        [InlineData(null,false, "", 0, 0)]
        [InlineData("A1",true, "A", 1, 1)]
        [InlineData("AA100", true, "AA", 27, 100)]
        [InlineData("ZZ11", true, "ZZ", 26*26 + 26, 11)]
        [InlineData("B", false, "", 0 , 0)]
        [InlineData("C-10", false, "", 0, 0)]
        public void Ctor1Arg(string a0, bool b0, string c0, int i0, int r0)
        {
            CellReference d = null;
            if (!b0)
                Assert.ThrowsAny<Exception>(() => new CellReference(a0));
            else
                d = new CellReference(a0);
            if(d != null)
            {
                Assert.Equal(c0, d.Column);
                Assert.Equal(i0, d.ColumnIndex);
                Assert.Equal(r0, d.Row);
            }
        }
        [Theory]
        [InlineData(null, null, "=")]
        [InlineData(null,"A1","<")]
        [InlineData("A1", null, ">")]
        [InlineData("A1", "A1", "=")]
        [InlineData("A1", "B1", "<")]
        [InlineData("B1", "A10", ">")]
        [InlineData("A10", "AA1", "<")]
        public void CompareTest(string r01, string r02, string t0)
        {
            var d01 = r01 == null ? null : new CellReference(r01);
            var d02 = r02 == null ? null : new CellReference(r02);
            switch (t0)
            {
                case "=":
                    Assert.True(d01 == d02);
                    Assert.False(d01 != d02);
                    Assert.False(d01 < d02);
                    Assert.False(d01 > d02);
                    Assert.True(d01 <= d02);
                    Assert.True(d01 >= d02);
                    if(d01 != null)Assert.True(d01.Equals(d02));
                    if (d01 != null) Assert.Equal(0, d01.CompareTo(d02));
                    if(d01 != null && d02 != null) Assert.Equal(d01.GetHashCode(), d02.GetHashCode());
                    break;
                case "<":
                    Assert.False(d01 == d02);
                    Assert.True(d01 != d02);
                    Assert.True(d01 < d02);
                    Assert.False(d01 > d02);
                    Assert.True(d01 <= d02);
                    Assert.False(d01 >= d02);
                    if(d01 != null)Assert.False(d01.Equals(d02));
                    if (d01 != null) Assert.Equal(-1, d01.CompareTo(d02));
                    if (d01 != null && d02 != null) Assert.NotEqual(d01.GetHashCode(), d02.GetHashCode());
                    break;
                case ">":
                    Assert.False(d01 == d02);
                    Assert.True(d01 != d02);
                    Assert.False(d01 < d02);
                    Assert.True(d01 > d02);
                    Assert.False(d01 <= d02);
                    Assert.True(d01 >= d02);
                    if (d01 != null) Assert.False(d01.Equals(d02));
                    if (d01 != null) Assert.Equal(1, d01.CompareTo(d02));
                    if (d01 != null && d02 != null) Assert.NotEqual(d01.GetHashCode(), d02.GetHashCode());
                    break;
                default:
                    Assert.False(true);
                    break;
            }
        }
        [Fact]
        public void ObjectTest()
        {
            var d = new CellReference("A1");
            Assert.False(d.IsNull);
            Assert.Equal("A1", d.ToString());
            Assert.Equal("", CellReference.Null.ToString());
        }
    }
}
