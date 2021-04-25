using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using OSELogic.Command;

namespace OSELogic.Test.Command
{
    public class CommandArgProtoTest
    {
        [Fact]
        public void CtorTest()
        {
            string n0 = "a";
            Usage u0 = new Usage("usage");
            string v0 = "b";
            CommandArgOptions o0 = CommandArgOptions.HasArgument
                | CommandArgOptions.HasMultiple
                | CommandArgOptions.IsRequired
                | CommandArgOptions.IsPositional;
            var d = new CommandArgProto(n0, u0, v0, null, o0);
            Assert.Equal(n0, d.Name);
            Assert.Equal(u0, d.Usage);
            Assert.Equal(v0, d.Value);
            Assert.Equal(o0, d.Options);
            Assert.True(d.IsRequired);
            Assert.True(d.HasMultiple);
            Assert.True(d.HasArgument);
            Assert.False(d.IsSwitch);
            Assert.True(d.IsPositional);
        }
        [Fact]
        public void ValidateTest()
        {
            string n0 = "a";
            Usage u0 = new Usage("usage");
            string v00 = "1abc";
            string v01 = "abc1";
            var d = new CommandArgProto(n0, u0, null, new NameValidator());
            var r00 = d.Validate(v00);
            var r01 = d.Validate(v01);
            Assert.False(r00.Succeeded);
            Assert.True(r01.Succeeded);
        }
        [Fact]
        public void ResolveTest()
        {
            string n0 = "a";
            Usage u0 = new Usage("usage");
            string v00 = "localhost";
            string v01 = "http://localhost";
            var d = new CommandArgProto(n0, u0, v01, new UrlValidator());
            Assert.Throws<ArgumentException>(() => d.Resolve(v00));
            Assert.True((Uri)d.Resolve(v01).ResolvedValue == new Uri(v01));
        }
    }
}
