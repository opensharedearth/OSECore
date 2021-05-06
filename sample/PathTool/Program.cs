using OSECommand;
using OSEConsole;
using System;

namespace PathTool
{
    public class Program : ConsoleApp
    {

        static int Main(string[] args)
        {
            var commandLine = GetCommandLine(args);
            var app = new Program(commandLine);
            return app.Run();
        }
        public Program(CommandLine args)
            : base(args)
        {
            DefineCommands();
        }
        protected override void DefineCommands()
        {
            PathToolCommands.RegisterAll();
        }
    }
}
