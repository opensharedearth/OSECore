using OSECore.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSECoreExample
{
    static class LoggingSamples
    {
        internal static void Run()
        {
            Console.WriteLine();
            Console.WriteLine("Logging examples");
            Console.WriteLine();
            ResultLogExample();
        }
        internal static void ResultLogExample()
        {
            ResultLog log0 = new ResultLog("First log");
            log0.LogBad("This is an error message.");
            log0.LogSuspect("This is a warning message");
            ResultLog log1 = new ResultLog("Second Log",log0);
            log1.LogGood("This is a good message.");
            Console.Write(log1);
            // Result:
            // Error: This is an error message.
            // Warning: This is a warning message
            // This is a good message.
        }
    }
}
