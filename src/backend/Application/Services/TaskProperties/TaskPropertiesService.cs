using Application.Shared;
using DAL;
using Domain.Persistables;
using Domain.Services.TaskProperties;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.TaskProperties
{
    public class TaskPropertiesService : DictonaryServiceBase<TaskProperty, TaskPropertyDto>, ITaskPropertiesService
    {
        public override void MapFromDtoToEntity(TaskProperty entity, TaskPropertyDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            entity.TaskName = dto.TaskName;
            entity.Properties = dto.Properties;
        }

        public override TaskPropertyDto MapFromEntityToDto(TaskProperty entity)
        {
            return new TaskPropertyDto
            {
                Id = entity.Id.ToString(),
                TaskName = entity.TaskName,
                Properties = entity.Properties
            };
        }

        public override DbSet<TaskProperty> UseDbSet(AppDbContext dbContext)
        {
            return dbContext.TaskProperties;
        }

        public IEnumerable<TaskPropertyDto> GetByTaskName(string taskName)
        {
            var entries = db.TaskProperties.Where(p => p.TaskName == taskName).ToList();
            var result = entries.Select(MapFromEntityToDto).ToList();
            return result;
        }

        public TaskPropertiesService(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
