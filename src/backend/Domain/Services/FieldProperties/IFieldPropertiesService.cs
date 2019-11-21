using Domain.Enums;
using Domain.Shared;
using System;
using System.Collections.Generic;

namespace Domain.Services.FieldProperties
{
    public interface IFieldPropertiesService
    {
        IEnumerable<FieldForFieldProperties> GetFor(string forEntity, Guid? companyId, Guid? roleId, Guid? userId);
        string GetAccessTypeForField(GetForFieldPropertyParams args);
        ValidateResult Save(FieldPropertyDto fieldPropertiesDto);

        IEnumerable<string> GetAvailableFields(FieldPropertiesForEntityType forEntityType, Guid? companyId, Guid? roleId, Guid? userId);
        IEnumerable<string> GetReadOnlyFields(FieldPropertiesForEntityType forEntityType, string stateName, Guid? companyId, Guid? roleId, Guid? userId);
        FieldPropertiesAccessType GetFieldAccess(FieldPropertiesForEntityType forEntityType, int state, string fieldName, Guid? companyId, Guid? roleId, Guid? userId);
        ValidateResult ToggleHiddenState(ToggleHiddenStateDto dto);
    }
}