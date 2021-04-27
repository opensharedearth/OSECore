using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSECommand
{
    public class CommandInstance
    {
        public CommandInstance(CommandLineProto command, CommandLine args)
        {
            Command = command;
            Args = args;
        }
        public CommandLineProto Command { get; } = null;
        public CommandLine Args { get; } = null;
        public CommandResult Execute()
        {
            return Command.Execute(Args);
        }
    }
}
