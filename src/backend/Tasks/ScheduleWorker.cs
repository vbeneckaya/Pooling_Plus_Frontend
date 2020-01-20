using DAL.Services;
using Domain.Persistables;
using Domain.Services.Translations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NCrontab;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Common;

namespace Tasks
{
    public class ScheduleWorker : BackgroundService
    {
        private bool _isDisposed = false;

        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly List<ScheduledTaskDescriptor> _tasks;

        public ScheduleWorker(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IEnumerable<IScheduledTask> tasks)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;

            CreateLogger();

            string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            Log.Logger.Information("Менеджер задач запущен в режиме {env}", env);

            _tasks = InitializeTasks(configuration, tasks).ToList();
        }

        public override void Dispose()
        {
            if (!_isDisposed)
            {
                Log.CloseAndFlush();
                _isDisposed = true;
            }
            base.Dispose();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            InitializeTranslations();

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var task in _tasks)
                {
                    if (task.IsActive && task.NextRun <= DateTime.Now)
                    {
                        string taskName = task.Task.TaskName;
                        Log.Logger.Information("Начало выполнения задачи {taskName}", taskName);

                        try
                        {
                            using (var scope = _serviceProvider.CreateScope()) 
                            using (LogContext.PushProperty("TaskName", taskName))
                            {
                                await task.Task.Execute(scope.ServiceProvider, task.Parameters, stoppingToken);
                            }
                            Log.Logger.Information("Задача {taskName} завершена успешно", taskName);
                        }
                        catch (Exception ex)
                        {
                            Log.Logger.Error(ex, "Ошибка при выполнении задачи {taskName}", taskName);
                        }

                        if (task.Schedule == null)
                        {
                            task.IsActive = false;
                        }
                        else
                        {
                            task.NextRun = task.Schedule.GetNextOccurrence(DateTime.Now);

                            string nextRun = task.NextRun.ToString("dd.MM.yyyy HH:mm:ss");
                            Log.Logger.Information("Следующий запуск задачи {taskName} запланирован на {nextRun}", taskName, nextRun);
                        }
                    }
                }

                if (!_tasks.Any(t => t.IsActive))
                {
                    break;
                }

                await Task.Delay(1000, stoppingToken);
            }

            Log.Logger.Information("Завершена работа менеджера задач");
        }

        private void InitializeTranslations()
        {
            var db = _serviceProvider.GetService<ICommonDataService>();
            var translations = db.GetDbSet<Translation>().ToList();
            TranslationProvider.FillCache(translations);
        }

        private IEnumerable<ScheduledTaskDescriptor> InitializeTasks(IConfiguration configuration, IEnumerable<IScheduledTask> tasks)
        {
            string runConsoleArg = configuration.GetValue<string>("run", null);
            string argsConsoleArg = configuration.GetValue<string>("args", null);

            foreach (IScheduledTask task in tasks)
            {
                if (!string.IsNullOrEmpty(runConsoleArg) && string.Compare(task.TaskName, runConsoleArg, true) != 0)
                {
                    continue;
                }

                var taskDesc = new ScheduledTaskDescriptor(task);

                if (string.IsNullOrEmpty(runConsoleArg))
                {
                    string schedule = _configuration.GetValue<string>($"{task.TaskName}:Schedule", null);
                    if (string.IsNullOrEmpty(schedule))
                    {
                        schedule = task.Schedule;
                    }

                    taskDesc.Schedule = CrontabSchedule.TryParse(schedule);
                    if (taskDesc.Schedule == null)
                    {
                        continue;
                    }
                    taskDesc.NextRun = taskDesc.Schedule.GetNextOccurrence(DateTime.Now);

                    string firstRun = taskDesc.NextRun.ToString("dd.MM.yyyy HH:mm:ss");
                    Log.Logger.Information("Задача {TaskName} добавлена в план, первый запуск назначен на {firstRun}", task.TaskName, firstRun);
                }
                else
                {
                    taskDesc.Parameters = argsConsoleArg;
                    taskDesc.NextRun = DateTime.Now;
                }

                yield return taskDesc;
            }
        }

        private void CreateLogger()
        {
            Log.Logger = Infrastructure.Logging.LoggerFactory.CreateLogger(_configuration, "Tasks");
        }
    }
}
