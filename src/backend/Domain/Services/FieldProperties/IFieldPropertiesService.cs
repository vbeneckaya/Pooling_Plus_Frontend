using Domain.Enums;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace Domain.Services.FieldProperties
{
    public interface IFieldPropertiesService
    {
        IEnumerable<FieldForFieldProperties> GetFor(string forEntity, Guid? roleId, Guid? userId);
        string GetAccessTypeForField(GetForFieldPropertyParams args);
        ValidateResult Save(FieldPropertyDto fieldPropertiesDto);
        bool Import(Stream stream, FieldPropertiesGetForParams props);
        Byte[] Export(IEnumerable<FieldForFieldProperties> data);
        
        IEnumerable<string> GetAvailableFields(FieldPropertiesForEntityType forEntityType, Guid? roleId, Guid? userId);
        IEnumerable<string> GetReadOnlyFields(FieldPropertiesForEntityType forEntityType, string stateName, Guid? roleId, Guid? userId);
        FieldPropertiesAccessType GetFieldAccess(FieldPropertiesForEntityType forEntityType, int state, string fieldName, Guid? roleId, Guid? userId);
        ValidateResult ToggleHiddenState(ToggleHiddenStateDto dto);
    }
}