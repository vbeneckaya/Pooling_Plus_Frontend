using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Enums;
using Domain.Extensions;
using Domain.Services.FieldProperties;
using Domain.Services.Orders;

namespace Application.Services.FieldProperties
{
    public class FieldDispatcherService : IFieldDispatcherService
    {
        public IEnumerable<FieldForFieldProperties> GetAllAvailableFieldsFor(FieldPropertiesForEntityType forEntityType, Guid? companyId, Guid? roleId, Guid? userId)
        {
            switch (forEntityType)
            {
                case FieldPropertiesForEntityType.Order:
                    var props = typeof(OrderDto).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(FieldTypeAttribute)));

                    return props.Select(prop => 
                            new FieldForFieldProperties
                            {
                            }
                         )
                         .ToList();
            }
            
            return new List<FieldForFieldProperties>();
        }
    }
}