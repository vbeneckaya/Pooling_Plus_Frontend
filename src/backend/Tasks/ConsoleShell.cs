using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace Tasks
{
    public class ConsoleShell
    {
        public static ConsoleShell ForTasksInThisAssembly()
        {
            return new ConsoleShell(Assembly.GetCallingAssembly());
        }

        public static ConsoleShell ForTasksInAssemblyOf<T>()
        {
            return new ConsoleShell(typeof(T).Assembly);
        }

        public int Run(string[] args)
        {
            var parser = new Parser(ps =>
            {
                ps.HelpWriter = Console.Out;
                ps.ParsingCulture = new CultureInfo("ru-RU");
                ps.CaseInsensitiveEnumValues = true;
                ps.CaseSensitive = false;
            });

            return parser
                .ParseArguments<ListTasksOptions, HelpTaskOptions, RunTaskOptions>(args)
                .MapResult(
                    (ListTasksOptions opts) => SafeInvoke(() => ListTasks(opts)),
                    (HelpTaskOptions opts) => SafeInvoke(() => HelpTask(opts)),
                    (RunTaskOptions opts) => SafeInvoke(() => RunTask(opts)),
                    errs => 1);
        }

        private static int SafeInvoke(Action action)
        {
            try
            {
                action();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Необработанная ошибка: {ex.Message}");
                return -1;
            }
        }

        private void ListTasks(ListTasksOptions options)
        {
            const string padding = "    ";

            var tasks = taskRepository.Tasks.ToArray();
            var maxNameLength = tasks.Max(t => t.Name.Length);
            var maxNameLenthWithPaddings = maxNameLength + 2 * padding.Length;
            var maxDescriptionLength = System.Console.WindowWidth - maxNameLenthWithPaddings;
            var taskDescriptions = tasks.Select(t =>
            {
                var name = t.Name.PadRight(maxNameLength, ' ');
                var description = t.Description.Split(maxDescriptionLength.ToString())
                    .Select((x, i) => i == 0 ? x : $"{new string(' ', maxNameLenthWithPaddings)}{x}");

                return $"{padding}{name}{padding}" + string.Join("\r\n", description);
            });

            Console.WriteLine("Доступные для выполнения задачи:");
            Console.WriteLine();

            foreach (var taskDescription in taskDescriptions)
            {
                Console.WriteLine(taskDescription);
                Console.WriteLine();
            }
        }

        private void HelpTask(HelpTaskOptions options)
        {
        }

        private void RunTask(RunTaskOptions options)
        {
            var taskName = options.TaskName.EndsWith("Task")
                ? options.TaskName
                : $"{options.TaskName}Task";
            var taskMetadata = taskRepository.Tasks
                .FirstOrDefault(tm => tm.Type.Name == taskName);
            var taskType = taskMetadata?.Type;

            if (taskType == null)
            {
                Console.WriteLine($"Задача {taskName} не найдена");
                return;
            }

            using (var task = (TaskBase)Activator.CreateInstance(taskType))
            {
                task.Run(options.Parameters);
            }
        }

        static ConsoleShell()
        {
            SentenceBuilder.Factory = () => new RussianSentenceBuilder();
        }

        private ConsoleShell(Assembly tasksAssembly)
        {
            Console.OutputEncoding = Encoding.UTF8;

            taskRepository = new TaskRepository(tasksAssembly);
        }

        private readonly TaskRepository taskRepository;
    }
}