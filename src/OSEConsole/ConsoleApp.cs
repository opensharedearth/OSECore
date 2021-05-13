using OSECommand;
using OSECore.Program;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OSEConsole
{
    public class ConsoleApp
    {
        public static ConsoleApp Instance { get; private set; } = null;
        public CommandLineProtoRegistry Commands = new CommandLineProtoRegistry();
        public CommandLine CommandLine { get; set; } 
        static int Main(string[] args)
        {
            var commandLine = GetCommandLine(args);
            var app = new ConsoleApp(commandLine);
            return app.Run();
        }

        public static CommandLine GetCommandLine(string[] args)
        {
            string commandName = GetCommandName();
            List<string> list = new List<string>(args);
            list.Insert(0, commandName);
            return new CommandLine(list.ToArray());
        }

        public static string GetCommandName()
        {
            return ProgramInfo.GetProgramName();
        }

        protected virtual int Run()
        {
            CommandResult result = ScriptingEngine.Instance.Execute(CommandLine);
            WriteError(result.ToString());
            return result.ErrorCode;
        }

        public ConsoleApp(CommandLine args)
        {
            CommandLine = args;
            StandardCommands.RegisterVersionArgument();
            StandardCommands.RegisterHelpArgument();
            Instance = this;
        }
        public void WriteError(AggregateException exs)
        {
            foreach (var ex in exs.InnerExceptions)
            {
                WriteError(ex);
            }
        }
        public void WriteError(Exception ex)
        {
            WriteError(ex.Message);
            if (ex.InnerException != null)
                WriteError(ex.InnerException);
        }
        public void WriteError()
        {
            Console.Error.WriteLine();
        }
        public void WriteError(string line, bool addLF = true)
        {
            if (addLF)
                Console.Error.WriteLine(line);
            else
                Console.Error.Write(line);
        }
        public void WriteError(string[] lines)
        {
            foreach (var line in lines)
            {
                WriteError(line);
            }
        }
        public void WriteOut()
        {
            Console.Out.WriteLine();
        }
        public void WriteOut(string line, bool addLF = true)
        {
            if (addLF)
                Console.Out.WriteLine(line);
            else
                Console.Out.Write(line);
        }
        public void WriteOut(string[] lines)
        {
            foreach (var line in lines)
            {
                WriteOut(line);
            }
        }
        protected virtual void DefineCommands()
        {
            StandardCommands.RegisterAll();
        }
        public bool GetConfirmation(string prompt)
        {
            bool? answer = null;
            while(answer == null)
            {
                Console.Error.Write($"{prompt} [Y/N] ");
                string s = Console.ReadLine();
                int deadman = 10;
                if(!String.IsNullOrEmpty(s) && s.Length == 1)
                {
                    if (s[0] == 'Y' || s[0] == 'y') answer = true;
                    if (s[0] == 'N' || s[0] == 'n') answer = false;
                    if (--deadman <= 0) throw new ApplicationException("No valid confirmation after 10 tries");
                }
            }
            return (bool)answer;
        }
    }
}
