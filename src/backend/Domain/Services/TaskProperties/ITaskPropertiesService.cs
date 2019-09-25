using Domain.Persistables;
using System.Collections.Generic;

namespace Domain.Services.TaskProperties
{
    public interface ITaskPropertiesService : IDictonaryService<TaskProperty, TaskPropertyDto>
    {
        IEnumerable<TaskPropertyDto> GetByTaskName(string taskName);
    }
}
