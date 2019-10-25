using System;
using System.Collections.Generic;
using Domain.Enums;
using Domain.Services.FieldProperties;

namespace Application.Services.FieldProperties
{
    public class FieldDispatcherService : IFieldDispatcherService
    {
        public IEnumerable<FieldForFieldProperties> GetAllAvailableFieldsFor(FieldPropertiesForEntityType forEntityType, Guid? companyId, Guid? roleId, Guid? userId)
        {
            switch (forEntityType)
            {
                case FieldPropertiesForEntityType.Order:
                    return new List<FieldForFieldProperties>
                    {
                        new FieldForFieldProperties
                        {
                            
                        }
                    };
            }
            
            return new List<FieldForFieldProperties>();
        }
    }
}