using Domain.Enums;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.IO;

namespace Domain.Services.FieldProperties
{
    public interface IFieldPropertiesService
    {
        IEnumerable<FieldForFieldProperties> GetFor(string forEntity, Guid? companyId, Guid? roleId, Guid? userId);
        string GetAccessTypeForField(GetForFieldPropertyParams args);
        ValidateResult Save(FieldPropertyDto fieldPropertiesDto);
        bool Import(FileStream stream, FieldPropertiesGetForParams props);
        
        IEnumerable<string> GetAvailableFields(FieldPropertiesForEntityType forEntityType, Guid? companyId, Guid? roleId, Guid? userId);
        IEnumerable<string> GetReadOnlyFields(FieldPropertiesForEntityType forEntityType, string stateName, Guid? companyId, Guid? roleId, Guid? userId);
        FieldPropertiesAccessType GetFieldAccess(FieldPropertiesForEntityType forEntityType, int state, string fieldName, Guid? companyId, Guid? roleId, Guid? userId);
        ValidateResult ToggleHiddenState(ToggleHiddenStateDto dto);
    }
}