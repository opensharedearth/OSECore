using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OSEConfig;
using OSECore.Program;

namespace OSECommand
{
    public static class StandardCommands
    {
        public struct Names
        {
            public const string WorkingFolder = "working-folder";
            public const string VersionCommand = "Version";
            public const string HelpCommand = "Help";
        }
        public static void RegisterAll()
        {
            RegisterVersionCommand();
            RegisterHelpCommand();
            RegisterWorkingFolderCommand();
        }
        public static void RegisterVersionCommand()
        {
            CommandLineProtoRegistry.Instance.Register(new CommandLineProto(Names.VersionCommand, new Usage("Program version", new UsageProto("Version")), Version));
        }
        public static void RegisterHelpCommand()
        {
            CommandLineProtoRegistry.Instance.Register(new CommandLineProto(Names.HelpCommand, new Usage("General Program Help", new UsageProto("Help")), Help,
                new CommandArgProto("Command", 1, new Usage("Command name")))
            );
        }
        public static void RegisterWorkingFolderCommand()
        {
            CommandLineProtoRegistry.Instance.Register(new CommandLineProto(Names.WorkingFolder, new Usage("Get or set working folder", new UsageProto("Working-Folder [folder]")), WorkingFolder,
                new CommandArgProto("folder", 1, new Usage("Path to working folder"), "", new FolderValidator(), CommandArgOptions.IsPositional))
            );
        }
        public static CommandResult Null(CommandLine args)
        {
            return new CommandResult();
        }
        public static CommandResult Version(CommandLine args)
        {
            Assembly a = Assembly.GetEntryAssembly();
            var name = a.GetName();
            return new CommandResult(true, $"{name.Name} {name.Version}");
        }
        public static CommandResult InvalidCommand(CommandLine args)
        {
            return new CommandResult(false, $"'{args[0].Name}' is not a command.");
        }
        public static CommandResult Help(CommandLine args)
        {
            if (args.Count <= 1)
                return GeneralHelp(CommandLineProtoRegistry.Instance);
            else
                return CommandHelp(args[1].Value as string, CommandLineProtoRegistry.Instance);
        }
        public static CommandResult GeneralHelp(CommandLineProtoRegistry commands)
        {
            CommandResult result = new CommandResult();
            result.AddMessage($"{ProgramInfo.GetProgramName()} {ProgramInfo.GetProgramVersion()}");
            result.AddMessage();
            var description = ProgramInfo.GetProgramDescription();
            if (!String.IsNullOrEmpty(description))
            {
                result.AddMessage(description);
                result.AddMessage();
            }
            CommandLineProto[] protos = commands.GetAll();
            foreach (var proto in protos)
            {
                result.AddMessage($"{proto.GetName()}\t{proto.GetDescription()}");
            }
            return result;
        }
        public static CommandResult CommandHelp(string command, CommandLineProtoRegistry commands)
        {
            CommandLineProto[] protos = commands.Find(command);
            CommandResult result = new CommandResult();
            foreach (var proto in protos)
            {
                result.AddMessage($"{proto.GetName()}\t{proto.GetDescription()}");
                result.AddMessage();
                if(proto.Count > 0)
                {
                    result.AddMessage();
                    result.AddMessage("Where:");
                    result.AddMessage();
                    foreach(var arg in proto.OfType<CommandArgProto>())
                    {
                        result.AddMessage($"{arg.Name}\t{arg.Description}");
                    }
                }
            }
            if(protos.Length == 0)
            {
                return new CommandResult(false, "'{command}' is not a command");
            }
            return result;
        }
        public static CommandResult WorkingFolder(CommandLine args)
        {
            if(args.Count == 2)
            {
                GeneralParameters.Instance.WorkingFolder = args[1].Value;
            }
            return new CommandResult(true, GeneralParameters.Instance.WorkingFolder);
        }
    }
}
