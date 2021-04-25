using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using OSELogic.Command;

namespace OSELogic.Test.Command
{
    public class CommandLineTests
    {
        [Fact]
        public void CtorDefaultTest()
        {
            var d = new CommandLine();
            Assert.Empty(d);
        }
        [Fact]
        public void CtorLineTest()
        {
            string s00 = "test";
            string s01 = "test\\";
            Assert.Single(new CommandLine(s00));
            Assert.Throws<ArgumentException>(() => new CommandLine(s01));
        }
        [Fact]
        public void CtorFieldsTest()
        {
            string[] a00 = { "test", "abc" };
            Assert.Equal(2, new CommandLine(a00).Count);
        }
        [Fact]
        public void CtorCommandFieldsTest()
        {
            string[] a00 = { "test", "abc" };
            string n0 = "ping";
            Assert.Equal(3, new CommandLine(n0, a00).Count);
        }
        [Fact]
        public void CtorCopyTest()
        {
            string[] a0 = { "test", "abc" };
            var d0 = new CommandLine(a0);
            var d1 = new CommandLine(d0);
            Assert.Equal(2, d1.Count);
        }
        [Fact]
        public void IListTests()
        {
            var a0 = new CommandArg("abc");
            var a1 = new CommandArg("def");
            var d0 = new CommandLine($"--{a0.Name}");
            Assert.Single(d0);
            d0.Insert(0, a1);
            Assert.Equal(a0, d0[a0.Name]);
            Assert.False(d0.IsReadOnly);
            d0.Clear();
            Assert.Empty(d0);
            d0.Add(a0);
            Assert.True(d0.Contains(a0));
            Assert.Single(d0);
            var r0 = new CommandArg[1];
            d0.CopyTo(r0, 0);
            Assert.Equal(a0.Name, r0[0].Name);
            Assert.Equal(0, d0.IndexOf(r0[0]));
            int i = 0;
            foreach (var arg in d0) ++i;
            Assert.Equal(1, i);
            Assert.True(d0.Remove(r0[0]));
            Assert.Empty(d0);
            d0.Add(r0[0]);
            d0.RemoveAt(0);
            Assert.Empty(d0);
        }
        [Fact]
        public void GetValueTest()
        {
            int a0 = 123;
            string n0 = "a";
            string v0 = "abc";
            int i0 = 1;
            string s0 = "b";
            var d = new CommandLine();
            d.Add(new CommandArg(n0, v0));
            d.Add(new CommandArg(i0, a0.ToString()));
            d.Add(new CommandArg(v0, s0));
            Assert.Equal(v0, d[n0].Value);
            Assert.Equal(a0, d.GetValue<int>(i0));
            Assert.Equal(s0, d[v0].Value);
        }
        [Fact]
        public void GetResolvedValueTest()
        {
            string a0 = "123";
            string n0 = "a";
            string v0 = "abc";
            int i0 = 1;
            string s0 = "b";
            var d = new CommandLine();
            d.Add(new CommandArg(n0, v0));
            d.Add(new CommandArg(i0, a0.ToString()));
            Assert.Equal(v0, d.GetResolvedValue<string>(n0));
            Assert.Equal(a0, d.GetResolvedValue<string>(i0));
            Assert.Equal(s0, d.GetResolvedValue<string>(v0, s0));
            Assert.Equal(s0, d.GetResolvedValue<string>(2, s0));
        }
        [Theory]
        [InlineData("", 0, new string[] { })]
        [InlineData("a", 1, new string[] { "a" })]
        [InlineData("  a  ", 1, new string[] { "a" })]
        [InlineData("a b c", 3, new string[] { "a", "b", "c" })]
        [InlineData("a \"b c\"", 2, new string[] { "a", "b c" })]
        [InlineData("a \\\"\\\" c", 3, new string[] { "a", "\"\"", "c" })]
        [InlineData("a\\",0,null)]
        [InlineData("a \" b",0,null)]
        public void ParseFieldsTest(string l0, int n0, string[] args)
        {
            if(args != null)
            {
                string[] args1 = CommandLine.ParseFields(l0);
                Assert.Equal(n0, args.Length);
                Assert.Equal<string>(args, args1);
            }
            else
            {
                Assert.Throws<ArgumentException>(() => CommandLine.ParseFields(l0));
            }
        }
        [Theory]
        [InlineData("a --b -cde", 5, 
            new string[] { "Arg0", "b", "c", "d", "e" },
            new string[] { "a", null, null, null, null },
            new bool[] { false, true, true, true, true }
        )]
        [InlineData("a --b=c e -de", 5,
            new string[] { "Arg0", "b", "Arg1", "d", "e" },
            new string[] { "a", "c", "e", null, null },
            new bool[] { false, true, false, true, true }
        )]
        public void ParseArgumentsTest(string l0, int n0, string[] name, string[] value, bool[] isSwitch)
        {
            var d = new CommandLine(l0);
            Assert.Equal(n0, d.Count);
            int iarg = 0;
            foreach(var r1 in d)
            {
                Assert.Equal(name[iarg], r1.Name);
                Assert.Equal(value[iarg], r1.Value);
                Assert.Equal(isSwitch[iarg], r1.IsSwitch);
                iarg++;
            }
        }
    }
}
