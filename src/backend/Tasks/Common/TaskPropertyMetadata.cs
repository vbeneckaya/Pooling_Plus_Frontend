using System;

namespace Tasks
{
    public class TaskPropertyMetadata
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DefaultValue { get; set; }

        public override string ToString() => Name;

        public TaskPropertyMetadata(string name, string description = null, string defaultValue = null)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            Description = description;
            DefaultValue = defaultValue;
        }
    }
}
