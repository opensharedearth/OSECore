using System;
using System.Collections.Generic;
using System.IO;
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
            public const string RestoreWorkingFolder = "restore-working-folder";
            public const string UpdateWorkingFolder = "update-working-folder";
            public const string VersionCommand = "version";
            public const string HelpCommand = "help";
            public const char HelpMnemonic = 'h';
        }
        public static void RegisterAll()
        {
            RegisterVersionCommand();
            RegisterHelpCommand();
            RegisterWorkingFolderCommands();
        }
        public static void RegisterVersionCommand()
        {
            CommandLineProtoRegistry.Instance.Register(new CommandLineProto(Names.VersionCommand, new Usage("Get program version", new UsageProto("Version")), Version));
        }
        public static void RegisterHelpCommand()
        {
            CommandLineProtoRegistry.Instance.Register(new CommandLineProto(Names.HelpCommand, new Usage("Get program help", new UsageProto("Help")), Help,
                new CommandArgProto("Command", 1, new Usage("Command name")))
            );
        }
        public static void RegisterWorkingFolderCommands()
        {
            CommandLineProtoRegistry.Instance.Register(new CommandLineProto(Names.WorkingFolder, new Usage("Get or set working folder", new UsageProto("Working-Folder [folder]")), WorkingFolder,
                new CommandArgProto("folder", 1, new Usage("Path to working folder"), "", new FolderValidator(), CommandArgOptions.IsPositional))
            );
            CommandLineProtoRegistry.Instance.Register(new CommandLineProto(Names.RestoreWorkingFolder, new Usage("Set current directory from saved working folder", new UsageProto("Restore-Working-Folder")), RestoreWorkingFolder));
            CommandLineProtoRegistry.Instance.Register(new CommandLineProto(Names.UpdateWorkingFolder, new Usage("Update save working folder from current directory.", new UsageProto("Update-Working-Folder")), UpdateWorkingFolder));
        }
        public static void RegisterVersionArgument()
        {
            string programName = ProgramInfo.GetProgramName();
            CommandLineProtoRegistry.Instance.Register(new CommandLineProto(programName, new UsageProto($"{programName} --version"), Version,
                new CommandArgProto(Names.VersionCommand, CommandArgProto.NoMnemonic, new Usage("Get program version"), null, null, CommandArgOptions.IsRequired))
                );
        }
        public static void RegisterHelpArgument(bool hasCommands = true)
        {
            string programName = ProgramInfo.GetProgramName();
            if(hasCommands)
                CommandLineProtoRegistry.Instance.Register(new CommandLineProto(programName, new UsageProto($"{programName} --help [command]"), HelpArgument,
                    new CommandArgProto(Names.HelpCommand, Names.HelpMnemonic, new Usage("Get program help"), null, null, CommandArgOptions.IsRequired),
                    new CommandArgProto("command", 1, new UsageWhere("command","Command name")))
                    );
            else
                CommandLineProtoRegistry.Instance.Register(new CommandLineProto(programName, new UsageProto($"{programName} --help"), HelpArgument,
                    new CommandArgProto(Names.HelpCommand, Names.HelpMnemonic, new Usage("Get program help"), null, null, CommandArgOptions.IsRequired))
                    );

        }
        public static CommandResult Null(CommandLineProto proto, CommandLine args)
        {
            return new CommandResult();
        }
        public static CommandResult Version(CommandLineProto proto, CommandLine args)
        {
            string name = ProgramInfo.GetProgramName();
            string version = ProgramInfo.GetProgramVersion();
            string copyright = ProgramInfo.GetCopyright();

            return new CommandResult(true, $"{name} {version} {copyright}");
        }
        public static CommandResult InvalidCommand(CommandLineProto proto, CommandLine args)
        {
            return new CommandResult(false, $"'{args[0].Name}' is not a command.");
        }
        public static CommandResult Help(CommandLineProto proto, CommandLine args)
        {
            string command = args.GetValue(1);
            if (command == null)
                return GeneralHelp(CommandLineProtoRegistry.Instance);
            else
                return CommandHelp(command, CommandLineProtoRegistry.Instance);
        }
        public static CommandResult HelpArgument(CommandLineProto proto, CommandLine args)
        {
            string command = args.GetValue(1);
            string programName = ProgramInfo.GetProgramName();
            if (command == null)
                return GeneralHelp(CommandLineProtoRegistry.Instance);
            else
                return CommandHelp(new CommandLine($"{programName} {command}"), CommandLineProtoRegistry.Instance);
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
            Usage u = CommandLineProto.GetUsage(protos);
            result.AddMessage(u.ToString());
            return result;
        }
        public static CommandResult CommandHelp(string command, CommandLineProtoRegistry commands)
        {
            CommandResult result = new CommandResult();
            CommandLineProto[] protos = commands.Find(command);
            if(protos.Length == 0)
            {
                return new CommandResult(false, "'{command}' is not a command");
            }
            Usage u = CommandLineProto.GetUsage(protos);
            result.AddMessage(u.ToString());
            return result;
        }
        public static CommandResult CommandHelp(CommandLine args, CommandLineProtoRegistry commands)
        {
            CommandResult result = new CommandResult();
            CommandLineProto proto = commands.Find(args);
            if (proto == null)
            { 
                return new CommandResult(false, "Command not found");
            }
            Usage u = CommandLineProto.GetUsage(proto);
            result.AddMessage(u.ToString());
            return result;
        }
        public static CommandResult WorkingFolder(CommandLineProto proto, CommandLine args)
        {
            if(args.Count == 2)
            {
                try
                {
                    string path = args[1].Value;
                    Directory.SetCurrentDirectory(path);
                    GeneralParameters.Instance.WorkingFolder = path;
                    return new CommandResult(true, $"Working folder set to '{path}'");
                }
                catch(Exception ex)
                {
                    return new CommandResult(false, "Unable to set working folder:", ex);
                }
            }
            else
            {
                string cd = Directory.GetCurrentDirectory();
                string path = GeneralParameters.Instance.WorkingFolder;
                if(cd != path)
                {
                    return new CommandResult(false, $"Saved working folder, '{path}', is different than current working folder, '{cd}'." +
                        "Use restore-working-folder or update-working-folder to synchronize.");
                }
                return new CommandResult(true, GeneralParameters.Instance.WorkingFolder);
            }
        }
        public static CommandResult RestoreWorkingFolder(CommandLineProto proto, CommandLine args)
        {
            try
            {
                string path = GeneralParameters.Instance.WorkingFolder;
                Directory.SetCurrentDirectory(path);
                return new CommandResult(true, $"Working folder restored to '{path}'.");
            }
            catch(Exception ex)
            {
                return new CommandResult(false, "Unable to restore working folder.");
            }
        }
        public static CommandResult UpdateWorkingFolder(CommandLineProto proto, CommandLine args)
        {
            string path = Directory.GetCurrentDirectory();
            GeneralParameters.Instance.WorkingFolder = path;
            return new CommandResult(true, $"Working folder updated to '{path}'");
        }
    }
}
