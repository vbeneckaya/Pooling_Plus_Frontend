﻿using Application.Shared;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.TaskProperties;
using Domain.Services.UserProvider;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.TaskProperties
{
    public class TaskPropertiesService : DictonaryServiceBase<TaskProperty, TaskPropertyDto>, ITaskPropertiesService
    {
        public TaskPropertiesService(ICommonDataService dataService, IUserProvider userProvider, IServiceProvider serviceProvider) 
            : base(dataService, userProvider, serviceProvider) 
        { }

        public override ValidateResult MapFromDtoToEntity(TaskProperty entity, TaskPropertyDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            entity.TaskName = dto.TaskName;
            entity.Properties = dto.Properties;

            return new ValidateResult(null, entity.Id.ToString());
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

        public IEnumerable<TaskPropertyDto> GetByTaskName(string taskName)
        {
            var entries = _dataService.GetDbSet<TaskProperty>().Where(p => p.TaskName == taskName).ToList();
            var result = entries.Select(MapFromEntityToDto).ToList();
            return result;
        }
    }
}
