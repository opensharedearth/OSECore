using OSEConsole;
using OSECommand;
using System;
using OSECore.Program;
using System.Threading.Tasks;
using OSEConfig;

namespace OSEInterpreter
{
    public class InterpreterApp : ConsoleApp
    {
        public static int Main(string[] args)
        {
            var commandLine = GetCommandLine(args);
            var app = new InterpreterApp(commandLine);
            return app.Run();
        }
        string _prompt = "";
        public InterpreterApp(CommandLine args)
            : base(args)
        {

        }
        protected override void DefineCommands()
        {
            base.DefineCommands();
            string programName = ProgramInfo.GetProgramName();
            Commands.Register(new CommandLineProto(programName, new Usage(), StandardCommands.Null));
            Commands.Register(new CommandLineProto("", new Usage(), StandardCommands.Null));
            Commands.Register(new CommandLineProto("Working-Folder", new Usage("Get or set working folder", new UsageProto("Working-Folder [folder]")), StandardCommands.WorkingFolder,
                new CommandArgProto("Folder", '\0', new Usage("Path to working folder"), "", new FolderValidator()))
            );
            Commands.Register(new CommandLineProto("exit", new Usage("Exit command interpreter", new UsageProto("Exit")), Exit));
        }
        private bool _exitRequested = false;
        protected CommandResult Exit(CommandLineProto proto, CommandLine args)
        {
            _exitRequested = true;
            return new CommandResult();
        }
        protected override int Run()
        {
            string programName = ProgramInfo.GetProgramName();
            string prompt = $"{programName}> ";
            int error = base.Run();
            if (error == 0)
            {
                ReadLoop(prompt);
            }
            GeneralParameters.Instance.Undirty();
            ConfigFile.Instance.Undirty();
            return error;
        }
        private void ReadLoop(string prompt)
        {
            CommandResult result = new CommandResult();
            while (!_exitRequested)
            {
                string line = ReadIn(prompt);
                result = ProcessCommand(line);
                string messages = result.GetMessages();
                if(messages.Length > 0)WriteError(messages);
            }
        }
        private CommandResult ProcessCommand(string line)
        {
            CommandLine cl = new CommandLine(line);
            if(cl.Count > 0)
            {
                var command = Commands.Find(cl);
                if (command != null)
                {
                    var result = command.Execute(cl);
                    return result;
                }
                return new CommandResult(false, true, 0, "Invalid command.");
            }
            return new CommandResult();
        }
    }
}
