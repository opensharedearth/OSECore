using OSECommand;
using OSEConsole;
using OSECore.Program;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("pathtool.Test")]
namespace PathTool
{
    public class Program : ConsoleApp
    {

        internal static int Main(string[] args)
        {
            ProgramInfo.TopLevelAssembly = Assembly.GetExecutingAssembly();
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
