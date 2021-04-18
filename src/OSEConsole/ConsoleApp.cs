using OSELogic.Command;
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
            var a = Assembly.GetEntryAssembly();
            return a.GetName().Name;
        }

        protected virtual int Run()
        {
            if(CommandLine.HasSwitch(StandardCommands.Names.VersionCommand))
            {
                CommandResult result = ScriptingEngine.Instance.Execute(StandardCommands.Names.VersionCommand);
                return result.ErrorCode;
            }
            else if(CommandLine.HasSwitch(StandardCommands.Names.HelpCommand))
            {
                CommandResult result = ScriptingEngine.Instance.Execute(StandardCommands.Names.HelpCommand);
                return result.ErrorCode;
            }
            return 0;
        }

        public ConsoleApp(CommandLine args)
        {
            CommandLine = args;
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
        protected virtual void DefineCommands()
        {
            StandardCommands.RegisterAll();
        }
    }
}
