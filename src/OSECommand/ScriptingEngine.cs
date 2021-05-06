using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSECommand
{
    public class ScriptingEngine
    {
        public static ScriptingEngine Instance { get; } = new ScriptingEngine();
        public ScriptingEngine()
        {

        }
        public CommandResult Execute(string line)
        {
            return Execute(new CommandLine(line));
        }
        public CommandResult Execute(params string[] fields)
        {
            return Execute(new CommandLine(fields));
        }
        public CommandResult Execute(string commandName, string[] args)
        {
            return Execute(new CommandLine(commandName, args));
        }
        public CommandResult Execute(CommandLine line)
        {
            var command = CommandLineProtoRegistry.Instance.Find(line);
            if (command != null)
                return command.Execute(line);
            else
                return new CommandResult(false, "Invalid command line");
        }
        public CommandResult Execute(CommandLineProto cd, CommandLine args)
        {
            return Execute(new CommandInstance(cd, args));
        }
        public CommandResult Execute(CommandInstance command)
        {
            return command.Command.Execute(command.Args);
        }
        public CommandResult Execute(CommandScript script)
        {
            CommandResult result = new CommandResult();
            foreach(CommandInstance ci in script)
            {
                result.Append(ci.Execute());
                if (!result.Continue)
                    break;
            }
            return result;
        }
    }
}
