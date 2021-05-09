using OSECore.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OSECore.Test.Text
{
    public class TabStopsTests
    {
        [Fact]
        public void CtorDefaultTest()
        {
            var d = new TabStops();
            Assert.Empty(d);
        }
        [Fact]
        public void CtorCopyTest()
        {
            var d0 = new TabStops("(1,2,3)");
            var d1 = new TabStops(d0);
            Assert.Equal(d0, d1);
        }
        [Fact]
        public void CtorStringTest()
        {
            var s0 = "(1,2,3)";
            var d = new TabStops(s0);
            Assert.Equal(s0, d.ToString());
        }
        [Fact]
        public void CtorEnumerableTest()
        {
            var i0 = new int[] { 1, 2, 3 };
            var s0 = "(1,2,3)";
            var d = new TabStops(i0);
            Assert.Equal(s0, d.ToString());
        }
        [Fact]
        public void ListTest()
        {
            var s0 = "(1,2,3)";
            var d = new TabStops(s0);
            Assert.Equal(1, d[0]);
            Assert.Throws<NotImplementedException>(() => d[0] = 2);
            Assert.Equal(3, d.Count);
            Assert.True(d.IsReadOnly);
            Assert.Throws<NotImplementedException>(() => d.Add(2));
            Assert.Throws<NotImplementedException>(() => d.Clear());
            Assert.Contains(2, d);
            int[] iv0 = { 1, 2, 3 };
            int[] iv1 = new int[3];
            d.CopyTo(iv1, 0);
            Assert.Equal(iv0, iv1);
            Assert.Equal(1, d.IndexOf(2));
            Assert.Throws<NotImplementedException>(() => d.Insert(0, 2));
            Assert.Throws<NotImplementedException>(() => d.Remove(2));
            Assert.Throws<NotImplementedException>(() => d.RemoveAt(0));
        }
        [Fact]
        public void GetTabColumnTest()
        {
            var s0 = "(2,4,6)";
            var d0 = new TabStops(s0);
            Assert.Equal(4, d0.GetTabColumn(2));
            Assert.Equal(2, d0.GetTabColumn(-1));
            Assert.Equal(6, d0.GetTabColumn(4));
            var d1 = new TabStops();
            Assert.Equal(1, d1.GetTabColumn(3));
        }
        [Fact]
        public void AddTabColumnTest()
        {
            var s0 = "(2,4,6)";
            var c0 = 5;
            var s1 = "(2,4,5,6)";
            var d = new TabStops(s0);
            d.AddTabStop(-4);
            Assert.Equal(s0, d.ToString());
            d.AddTabStop(c0);
            Assert.Equal(s1, d.ToString());
        }
        [Fact]
        public void RemoveTabColumnTest()
        {
            var s0 = "(2,4,6)";
            var c0 = 4;
            var s1 = "(2,6)";
            var d = new TabStops(s0);
            d.RemoveTabstop(c0);
            Assert.Equal(s1, d.ToString());
        }
        [Fact]
        public void GetPaddingTest()
        {
            var s0 = "(2,4,6)";
            var c0 = 2;
            string p1 = "   ";
            var d = new TabStops(s0);
            Assert.Equal(p1, d.GetPadding(c0));
        }
        [Fact]
        public void EqualsTest()
        {
            TabStops d00 = null;
            TabStops d01 = new TabStops("(1,2,3)");
            TabStops d02 = new TabStops("(1,2,3)");
            TabStops d03 = new TabStops("(2,4,6)");
            TabStops d04 = new TabStops("(1)");
            Assert.True(d01 == d02);
            Assert.True(d01.Equals(d02));
            Assert.True(d01.GetHashCode() == d02.GetHashCode());
            Assert.True(d01 != d00);
            Assert.False(d00 == d01);
            Assert.False(d03.Equals(d04));
            Assert.False(d02.Equals(d03));
        }
    }
}
