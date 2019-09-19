using CommandLine;

namespace Tasks
{
    [Verb("help-task", HelpText = "Отображает информацию по использованию по использованию указанной задачи")]
    public class HelpTaskOptions
    {
        [Value(0, MetaName = "задача", Required = true, HelpText = "Название задачи")]
        public string TaskName { get; set; }
    }
}
