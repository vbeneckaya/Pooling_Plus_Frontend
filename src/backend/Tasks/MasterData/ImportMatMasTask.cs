using System;
using System.ComponentModel;

namespace Tasks.MasterData
{
    [Description("Импорт инжекций MatMas на сихронизацию мастер-данных по продуктам")]
    public class ImportMatMasTask : TaskBase
    {
        public void Execute(ImportMatMasProperties props)
        {
            Console.WriteLine("Run task ImportMatMasTask with options:");
            Console.WriteLine($"FtpHost = {props?.FtpHost ?? "<пусто>"}");
            Console.WriteLine($"FtpLogin = {props?.FtpLogin ?? "<пусто>"}");
            Console.WriteLine($"FtpPassword = {props?.FtpPassword ?? "<пусто>"}");
        }
    }
}
