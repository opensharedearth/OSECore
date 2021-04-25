using System;
using System.Collections.Generic;
using System.Text;

namespace OSELogic.Command
{
    public class CommandLineProtoRegistry : Dictionary<string, CommandLineProto>
    {
        public static CommandLineProtoRegistry Instance { get; } = new CommandLineProtoRegistry();
        public static CommandLineProto NullDefinition = new CommandLineProto("", new Usage("The null command"), StandardCommands.Null);
        public CommandLineProtoRegistry() : base(StringComparer.CurrentCultureIgnoreCase)
        {
        }

        public void Define(string name, Usage usage, Func<CommandLine, CommandResult> func, params CommandArgProto[] args)
        {
            this[name] = new CommandLineProto(name, usage, func, args);
        }
        public CommandLineProto Find(string name)
        {
            if(TryGetValue(name, out CommandLineProto cd))
            {
                return cd;
            }
            return new CommandLineProto(name, new Usage(), StandardCommands.InvalidCommand, new CommandArgProto("Ignored",new Usage(),null, null, CommandArgOptions.HasMultiple));
        }
    }
}
