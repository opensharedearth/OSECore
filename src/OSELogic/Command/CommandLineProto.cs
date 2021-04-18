using System;
using System.Collections.Generic;
using System.Text;

namespace OSELogic.Command
{
    public class CommandLineProto : CommandLine
    {
        public CommandLineProto(string name, string description, string syntax, Func<CommandLine, CommandResult> func, params CommandArgProto[] args)
            : base(args)
        {
            Name = name;
            Description = description;
            Syntax = syntax;
            _command = func;
        }
        public string Name { get; } = "";
        public string Description { get; } = "";
        public string Syntax { get; } = "";
        public Func<CommandLine, CommandResult> _command = null;
        public CommandResult Execute(string[] fields)
        {
            return Execute(new CommandLine(fields));
        }
        public CommandResult Execute(CommandLine argList)
        {
            List<CommandResult> results = new List<CommandResult>();
            int iarg = -1;
            CommandLine args = new CommandLine();
            foreach (var arg in argList)
            {
                if(iarg < 0)
                {
                    args.Add(new CommandArg("Name", arg.Value));
                    iarg++;
                }
                else if(iarg < Count)
                {
                    var argProto = this[iarg] as CommandArgProto;
                    try
                    {
                        args.Add(argProto.Resolve(arg));
                        if (!argProto.HasMultiple)
                            iarg++;
                    }
                    catch(Exception ex)
                    {
                        results.Add(new CommandResult(false, $"Error in argument {argProto.Name}: ", ex));
                    }
                }
                else
                {
                    results.Add(new CommandResult(false, "Too many arguments for command."));
                    break;
                }
            }
            CommandResult result = CommandResult.Aggregate(results.ToArray());
            if(result.Succeeded)
            {
                return Execute(args);
            }
            return result;
        }
    }
}
