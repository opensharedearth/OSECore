using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using OSECommand;

namespace OSECommand.Test
{
    public class CommandArgTests
    {
        [Fact]
        public void CtorSwitchTest()
        {
            string n0 = "a";
            var d = new CommandArg(n0);
            Assert.Equal(n0, d.Name);
            Assert.True(d.IsSwitch);
            Assert.False(d.IsPositional);
            Assert.False(d.IsNull);
            Assert.Null(d.Value);
            Assert.Null(d.ResolvedValue);
        }
        [Fact]
        public void CtorNullTest()
        {
            var d = new CommandArg();
            Assert.Equal("", d.Name);
            Assert.False(d.IsSwitch);
            Assert.False(d.IsPositional);
            Assert.True(d.IsNull);
            Assert.Null(d.Value);
            Assert.Null(d.ResolvedValue);
        }
        [Fact]
        public void SingletonTest()
        {
            var d = CommandArg.Null;
            Assert.NotNull(d);
            Assert.Equal("", d.Name);
            Assert.False(d.IsSwitch);
            Assert.False(d.IsPositional);
            Assert.True(d.IsNull);
            Assert.Null(d.Value);
            Assert.Null(d.ResolvedValue);
        }
        [Fact]
        public void CtorPositionalTest()
        {
            string v0 = "a";
            int i0 = 1;
            var d = new CommandArg(i0,v0);
            Assert.Equal($"Arg{i0}", d.Name);
            Assert.False(d.IsSwitch);
            Assert.True(d.IsPositional);
            Assert.False(d.IsNull);
            Assert.Equal(i0, d.PositionIndex);
            Assert.Equal(v0, d.Value);
            Assert.Equal(v0, d.ResolvedValue as string);
        }
        [Fact]
        public void CtorSwitchValueTest()
        {
            string v0 = "a";
            string n0 = "b";
            var d = new CommandArg(n0, v0);
            Assert.Equal(n0, d.Name);
            Assert.True(d.IsSwitch);
            Assert.False(d.IsPositional);
            Assert.False(d.IsNull);
            Assert.Equal(v0, d.Value);
            Assert.Equal(v0, d.ResolvedValue as string);
        }
    }
}
