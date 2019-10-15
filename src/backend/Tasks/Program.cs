using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace Tasks
{
    class Program
    {
        static int Main(string[] args)
        {
            var culture = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentCulture = culture;

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            if (env != "Debug")
            {
                //disable console
                Console.SetOut(TextWriter.Null);
                Console.SetError(TextWriter.Null);
            }

            return ConsoleShell.ForTasksInThisAssembly().Run(args);
        }
    }
}
