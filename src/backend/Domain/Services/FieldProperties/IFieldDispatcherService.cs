using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Domain.Services.FieldProperties
{
    public interface IFieldDispatcherService
    {
        IEnumerable<FieldForFieldProperties> GetAllAvailableFieldsFor(FieldPropertiesForEntityType forEntityType, Guid? companyId, Guid? roleId, Guid? userId);
    }
}