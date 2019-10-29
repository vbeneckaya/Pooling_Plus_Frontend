using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.FieldProperties;
using Domain.Services.Orders;
using Domain.Services.Shippings;
using Domain.Shared;

namespace Application.Services.FieldProperties
{
    public class FieldPropertiesService : IFieldPropertiesService
    {
        private readonly ICommonDataService _dataService;
        private readonly IFieldDispatcherService _fieldDispatcherService;

        public FieldPropertiesService(ICommonDataService dataService, IFieldDispatcherService fieldDispatcherService)
        {
            _dataService = dataService;
            _fieldDispatcherService = fieldDispatcherService;
        }

        public IEnumerable<FieldForFieldProperties> GetFor(string forEntity, Guid? companyId, Guid? roleId, Guid? userId)
        {
            var result = new List<FieldForFieldProperties>();

            var forEntityType = Enum.Parse<FieldPropertiesForEntityType>(forEntity, true);

            Array states = forEntityType == FieldPropertiesForEntityType.Shippings
                ? Enum.GetValues(typeof(ShippingState))
                : Enum.GetValues(typeof(OrderState));

            if (userId != null)
                roleId = _dataService.GetById<User>(userId.Value)?.RoleId;

            var fieldMatrixItems = _dataService.GetDbSet<FieldPropertyItem>()
                                               .Where(x => x.ForEntity == forEntityType
                                                        && (x.RoleId == roleId || x.RoleId == null)
                                                        && (x.CompanyId == companyId || x.CompanyId == null))
                                               .ToList();

            var fieldNames = GetFieldNames(forEntityType);
            foreach (string fieldName in fieldNames)
            {
                var accessTypes = new Dictionary<string, string>();
                foreach (var state in states)
                {
                    var stateId = (int)state;
                    var fieldMatrixItem =
                        fieldMatrixItems.Where(x => x.State == stateId && x.FieldName == fieldName)
                                        .OrderBy(x => x)
                                        .FirstOrDefault();

                    var stateName = state.ToString()?.ToLowerFirstLetter();
                    var accessType = fieldMatrixItem?.AccessType.ToString()?.ToLowerFirstLetter()
                                        ?? FieldPropertiesAccessType.Show.ToString().ToLowerFirstLetter();

                    if (!string.IsNullOrEmpty(stateName))
                    {
                        accessTypes[stateName] = accessType;
                    }
                }
                result.Add(new FieldForFieldProperties
                {
                    FieldName = fieldName,
                    AccessTypes = accessTypes
                });
            }

            return result;
        }

        public IEnumerable<string> GetAvailableFields(FieldPropertiesForEntityType forEntityType, Guid? companyId, Guid? roleId, Guid? userId)
        {
            var hiddenAccessType = FieldPropertiesAccessType.Hidden.ToString().ToLowerFirstLetter();
            var fieldProperties = GetFor(forEntityType.ToString(), companyId, roleId, userId);
            foreach (var prop in fieldProperties)
            {
                bool hasAccess = prop.AccessTypes.Any(x => x.Value != hiddenAccessType);
                if (hasAccess)
                {
                    yield return prop.FieldName;
                }
            }
        }

        public IEnumerable<string> GetReadOnlyFields(FieldPropertiesForEntityType forEntityType, string stateName, Guid? companyId, Guid? roleId, Guid? userId)
        {
            var editAccessType = FieldPropertiesAccessType.Edit.ToString().ToLowerFirstLetter();
            var fieldProperties = GetFor(forEntityType.ToString(), companyId, roleId, userId);
            foreach (var prop in fieldProperties)
            {
                bool isReadOnly = true;
                if (prop.AccessTypes.TryGetValue(stateName, out string accessType))
                {
                    isReadOnly = accessType != editAccessType;
                }
                if (isReadOnly)
                {
                    yield return prop.FieldName;
                }
            }
        }

        public ValidateResult Save(FieldPropertyDto dto)
        {
            var dbSet = _dataService.GetDbSet<FieldPropertyItem>();

            var forEntity = Enum.Parse<FieldPropertiesForEntityType>(dto.ForEntity, true);

            var companyId = string.IsNullOrEmpty(dto.CompanyId)
                ? (Guid?)null
                : Guid.Parse(dto.CompanyId);

            var roleId = string.IsNullOrEmpty(dto.RoleId)
                ? (Guid?)null
                : Guid.Parse(dto.RoleId);

            var entities = dbSet.Where(x => x.ForEntity == forEntity
                                        && x.RoleId == roleId
                                        && x.CompanyId == companyId
                                        && x.FieldName == dto.FieldName)
                                .ToList();

            Array states;
            if (string.IsNullOrEmpty(dto.State))
            {
                states = forEntity == FieldPropertiesForEntityType.Shippings
                        ? Enum.GetValues(typeof(ShippingState))
                        : Enum.GetValues(typeof(OrderState));
            }
            else
            {
                int state = forEntity == FieldPropertiesForEntityType.Shippings
                    ? (int)Enum.Parse<ShippingState>(dto.State, true)
                    : (int)Enum.Parse<OrderState>(dto.State, true);
                states = new[] { state };
            }

            foreach (var state in states)
            {
                var stateId = (int)state;
                var entity = entities.Where(x => x.State == stateId).FirstOrDefault();

                if (entity == null)
                {
                    entity = new FieldPropertyItem
                    {
                        Id = Guid.NewGuid(),
                        ForEntity = forEntity,
                        CompanyId = companyId,
                        RoleId = roleId,
                        FieldName = dto.FieldName,
                        State = stateId
                    };
                    dbSet.Add(entity);
                }

                entity.AccessType = Enum.Parse<FieldPropertiesAccessType>(dto.AccessType, true);
            }

            _dataService.SaveChanges();

            return new ValidateResult();
        }

        private List<string> GetFieldNames(FieldPropertiesForEntityType entityType)
        {
            switch(entityType)
            {
                case FieldPropertiesForEntityType.Orders:
                    return ExtractFieldNamesFromDto<OrderDto>();
                case FieldPropertiesForEntityType.OrderItems:
                    return ExtractFieldNamesFromDto<OrderItemDto>();
                case FieldPropertiesForEntityType.Shippings:
                    return ExtractFieldNamesFromDto<ShippingDto>();
                default:
                    return new List<string>();
            }
        }

        private List<string> ExtractFieldNamesFromDto<TDto>()
        {
            var fields = _fieldDispatcherService.GetDtoFields<TDto>();
            var result = fields.Select(x => x.Name?.ToLowerFirstLetter()).ToList();
            return result;
        }
    }
}