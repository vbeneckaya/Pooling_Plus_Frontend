using Infrastructure.Installers;
using Infrastructure.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Tasks
{
    public abstract class TaskBase : IDisposable
    {
        private const string ExecuteMethodName = "Execute";

        public string TaskName { get; }

        public Guid RunId { get; } = Guid.NewGuid();

        public bool IsCompleted { get; private set; }

        public virtual void Dispose()
        {
        }

        [Test]
        [TestCase((string)null)]
        public void Run(string parameters = null)
        {
            if (IsCompleted)
            {
                throw new TaskRunException($"Попытка повторного выполнения уже выполненной задачи {TaskName}", this);
            }

            var stopwatch = Stopwatch.StartNew();

            try
            {
                var propertiesType = typeof(PropertiesBase);
                var methodInfo = GetType()
                    .GetMethod(ExecuteMethodName, BindingFlags.Instance | BindingFlags.Public);
                var executeParameters = methodInfo.GetParameters()
                    .Select(pi => propertiesType.IsAssignableFrom(pi.ParameterType)
                                ? CreateProperties(pi.ParameterType, parameters)
                                : ServiceProvider.GetService(pi.ParameterType))
                    .ToArray();

                methodInfo.Invoke(this, executeParameters);

                IsCompleted = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to run {TaskName}");
                if (TestContext.CurrentContext.WorkerId != null)
                    Assert.Fail(ex.Message);
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        protected virtual void Initialize()
        {
        }

        protected virtual void CreateContainer()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddDomain(Configuration, false);

            ServiceProvider = services.BuildServiceProvider();
        }

        protected virtual void CreateLogger()
        {
            Log.Logger = LoggerFactory.CreateLogger(Configuration, "Tasks");
        }

        private object CreateProperties(Type propertiesType, string parameters)
        {
            var propertiesTypeFields = propertiesType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var isPropertiesTypeValid = propertiesTypeFields.All(pi => pi.PropertyType == typeof(string));
            if (!isPropertiesTypeValid)
                throw new NotSupportedException($"Тип параметров {propertiesType.Name} не " +
                                                "должен содержать свойств, тип которых отличается от System.String");

            var properties = Activator.CreateInstance(propertiesType);
            var propertiesDictionary = (parameters ?? "").Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(new[] { '=' }, 2))
                .ToDictionary(k => k[0], v => v[1]);

            foreach (var propertyInfo in propertiesTypeFields)
            {
                if (propertiesDictionary.ContainsKey(propertyInfo.Name))
                    propertyInfo.SetValue(properties, propertiesDictionary[propertyInfo.Name]);
            }

            return properties;
        }

        protected TaskBase()
        {
            TaskName = Regex.Replace(GetType().Name, "Task$", string.Empty);
            Initialize();
            CreateLogger();
            CreateContainer();
        }

        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static IServiceProvider ServiceProvider { get; set; }
    }
}
