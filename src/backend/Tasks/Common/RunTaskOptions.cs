using CommandLine;

namespace Tasks
{
    [Verb("run", HelpText = "Выполняет указанную задачу")]
    public class RunTaskOptions
    {
        [Value(0, MetaName = "задача", Required = true, HelpText = "Название задачи")]
        public string TaskName { get; set; }

        [Option('P', Required = false,  HelpText = "Дополнительные параметры")]
        public string Parameters { get; set; }
    }
}
