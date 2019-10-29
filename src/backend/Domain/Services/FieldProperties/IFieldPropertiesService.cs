using System;
using System.Collections.Generic;
using Domain.Shared;

namespace Domain.Services.FieldProperties
{
    public interface IFieldPropertiesService
    {
        IEnumerable<FieldForFieldProperties> GetFor(string forEntity, Guid? companyId, Guid? roleId, Guid? userId);
        ValidateResult Save(FieldPropertyDto fieldPropertiesDto);
    }
}