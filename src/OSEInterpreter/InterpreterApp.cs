using OSEConsole;
using OSELogic.Command;
using System;

namespace OSEInterpreter
{
    public class InterpreterApp : ConsoleApp
    {
        static int Main(string[] args)
        {
            var commandLine = GetCommandLine(args);
            var app = new InterpreterApp(commandLine);
            return app.Run();
        }
        public InterpreterApp(CommandLine args)
            : base(args)
        {

        }
        protected override void DefineCommands()
        {
            base.DefineCommands();
            Commands.Define("", "", "", StandardCommands.Null);
            Commands.Define("Working-Folder", "Get or set working folder", "Working-Folder [folder]", StandardCommands.WorkingFolder,
                new CommandArgProto("Folder", "Path to working folder", "", new FolderValidator())
            );
        }
    }
}
