using System;
using System.Globalization;
using System.Threading;

namespace Tasks
{
    class Program
    {
        static int Main(string[] args)
        {
            var culture = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentCulture = culture;

            return ConsoleShell.ForTasksInThisAssembly().Run(args);
        }
    }
}
