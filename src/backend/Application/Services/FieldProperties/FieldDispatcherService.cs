using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Extensions;
using Domain.Services.FieldProperties;

namespace Application.Services.FieldProperties
{
    public class FieldDispatcherService : IFieldDispatcherService
    {
        private readonly Dictionary<Type, IEnumerable<FieldInfo>> _fieldsCache;

        public FieldDispatcherService()
        {
            _fieldsCache = new Dictionary<Type, IEnumerable<FieldInfo>>();
        }

        public IEnumerable<FieldInfo> GetDtoFields<TDto>()
        {
            Type dtoType = typeof(TDto);
            IEnumerable<FieldInfo> result;
            if (!_fieldsCache.TryGetValue(dtoType, out result))
            {
                result = GetDtoFieldsInner<TDto>();
                _fieldsCache[dtoType] = result;
            }
            return result;
        }

        private IEnumerable<FieldInfo> GetDtoFieldsInner<TDto>()
        {
            var props = typeof(TDto).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(FieldTypeAttribute)));
            foreach (var prop in props)
            {
                var fieldTypeAttr = (FieldTypeAttribute)Attribute.GetCustomAttribute(prop, typeof(FieldTypeAttribute));

                bool isDefault = Attribute.IsDefined(prop, typeof(IsDefaultAttribute));

                bool isIgnoredForFieldSettings = Attribute.IsDefined(prop, typeof(IgnoreFieldSettingsAttribute));

                bool isBulkUpdateAllowed = Attribute.IsDefined(prop, typeof(AllowBulkUpdateAttribute));

                int orderNumber = int.MaxValue;
                if (Attribute.IsDefined(prop, typeof(OrderNumberAttribute)))
                {
                    var orderNumberAttr = (OrderNumberAttribute)Attribute.GetCustomAttribute(prop, typeof(OrderNumberAttribute));
                    orderNumber = orderNumberAttr.Value;
                }

                var fieldInfo = new FieldInfo
                {
                    Name = prop.Name,
                    FieldType = fieldTypeAttr.Type,
                    ReferenceSource = fieldTypeAttr.Source,
                    ShowRawReferenceValue = fieldTypeAttr.ShowRawValue,
                    OrderNumber = orderNumber,
                    IsDefault = isDefault,
                    IsIgnoredForFieldSettings = isIgnoredForFieldSettings,
                    IsBulkUpdateAllowed = isBulkUpdateAllowed
                };
                yield return fieldInfo;
            }
        }
    }
}