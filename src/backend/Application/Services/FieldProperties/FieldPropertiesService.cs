using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using DAL.Queries;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services.FieldProperties;
using Domain.Services.Orders;
using Domain.Shared;

namespace Application.Services.FieldProperties
{
    public class FieldPropertiesService : IFieldPropertiesService
    {
        private readonly AppDbContext dbContext;
        private readonly IFieldDispatcherService fieldDispatcherService;

        public FieldPropertiesService(AppDbContext dbContext, IFieldDispatcherService fieldDispatcherService)
        {
            this.dbContext = dbContext;
            this.fieldDispatcherService = fieldDispatcherService;
        }
        public FieldPropertiesSelectors GetSelectors(Guid? companyId, Guid? roleId)
        {
            var companies = dbContext.TransportCompanies.ToList().Select(x => new FieldMatrixSelectorItem { Id = x.Id.ToString(), Name = x.Title }).ToList();
            var roles = dbContext.Roles.ToList().Select(x => new FieldMatrixSelectorItem { Id = x.Id.ToString(), Name = x.Name }).ToList();

            companies.Insert(0, new FieldMatrixSelectorItem { Id = "all", Name = "any_company" });
            roles.Insert(0, new FieldMatrixSelectorItem { Id = "all", Name = "any_role" });

            return new FieldPropertiesSelectors
            {
                
                Companies = companies,
                Roles = roles,
                CompanyId = companyId == null ? "all" : companyId.ToString(),
                RoleId = roleId == null ? "all" : roleId.ToString(),
            };
        }

        public IEnumerable<FieldForFieldProperties> GetFor(string forEntity, Guid? companyId, Guid? roleId, Guid? userId)
        {
            var result = new List<FieldForFieldProperties>();

            var forEntityType = Enum.Parse<FieldPropertiesForEntityType>(forEntity);

            IEnumerable<FieldForFieldProperties> allAvailableFieldsDto = new List<FieldForFieldProperties>();
            
            allAvailableFieldsDto = fieldDispatcherService.GetAllAvailableFieldsFor(forEntityType, companyId, roleId, userId);

            Array states = forEntityType == FieldPropertiesForEntityType.Order
                ? Enum.GetValues(typeof(OrderState))
                : Enum.GetValues(typeof(ShippingState));

            if (userId != null)
                roleId = dbContext.Users.GetById(userId.Value).RoleId;

            var fieldMatrixItems = dbContext.FieldPropertyItems.Where(x => x.ForEntity == forEntityType);

            foreach (FieldForFieldProperties availableFieldsDto in allAvailableFieldsDto)
            {
                var accessTypes = new List<FieldPropertiesAccessTypeDto>();
                foreach (var state in states)
                {
                    var fieldMatrixItem =
                        fieldMatrixItems.Where(x =>
                            x.State == state &&
                            x.FieldName == availableFieldsDto.Name
                            && (x.RoleId == roleId || x.RoleId == null)
                            && (x.CompanyId == companyId || x.CompanyId == null)
                            ).OrderBy(x => x).FirstOrDefault();

                    accessTypes.Add(new FieldPropertiesAccessTypeDto
                    {
                        State = state.ToString(),
                        AccessType = fieldMatrixItem?.AccessType.ToString() ?? FieldPropertiesAccessType.Show.ToString()
                    });

                }
                availableFieldsDto.FieldPropertiesAccessTypes = accessTypes;
                result.Add(availableFieldsDto);
            }

            return result;
        }

        public ValidateResult Save(FieldPropertiesDto dto)
        {
            var forEntity = Enum.Parse<FieldPropertiesForEntityType>(dto.ForEntity);

            var fieldMatrixItems = dbContext.FieldPropertyItems.ToList()
                .Where(x => x.ForEntity == forEntity);

            var companyId = dto.CompanyId == "all"
                ? (Guid?)null
                : Guid.Parse(dto.CompanyId);

            var roleId = dto.RoleId == "all"
                ? (Guid?)null
                : Guid.Parse(dto.RoleId);

            foreach (var fieldMatrixAccessType in dto.Fields.ElementAt(0).FieldPropertiesAccessTypes)
            {
                var fieldName = dto.Fields.ElementAt(0).Name;

                var candidate = fieldMatrixItems.FirstOrDefault(x =>
                    x.CompanyId == companyId &&
                    x.RoleId == roleId &&
                    x.FieldName == fieldName &&
                    x.ForEntity == forEntity &&
                    (
                        forEntity == FieldPropertiesForEntityType.Order ? 
                            x.State == Enum.Parse<OrderState>(fieldMatrixAccessType.State).ToString() : 
                            x.State == Enum.Parse<ShippingState>(fieldMatrixAccessType.State).ToString())
                    );

                FieldPropertyItem fieldPropertyItem;
                if (candidate != null)
                {
                    fieldPropertyItem = candidate;
                }
                else
                {
                    fieldPropertyItem = new FieldPropertyItem
                    {
                        CompanyId = companyId,
                        RoleId = roleId,
                        FieldName = fieldName,
                        ForEntity = forEntity
                    };
                    if (forEntity == FieldPropertiesForEntityType.Order)
                        fieldPropertyItem.State = Enum.Parse<OrderState>(fieldMatrixAccessType.State).ToString();
                    else
                        fieldPropertyItem.State = Enum.Parse<ShippingState>(fieldMatrixAccessType.State).ToString();
                }

                fieldPropertyItem.AccessType = Enum.Parse<FieldPropertiesAccessType>(fieldMatrixAccessType.AccessType);

                dbContext.FieldPropertyItems.Add(fieldPropertyItem);
                dbContext.SaveChanges();
            }

            return new ValidateResult { Error = "" };
        }
    }
}