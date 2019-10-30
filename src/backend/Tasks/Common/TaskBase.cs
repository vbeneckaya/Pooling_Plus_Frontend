using Domain.Services.TaskProperties;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks.Common
{
    public abstract class TaskBase<TArgs> where TArgs : PropertiesBase
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly IConfiguration _configuration;

        public TaskBase(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public string TaskName
        {
            get
            {
                string taskSuffix = "Task";
                string result = GetType().Name;
                if (result.EndsWith(taskSuffix))
                {
                    result = result.Substring(0, result.Length - taskSuffix.Length);
                }
                return result;
            }
        }

        public async Task Execute(string consoleParameters, CancellationToken cancellationToken)
        {
            var parameters = LoadParameters(consoleParameters);
            foreach (var param in parameters)
            {
                string args = JsonConvert.SerializeObject(param);
                Log.Logger.Information("Выполнение задачи {TaskName} с параметрами {args}...", TaskName, args);

                await Execute(param, cancellationToken);
            }
        }

        protected abstract Task Execute(TArgs parameters, CancellationToken cancellationToken);

        private List<TArgs> LoadParameters(string consoleParameters)
        { 
            var parametersStrList = GetParameters(consoleParameters);
            var parametersList = parametersStrList.Select(CreateProperties).ToList();
            return parametersList;
        }

        private List<string> GetParameters(string consoleParameters)
        {
            var result = new List<string>();

            // 1. Параметры из консоли
            if (!string.IsNullOrEmpty(consoleParameters))
            {
                result.Add(consoleParameters);
            }

            // 2. Параметры из базы
            if (!result.Any())
            {
                var propertiesService = _serviceProvider.GetService<ITaskPropertiesService>();
                var propertiesEntries = propertiesService.GetByTaskName(TaskName);
                result.AddRange(propertiesEntries.Select(p => p.Properties));
            }

            // 3. Параметры из конфига
            if (!result.Any())
            {
                string configParameters = _configuration.GetValue<string>($"{TaskName}:Args", null);
                if (!string.IsNullOrEmpty(configParameters))
                {
                    result.Add(configParameters);
                }
            }

            // Хотя бы пустые передаем, если нигде ничего не указано
            if (!result.Any())
            {
                result.Add(null);
            }

            return result;
        }

        private TArgs CreateProperties(string parameters)
        {
            Type propertiesType = typeof(TArgs);
            var propertiesTypeFields = propertiesType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var isPropertiesTypeValid = propertiesTypeFields.All(pi => pi.PropertyType == typeof(string));
            if (!isPropertiesTypeValid)
                throw new NotSupportedException($"Тип параметров {propertiesType.Name} не " +
                                                "должен содержать свойств, тип которых отличается от System.String");

            var properties = (TArgs)Activator.CreateInstance(propertiesType);
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
    }
}
