using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using OSECommand;

namespace OSECommand.Test
{
    public class CommandLineProtoTests
    {
        [Fact]
        public void CtorTest()
        {
            string n0 = "a";
            string d0 = "b";
            var d = new CommandLineProto(
                n0,
                new Usage(d0),
                (args) => { return new CommandResult(true, (args.GetValue<int>(1) + args.GetValue<int>(2)).ToString()); },
                new CommandArgProto("add1", 1),
                new CommandArgProto("add2", 2)
                );
            Assert.Equal(n0, d.GetName());
            Assert.Equal(d0, d.GetDescription());
        }
        [Fact]
        public void ExecuteTest()
        {
            string n0 = "a";
            string d0 = "b";
            var d = new CommandLineProto(
                n0,
                new Usage(d0),
                (args) => { return new CommandResult(true, (args.GetValue<int>(0) + args.GetValue<int>(1)).ToString()); },
                new CommandArgProto("add1", 0),
                new CommandArgProto("add2", 1)
                );
            string a1 = "1";
            string a2 = "5";
            string r1 = "6";
            var r = d.Execute(new string[] { a1, a2 });
            Assert.Equal(r1, r.ToString());
        }
        [Theory]
        [InlineData("","", true)]
        [InlineData("--version","", false)]
        [InlineData("add 1 2", "add 1 2", true)]
        [InlineData("--help add", "--help=add", true)]
        [InlineData("add","add", true)]
        public void CheckSwitchesTest(string c0, string c1, bool r1)
        {
            string n0 = "a";
            string d0 = "b";
            var d = new CommandLineProto(
                n0,
                new Usage(d0),
                null,
                new CommandArgProto("opcode", 0, null, null, null, CommandArgOptions.IsRequired),
                new CommandArgProto("add", 1, null, null, null, CommandArgOptions.HasMultiple),
                new CommandArgProto("help", 'h', null, null, null, CommandArgOptions.HasArgument)
                );
            var r0 = new CommandResult();
            var cl1 = d.CheckSwitches(new CommandLine(c0), r0);
            Assert.Equal(r1, r0.Succeeded);
            Assert.Equal(c1, cl1.ToString());
        }
        [Theory]
        [InlineData("", "", true)]
        [InlineData("--version", "--version", true)]
        [InlineData("add 1 2", "add 1", false)]
        [InlineData("add", "add", true)]
        public void CheckPositionalsTest(string c0, string c1, bool r1)
        {
            string n0 = "a";
            string d0 = "b";
            var d = new CommandLineProto(
                n0,
                new Usage(d0),
                null,
                new CommandArgProto("opcode", 0, null, null, null, CommandArgOptions.IsRequired),
                new CommandArgProto("add", 1, null, null, null, CommandArgOptions.None),
                new CommandArgProto("help", 'h', null, null, null, CommandArgOptions.HasArgument)
                );
            var r0 = new CommandResult();
            var cl1 = d.CheckPositionals(new CommandLine(c0), r0);
            Assert.Equal(r1, r0.Succeeded);
            Assert.Equal(c1, cl1.ToString());
        }
        [Theory]
        [InlineData("", false)]
        [InlineData("--version", false)]
        [InlineData("add 1 2", true)]
        [InlineData("add", true)]
        public void CheckRequiredTest(string c0, bool r1)
        {
            string n0 = "a";
            string d0 = "b";
            var d = new CommandLineProto(
                n0,
                new Usage(d0),
                null,
                new CommandArgProto("opcode", 0, null, null, null, CommandArgOptions.IsRequired),
                new CommandArgProto("add", 1, null, null, null, CommandArgOptions.None),
                new CommandArgProto("help", 'h', null, null, null, CommandArgOptions.HasArgument)
                );
            var r0 = new CommandResult();
            d.CheckRequired(new CommandLine(c0), r0);
            Assert.Equal(r1, r0.Succeeded);
        }
        [Theory]
        [InlineData("", "", false)]
        [InlineData("--version", "", false)]
        [InlineData("add 1 2", "add 1", false)]
        [InlineData("add", "add", true)]
        public void ResolveTest(string c0, string c1, bool r1)
        {
            string n0 = "a";
            string d0 = "b";
            var d = new CommandLineProto(
                n0,
                new Usage(d0),
                null,
                new CommandArgProto("opcode", 0, null, null, null, CommandArgOptions.IsRequired),
                new CommandArgProto("add", 1, null, null, null, CommandArgOptions.None),
                new CommandArgProto("help", 'h', null, null, null, CommandArgOptions.HasArgument)
                );
            var r0 = new CommandResult();
            var cl1 = d.Resolve(new CommandLine(c0), r0);
            Assert.Equal(r1, r0.Succeeded);
            Assert.Equal(c1, cl1.ToString());
        }
    }
}
