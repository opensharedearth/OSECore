using OSEConsole;
using OSECommand;
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
            Commands.Define("", new Usage(), StandardCommands.Null);
            Commands.Define("Working-Folder", new Usage("Get or set working folder", new UsageProto("Working-Folder [folder]")), StandardCommands.WorkingFolder,
                new CommandArgProto("Folder", '\0', new Usage("Path to working folder"), "", new FolderValidator())
            );
        }
    }
}
