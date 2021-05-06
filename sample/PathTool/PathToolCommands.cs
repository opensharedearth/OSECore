﻿using OSECommand;
using OSEConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PathTool
{
    public class PathToolCommands
    {
        public struct Names 
        {
            public const string ProgramName = "pathtool";
            public const string ListCommand = "list";
            public const string AddCommand = "add";
            public const string RemoveCommand = "remove";
            public const string FilterSwitch = "filter";
            public const char FilterMnemonic = 'f';
            public const string VerboseSwitch = "verbose";
            public const char VerboseMnemonic = 'v';
            public const string SortSwitch = "sort";
            public const string MachineSwitch = "machine";
            public const char MachineMnemonic = 'm';
            public const string UserSwitch = "user";
            public const char UserMnemonic = 'u';
            public const string CleanCommand = "clean";
            public const string QuietSwitch = "quiet";
            public const char QuietMnemonic = 'q';
            public const string InlineSwitch = "inline";
            public const char InlineMnemonic = 'i';
            public const string MoveCommand = "move";
            public const string PositionSwitch = "position";
            public const char PositionMnemonic = 'p';
            public const string LengthSwitch = "length";
            public const char LengthMnemonic = 'l';
            public const string ToSwitch = "to";
            public const char ToMnemonic = 't';
            public const string FolderArgument = "folder";
        }
        public static void RegisterAll()
        {
            var pathArg = new CommandArgProto(Names.ProgramName, 0);
            var filterArg = new CommandArgProto(Names.FilterSwitch, Names.FilterMnemonic,
                new Usage("Filter list of folders with expression",
                    new UsageWhere("exp", "Regular expression")),
                null, null, CommandArgOptions.HasArgument);
            var positionArg = new CommandArgProto(Names.PositionSwitch, Names.PositionMnemonic,
                new Usage("Starting position of folder", new UsageWhere("pos", "position of folder; default = 1")),
                "1", new ValueValidator<int>(), CommandArgOptions.HasArgument);
            var lengthArg = new CommandArgProto(Names.LengthSwitch, Names.LengthMnemonic,
                new Usage("Number of path folders from position to include", new UsageWhere("len", "number of folders; default = 1")),
                "1", new ValueValidator<int>(), CommandArgOptions.HasArgument);
            var toArg = new CommandArgProto(Names.ToSwitch, Names.ToMnemonic,
                new Usage("Destination position for move", new UsageWhere("dpos", "1-based destination position")),
                "1", new ValueValidator<int>(), CommandArgOptions.HasArgument);
            var verboseArg = new CommandArgProto(Names.VerboseSwitch, Names.VerboseMnemonic, new Usage("Verbose output. Path elements in table with position and status."));
            var sortArg = new CommandArgProto(Names.SortSwitch, CommandArgProto.NoMnemonic, new Usage("Sort output.  By default the order of the folder is as they appear in PATH"));
            var machineArg = new CommandArgProto(Names.MachineSwitch, Names.MachineMnemonic, new Usage("Use system PATH variable.  By default the local process path is used. Incompatible with -u"));
            var userArg = new CommandArgProto(Names.UserSwitch, Names.UserMnemonic, new Usage("Use user PATH variable.  By default the local process path is used. Incompatible with -m"));
            var quietArg = new CommandArgProto(Names.QuietSwitch, Names.QuietMnemonic, new Usage("Do not confirm operation.  Otherwise Y/N confirmation is required."));
            var inlineArg = new CommandArgProto(Names.InlineSwitch, Names.InlineMnemonic, new Usage("Inline formating.  Creates output suitable for command execution.  Incompatible with -v"));
            CommandLineProtoRegistry.Instance.Register(new CommandLineProto(Names.ProgramName,
                new Usage("pathtool list [--filter exp] [--verbose | --inline] [--machine | --user] [--sort]"),
                ListCommand,
                pathArg,
                new CommandArgProto(Names.ListCommand, 1, new Usage("List elements of path")),
                filterArg,
                verboseArg,
                sortArg,
                machineArg,
                userArg,
                inlineArg
                ));
            CommandLineProtoRegistry.Instance.Register(new CommandLineProto(Names.ProgramName,
                new Usage("pathtool clean [--machine | --user] [--quiet] [--verbose | --inline]"),
                CleanCommand,
                new CommandArgProto(Names.CleanCommand, 1, new Usage("Clean path of invalid and duplicate elements")),
                quietArg,
                machineArg,
                userArg,
                verboseArg,
                inlineArg
                ));
            CommandLineProtoRegistry.Instance.Register(new CommandLineProto(Names.ProgramName,
                new Usage("pathtool add [--machine | --user] [--verbose | --inline] [--position pos] [--quiet] folder1 folder2 ..."),
                AddCommand,
                pathArg,
                new CommandArgProto(Names.AddCommand, 1, new Usage("Add folders to path"), null, null, CommandArgOptions.IsRequired),
                machineArg,
                userArg,
                verboseArg,
                inlineArg,
                positionArg,
                quietArg,
                new CommandArgProto(Names.FolderArgument, 2, new Usage("Folder to add to path"), null, new FolderValidator(), CommandArgOptions.IsRequired | CommandArgOptions.HasMultiple)
                ));
            CommandLineProtoRegistry.Instance.Register(new CommandLineProto(Names.ProgramName,
                new Usage("pathtool remove [--machine | --user] [--verbose | --inline] [--position pos [--length len] | --filter exp] [--quiet]"),
                RemoveCommand,
                pathArg,
                new CommandArgProto(Names.RemoveCommand, 1, new Usage("Remove folders from path"), null, null, CommandArgOptions.IsRequired),
                machineArg,
                userArg,
                verboseArg,
                inlineArg,
                positionArg,
                lengthArg,
                filterArg,
                quietArg
                ));
            CommandLineProtoRegistry.Instance.Register(new CommandLineProto(Names.ProgramName,
                new Usage("pathtool move [--machine | --user] [--verbose | --inline] [--position pos] [--length len] [--distance dist] [--quiet]"),
                MoveCommand,
                new CommandArgProto(Names.MoveCommand, 1, new Usage("Move folders within PATH"), null, null, CommandArgOptions.IsRequired),
                machineArg,
                userArg,
                verboseArg,
                inlineArg,
                positionArg,
                lengthArg,
                toArg,
                quietArg
                ));

        }
        public static PathFolderOptions GetPathFolderOptions(CommandLine args)
        {
            bool isMachine = args.HasSwitch(Names.MachineSwitch, Names.MachineMnemonic);
            bool isUser = args.HasSwitch(Names.UserSwitch, Names.UserMnemonic);
            PathFolderOptions options = PathFolderOptions.Process;
            if (isUser) options = PathFolderOptions.User;
            if (isMachine) options = PathFolderOptions.Machine;
            if (isMachine && isUser)
            {
                throw new ApplicationException($"--{Names.MachineSwitch} and --{Names.UserSwitch} cannot be used together.");
            }
            return options;
        }
        public static CommandResult RemoveCommand(CommandLine args)
        {
            try
            {
                var app = ConsoleApp.Instance;
                var options = GetPathFolderOptions(args);
                bool isVerbose = args.HasSwitch(Names.VerboseSwitch, Names.VerboseMnemonic);
                bool isQuiet = args.HasSwitch(Names.QuietSwitch, Names.QuietMnemonic);
                var folders = new PathFolders(options);
                folders.Fill();
                int pos = args.GetValue<int>(Names.PositionSwitch, Names.PositionMnemonic, 1);
                int len = args.GetValue<int>(Names.LengthSwitch, Names.LengthMnemonic, 1);
                if (pos < 1 || pos + len - 1 > folders.Count)
                    throw new ApplicationException("Position and length outside valid range for remove operation");
                if(!isQuiet)
                {
                    app.WriteError(isVerbose ? folders.ToVerboseString() : folders.ToString());
                    app.WriteError();
                    if (!app.GetConfirmation($"Remove {len} folder(s) from PATH starting at position {pos}?"))
                        return new CommandResult(false, "Remove operation canceled");
                }
                int l = len;
                while(l-- > 0)
                {
                    folders.RemoveAt(pos - 1);
                }
                if (options == PathFolderOptions.Process)
                    app.WriteOut(folders.ToInlineString());
                else
                    folders.Commit();
                return new CommandResult(true, $"{len} folder(s) removed from PATH");
            }
            catch (Exception ex)
            {
                return new CommandResult(false, "Unable to remove folders from PATH", ex);
            }
        }
        public static CommandResult MoveCommand(CommandLine args)
        {
            CommandResult result = new CommandResult();
            try
            {
                var app = ConsoleApp.Instance;
                var options = GetPathFolderOptions(args);
                bool isVerbose = args.HasSwitch(Names.VerboseSwitch, Names.VerboseMnemonic);
                bool isQuiet = args.HasSwitch(Names.QuietSwitch, Names.QuietMnemonic);
                var folders = new PathFolders(options);
                folders.Fill();
                int pos = args.GetValue<int>(Names.PositionSwitch, Names.PositionMnemonic, 1);
                int len = args.GetValue<int>(Names.LengthSwitch, Names.LengthMnemonic, 1);
                int to = args.GetValue<int>(Names.ToSwitch, Names.ToMnemonic, 1);
                if (pos < 1 || pos + len - 1> folders.Count)
                    throw new ApplicationException("Position and length outside valid range for move operation");
                if (to < 1 || to >  folders.Count)
                    throw new ApplicationException("Move to position outside valid range");
                if (pos == to)
                    throw new ApplicationException("The source position and the destination position are the same");
                if (!isQuiet)
                {
                    app.WriteError(isVerbose ? folders.ToVerboseString() : folders.ToString());
                    app.WriteError();
                    if (!app.GetConfirmation($"Move {folders.Count} in PATH starting at position {pos} to position {to}?"))
                        return new CommandResult(false, "Move operation canceled");
                }
                int l = len;
                int p = pos - 1;
                int t = to - 1;
                while (l-- > 0)
                {
                    if (t < p)
                    {
                        folders.Insert(t, folders[p]);
                        folders.RemoveAt(p + 1);
                        ++p;
                        ++t;
                    }
                    else
                    {
                        folders.Insert(t, folders[p]);
                        folders.RemoveAt(p);
                    }
                }
                if (options == PathFolderOptions.Process)
                    app.WriteOut(folders.ToInlineString());
                else
                    folders.Commit();
                return new CommandResult(true, $"{len} folders moved from position {pos} to position {to} in PATH");

            }
            catch (Exception ex)
            {
                return new CommandResult(false, "Unable to move folders in PATH", ex);
            }
        }
        public static CommandResult AddCommand(CommandLine args)
        {
            var app = ConsoleApp.Instance;
            try
            {
                int pos = args.GetValue<int>(Names.PositionSwitch, Names.PositionMnemonic, 1);
                var options = GetPathFolderOptions(args);
                bool isVerbose = args.HasSwitch(Names.VerboseSwitch, Names.VerboseMnemonic);
                bool isQuiet = args.HasSwitch(Names.QuietSwitch, Names.QuietMnemonic);
                var folders = new PathFolders(options);
                folders.Fill();
                var added = new PathFolders(options);
                int parg = 2;
                string path = args.GetResolvedValue<string>(parg++);
                while(path != null)
                {
                    added.Add(new PathFolder(path));
                    path = args.GetResolvedValue<string>(parg++);
                }
                if(!isQuiet)
                {
                    app.WriteError();
                    app.WriteError("Add...");
                    app.WriteError();
                    app.WriteError(added.ToString(), false);
                    app.WriteError();
                    app.WriteError("to...");
                    app.WriteError();
                    app.WriteError(isVerbose ? folders.ToVerboseString() : folders.ToString());
                    app.WriteError();
                    if(!app.GetConfirmation($"at position {pos}"))
                        return new CommandResult(false, "Add folders operation canceled");
                }
                int p = pos - 1;
                foreach(var f in added)
                {
                    folders.Insert(p++, f);
                }
                if (options == PathFolderOptions.Process)
                    app.WriteOut(folders.ToInlineString());
                else
                    folders.Commit();
                return new CommandResult(true, $"{added.Count} folder(s) added to PATH at position {pos}");
            }
            catch (Exception ex)
            {
                return new CommandResult(false, "Unable to add folders to command", ex);
            }
        }
        public static CommandResult CleanCommand(CommandLine args)
        {
            try
            {
                PathFolderOptions options = GetPathFolderOptions(args);
                CommandResult result = new CommandResult();
                PathFolders foriginal = new PathFolders(options);
                PathFolders fvalid = new PathFolders(options);
                PathFolders finvalid = new PathFolders(options);
                bool isVerbose = args.HasSwitch(Names.VerboseSwitch, Names.VerboseMnemonic);
                bool isQuiet = args.HasSwitch(Names.QuietSwitch, Names.QuietMnemonic);
                foriginal.Fill();
                foreach (PathFolder f0 in foriginal)
                {
                    if (f0.IsValid)
                        fvalid.Add(f0);
                    else
                        finvalid.Add(f0);
                }
                if (finvalid.Count == 0)
                {
                    return new CommandResult(true, "No invalid folders in path");
                }
                if (!isQuiet)
                {
                    ConsoleApp.Instance.WriteError(isVerbose ? finvalid.ToVerboseString() : finvalid.ToString());
                    if (!ConsoleApp.Instance.GetConfirmation("Remove folders from PATH?"))
                    {
                        return new CommandResult(false, "User canceled operation");
                    }
                }
                if (options == PathFolderOptions.Process)
                    ConsoleApp.Instance.WriteOut(fvalid.ToInlineString());
                else
                    fvalid.Commit();
                return new CommandResult(true, $"{finvalid.Count} folder(s) removed from PATH");
            }
            catch(Exception ex)
            {
                return new CommandResult(false, "Unable to clean invalid PATH elements", ex);
            }
        }
        public static CommandResult ListCommand(CommandLine args)
        {
            try
            {
                PathFolderOptions options = GetPathFolderOptions(args);
                PathFolders folders = new PathFolders(options);
                folders.Fill();
                var f0 = folders.ToArray();
                string pattern = args.GetResolvedValue<string>(Names.FilterSwitch, Names.FilterMnemonic);
                if (pattern != null)
                {
                    Regex r = new Regex(pattern);
                    f0 = (from f in f0 where r.IsMatch(f.Path) select f).ToArray();
                }
                if (args.HasSwitch(Names.SortSwitch))
                {
                    Array.Sort(f0);
                }
                folders = new PathFolders(f0);
                bool isVerbose = args.HasSwitch(Names.VerboseSwitch, Names.VerboseMnemonic);
                bool isInline = args.HasSwitch(Names.InlineSwitch, Names.InlineMnemonic);
                if(isVerbose && isInline)
                {
                    return new CommandResult(false, "--Verbose and --Inline are incompatible options");
                }
                if(!isInline)
                {
                    ConsoleApp.Instance.WriteOut(isVerbose ? folders.ToVerboseString() : folders.ToString());
                }
                else
                {
                    ConsoleApp.Instance.WriteOut(folders.ToInlineString());
                }
                return new CommandResult();
            }
            catch(Exception ex)
            {
                return new CommandResult(false, "Unable to list PATH elements", ex);
            }
        }
    }
}
