using System;
using System.Runtime.Serialization;

namespace Tasks
{
    [Serializable]
    public class TaskRunException : Exception
    {
        public TaskBase Task { get; }

        public TaskRunException(string message, TaskBase task)
            : base(message)
        {
            Task = task;
        }

        public TaskRunException(string message, TaskBase task, Exception inner)
            : base(message, inner)
        {
            Task = task;
        }

        protected TaskRunException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
