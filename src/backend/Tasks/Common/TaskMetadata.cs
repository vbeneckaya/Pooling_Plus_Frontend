using System;

namespace Tasks
{
    public class TaskMetadata
    {
        public string Name { get; }
        public Type Type { get; }
        public string Description { get; }
        public TaskPropertyMetadata[] Properties { get; set; }

        public override string ToString() => Name;

        public TaskMetadata(string name, Type type, string description = null, TaskPropertyMetadata[] properties = null)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            Name = name;
            Type = type;
            Description = description;
            Properties = properties ?? new TaskPropertyMetadata[0];
        }
    }
}
