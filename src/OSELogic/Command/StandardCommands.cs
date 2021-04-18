using OSELogic.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OSELogic.Command
{
    public static class StandardCommands
    {
        public struct Names
        {
            public const string GeneralSection = "General";
            public const string WorkingFolder = "working-folder";
            public const string VersionCommand = "Version";
            public const string HelpCommand = "Help";
        }
        public static void RegisterAll()
        {
            CommandLineProtoRegistry.Instance.Define("", "", "", Null);
            CommandLineProtoRegistry.Instance.Define(Names.VersionCommand, "Program version", "Version", Version);
            CommandLineProtoRegistry.Instance.Define(Names.HelpCommand, "General Program Help", "Help", Help,
                new CommandArgProto("Command", "Command name")
            );
            CommandLineProtoRegistry.Instance.Define(Names.WorkingFolder, "Get or set working folder", "Working-Folder [folder]", WorkingFolder,
                new CommandArgProto("Folder", "Path to working folder", "", new FolderValidator())
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
            result.AddMessage($"{GetProgramName()} {GetProgramVersion()}");
            result.AddMessage();
            var description = GetProgramDescription();
            if (!String.IsNullOrEmpty(description))
            {
                result.AddMessage(description);
                result.AddMessage();
            }
            var names = from name in commands.Keys orderby name select name;
            foreach (var name in names)
            {
                string commandDescription = commands[name].Description;
                result.AddMessage($"{name}\t{commandDescription}");
            }
            return result;
        }
        public static CommandResult CommandHelp(string command, CommandLineProtoRegistry commands)
        {
            if(commands.TryGetValue(command, out CommandLineProto cd))
            {
                CommandResult result = new CommandResult();
                result.AddMessage($"{command}\t{cd.Description}");
                result.AddMessage();
                result.AddMessage($"{cd.Syntax}");
                if(cd.Count > 0)
                {
                    result.AddMessage();
                    result.AddMessage("Where:");
                    result.AddMessage();
                    foreach(var arg in cd.OfType<CommandArgProto>())
                    {
                        result.AddMessage($"{arg.Name}\t{arg.Description}");
                    }
                }
                return result;
            }
            else
            {
                return new CommandResult(false, "'{command}' is not a command");
            }
        }
        public static CommandResult WorkingFolder(CommandLine args)
        {
            if(args.Count == 2)
            {
                GeneralParameters.Instance.WorkingFolder = args[1].Value;
            }
            return new CommandResult(true, GeneralParameters.Instance.WorkingFolder);
        }
        public static string GetProgramName()
        {
            Assembly a = Assembly.GetEntryAssembly();
            var name = a.GetName();
            return name.Name;
        }
        public static string GetProgramVersion()
        {
            Assembly a = Assembly.GetEntryAssembly();
            var name = a.GetName();
            return name.Version.ToString();

        }
        public static string GetProgramDescription()
        {
            Assembly a = Assembly.GetEntryAssembly();
            var description = a.GetCustomAttribute<AssemblyDescriptionAttribute>();
            if (description != null)
            {
                return description.Description;
            }
            else
            {
                return "";
            }
        }
    }
}
