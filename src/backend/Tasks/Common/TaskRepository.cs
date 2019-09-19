using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Tasks
{
    public class TaskRepository
    {
        public IEnumerable<TaskMetadata> Tasks
            => tasks ?? (tasks = GetTasks());

        private IEnumerable<TaskMetadata> GetTasks()
        {
            return tasksAssembly.GetTypes()
                .Where(type => typeof(TaskBase).IsAssignableFrom(type) && !type.IsAbstract)
                .Where(type => type.GetCustomAttribute<ObsoleteAttribute>() == null)
                .OrderBy(type => type.Name)
                .Select(type =>
                {
                    var name = Regex.Replace(type.Name, "Task$", string.Empty);
                    var description = type.GetCustomAttribute<DescriptionAttribute>()?.Description;

                    var propertiesType = type.GetNestedTypes()
                        .FirstOrDefault(nestedType => typeof(PropertiesBase).IsAssignableFrom(nestedType));

                    var defaultProperties = propertiesType != null
                        ? Activator.CreateInstance(propertiesType)
                        : null;

                    var properties = propertiesType?.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Select(pi => new TaskPropertyMetadata(
                            pi.Name,
                            pi.GetCustomAttribute<DescriptionAttribute>()?.Description,
                            pi.GetValue(defaultProperties)?.ToString()))
                        .ToArray();

                    return new TaskMetadata(name, type, description, properties);
                })
                .ToArray();
        }

        public TaskRepository(Assembly tasksAssembly)
        {
            this.tasksAssembly = tasksAssembly;
        }

        private readonly Assembly tasksAssembly;
        private IEnumerable<TaskMetadata> tasks;
    }
}
