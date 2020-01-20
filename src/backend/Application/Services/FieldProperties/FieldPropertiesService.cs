using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.FieldProperties;
using Domain.Services.Orders;
using Domain.Services.Shippings;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;
using Newtonsoft.Json;

namespace Application.Services.FieldProperties
{
    public class FieldPropertiesService : IFieldPropertiesService
    {
        private readonly ICommonDataService _dataService;
        private readonly IFieldDispatcherService _fieldDispatcherService;
        private readonly IUserProvider _userProvider;
        private static readonly string ShowIdentifier = FieldPropertiesAccessType.Show.ToString().ToLowerFirstLetter();
        private IFieldPropertiesService _fieldPropertiesServiceImplementation;

        public FieldPropertiesService(ICommonDataService dataService, IFieldDispatcherService fieldDispatcherService,
            IUserProvider userProvider)
        {
            _dataService = dataService;
            _fieldDispatcherService = fieldDispatcherService;
            _userProvider = userProvider;
        }

        public IEnumerable<FieldForFieldProperties> GetFor(string forEntity, Guid? companyId, Guid? roleId,
            Guid? userId)
        {
            var result = new List<FieldForFieldProperties>();

            var forEntityType = Enum.Parse<FieldPropertiesForEntityType>(forEntity, true);

            Array states = forEntityType == FieldPropertiesForEntityType.Shippings ||
                           forEntityType == FieldPropertiesForEntityType.RoutePoints
                ? Enum.GetValues(typeof(ShippingState))
                : Enum.GetValues(typeof(OrderState));

            if (userId != null)
                roleId = _dataService.GetById<User>(userId.Value)?.RoleId;

            string lang = _userProvider.GetCurrentUser()?.Language;

            var fieldMatrixItems = _dataService.GetDbSet<FieldPropertyItem>()
                .Where(x => x.ForEntity == forEntityType
                            && (x.RoleId == roleId || x.RoleId == null)
                            && (x.CompanyId == companyId || x.CompanyId == null))
                .ToList();

            var fieldVisibilities = _dataService.GetDbSet<FieldPropertyItemVisibility>()
                .Where(x => x.ForEntity == forEntityType
                            && (x.RoleId == roleId || x.RoleId == null)
                            && (x.CompanyId == companyId || x.CompanyId == null))
                .ToList();

            var fieldNames = GetFieldNames(forEntityType);
            foreach (var fieldName in fieldNames)
            {
                var accessTypes = new Dictionary<string, string>();

                var visibilitySetting = fieldVisibilities.SingleOrDefault(x => x.FieldName == fieldName.Name);

                var isHidden = visibilitySetting?.IsHidden ?? false;

                foreach (var state in states)
                {
                    var stateName = state.ToString()?.ToLowerFirstLetter();
                    if (isHidden)
                        accessTypes[stateName] = ShowIdentifier;
                    else
                    {
                        var stateId = (int) state;

                        var fieldMatrixItem =
                            fieldMatrixItems.Where(x => x.State == stateId && x.FieldName == fieldName.Name)
                                .OrderBy(x => x)
                                .FirstOrDefault();

                        var accessType = fieldMatrixItem?.AccessType.ToString()?.ToLowerFirstLetter()
                                         ?? ShowIdentifier;

                        if (!string.IsNullOrEmpty(stateName))
                        {
                            accessTypes[stateName] = accessType;
                        }
                    }
                }

                result.Add(new FieldForFieldProperties
                {
                    FieldName = fieldName.Name,
                    DisplayName = fieldName.DisplayNameKey.Translate(lang),
                    AccessTypes = accessTypes,
                    isReadOnly = fieldName.IsReadOnly,
                    isHidden = isHidden
                });
            }

            return result;
        }

        public string GetAccessTypeForField(GetForFieldPropertyParams dto)
        {
            var roleId = _userProvider.GetCurrentUser().RoleId;

            var forEntity = Enum.Parse<FieldPropertiesForEntityType>(dto.ForEntity, true);

            int state = forEntity == FieldPropertiesForEntityType.Shippings ||
                        forEntity == FieldPropertiesForEntityType.RoutePoints
                ? (int) Enum.Parse<ShippingState>(dto.State, true)
                : (int) Enum.Parse<OrderState>(dto.State, true);

            var fieldMatrixItem = _dataService.GetDbSet<FieldPropertyItem>()
                .Where(x => x.ForEntity == forEntity
                            && x.FieldName == dto.FieldName
                            && x.State == state
                            && (x.RoleId == roleId || x.RoleId == null))
                .OrderBy(x => x)
                .FirstOrDefault();

            var accessType = fieldMatrixItem?.AccessType ?? FieldPropertiesAccessType.Show;
            return accessType.ToString().ToLowerFirstLetter();
        }

        public IEnumerable<string> GetAvailableFields(
            FieldPropertiesForEntityType forEntityType,
            Guid? companyId,
            Guid? roleId,
            Guid? userId)
        {
            var hiddenAccessType = FieldPropertiesAccessType.Hidden.ToString().ToLowerFirstLetter();
            var fieldProperties = GetFor(forEntityType.ToString(), companyId, roleId, userId)
                .Where(x => !x.isHidden);
            foreach (var prop in fieldProperties)
            {
                bool hasAccess = prop.AccessTypes.Any(x => x.Value != hiddenAccessType);
                if (hasAccess)
                {
                    yield return prop.FieldName;
                }
            }
        }

        public IEnumerable<string> GetReadOnlyFields(
            FieldPropertiesForEntityType forEntityType,
            string stateName,
            Guid? companyId,
            Guid? roleId,
            Guid? userId)
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

        public FieldPropertiesAccessType GetFieldAccess(
            FieldPropertiesForEntityType forEntityType,
            int state,
            string fieldName,
            Guid? companyId,
            Guid? roleId,
            Guid? userId)
        {
            var fieldMatrixItem = _dataService.GetDbSet<FieldPropertyItem>()
                .Where(x => x.ForEntity == forEntityType
                            && x.FieldName == fieldName
                            && x.State == state
                            && (x.RoleId == roleId || x.RoleId == null)
                            && (x.CompanyId == companyId || x.CompanyId == null))
                .OrderBy(x => x)
                .FirstOrDefault();
            return fieldMatrixItem?.AccessType ?? FieldPropertiesAccessType.Show;
        }

        public ValidateResult ToggleHiddenState(ToggleHiddenStateDto dto)
        {
            var dbSet = _dataService.GetDbSet<FieldPropertyItemVisibility>();

            var forEntity = Enum.Parse<FieldPropertiesForEntityType>(dto.ForEntity, true);

            var companyId = string.IsNullOrEmpty(dto.CompanyId)
                ? (Guid?) null
                : Guid.Parse(dto.CompanyId);

            var roleId = string.IsNullOrEmpty(dto.RoleId)
                ? (Guid?) null
                : Guid.Parse(dto.RoleId);


            var visibilityItem = dbSet.SingleOrDefault(x => x.ForEntity == forEntity
                                                            && x.RoleId == roleId
                                                            && x.FieldName == dto.FieldName);

            if (visibilityItem == null)
            {
                visibilityItem = new FieldPropertyItemVisibility
                {
                    Id = Guid.NewGuid(),
                    ForEntity = forEntity,
                    CompanyId = companyId,
                    RoleId = roleId,
                    FieldName = dto.FieldName,
                    IsHidden = true
                };
                dbSet.Add(visibilityItem);
            }
            else
                visibilityItem.IsHidden = !visibilityItem.IsHidden;

            _dataService.SaveChanges();

            return new ValidateResult();
        }

        private void SetHiddenState(FieldPropertyDto dto, bool isHidden)
        {
            var dbSet = _dataService.GetDbSet<FieldPropertyItemVisibility>();

            var forEntity = Enum.Parse<FieldPropertiesForEntityType>(dto.ForEntity, true);

            var companyId = string.IsNullOrEmpty(dto.CompanyId)
                ? (Guid?) null
                : Guid.Parse(dto.CompanyId);

            var roleId = string.IsNullOrEmpty(dto.RoleId)
                ? (Guid?) null
                : Guid.Parse(dto.RoleId);


            var visibilityItem = dbSet.SingleOrDefault(x => x.ForEntity == forEntity
                                                            && x.RoleId == roleId
                                                            && x.FieldName == dto.FieldName);

            if (visibilityItem == null && isHidden)
            {
                visibilityItem = new FieldPropertyItemVisibility
                {
                    Id = Guid.NewGuid(),
                    ForEntity = forEntity,
                    CompanyId = companyId,
                    RoleId = roleId,
                    FieldName = dto.FieldName,
                    IsHidden = isHidden
                };
                dbSet.Add(visibilityItem);
            }
            else if (visibilityItem != null)
                visibilityItem.IsHidden = isHidden;

            _dataService.SaveChanges();
        }

        public bool Import(FileStream stream, FieldPropertiesGetForParams props)
        {
            byte[] bytes = new byte[stream.Length];
            int numBytesToRead = (int) stream.Length;
            int numBytesRead = 0;
            while (numBytesToRead > 0)
            {
                int n = stream.Read(bytes, numBytesRead, numBytesToRead);
                if (n == 0)
                    break;
                numBytesRead += n;
                numBytesToRead -= n;
            }

            var res = Encoding.UTF8.GetString(bytes);
      //      res = "[{\"FieldName\":\"status\",\"DisplayName\":\"Статус накладной\",\"isHidden\":false,\"isReadOnly\":true,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"orderNumber\",\"DisplayName\":\"Накладная\",\"isHidden\":false,\"isReadOnly\":true,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"clientOrderNumber\",\"DisplayName\":\"Номер заказа клиента\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"orderDate\",\"DisplayName\":\"Дата заказа\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"orderType\",\"DisplayName\":\"Тип накладной\",\"isHidden\":false,\"isReadOnly\":true,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"payer\",\"DisplayName\":\"Плательщик\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"clientId\",\"DisplayName\":\"Клиент\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"temperatureMin\",\"DisplayName\":\"Терморежим мин. °C\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"temperatureMax\",\"DisplayName\":\"Терморежим макс. °C\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"shippingDate\",\"DisplayName\":\"Плановая дата отгрузки\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"transitDays\",\"DisplayName\":\"Leadtime\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"deliveryDate\",\"DisplayName\":\"Плановая дата доставки\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"articlesCount\",\"DisplayName\":\"Количество артикулов\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"boxesCount\",\"DisplayName\":\"Плановое количество коробов\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"confirmedBoxesCount\",\"DisplayName\":\"Фактическое количество коробов\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"edit\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"palletsCount\",\"DisplayName\":\"Плановое количество паллет\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"confirmedPalletsCount\",\"DisplayName\":\"Подтвержденное количество паллет\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"actualPalletsCount\",\"DisplayName\":\"Фактическое количество паллет\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"weightKg\",\"DisplayName\":\"Плановый вес, кг\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"actualWeightKg\",\"DisplayName\":\"Фактический вес, кг\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"orderAmountExcludingVAT\",\"DisplayName\":\"Сумма накладной, без НДС\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"shippingCity\",\"DisplayName\":\"Город отгрузки\",\"isHidden\":false,\"isReadOnly\":true,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"deliveryRegion\",\"DisplayName\":\"Регион доставки\",\"isHidden\":false,\"isReadOnly\":true,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"deliveryCity\",\"DisplayName\":\"Город доставки\",\"isHidden\":false,\"isReadOnly\":true,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"shippingStatus\",\"DisplayName\":\"Статус отгрузки\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"deliveryStatus\",\"DisplayName\":\"Статус доставки\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"shippingAvisationTime\",\"DisplayName\":\"Время авизации на складе отгрузки\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"clientAvisationTime\",\"DisplayName\":\"Время авизации у клиента\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"orderComments\",\"DisplayName\":\"Комментарии по накладной\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"pickingTypeId\",\"DisplayName\":\"Тип комплектации\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"loadingArrivalTime\",\"DisplayName\":\"Дата и время прибытия на загрузку факт\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"loadingDepartureTime\",\"DisplayName\":\"Дата и время убытия с загрузки факт\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"unloadingArrivalDate\",\"DisplayName\":\"Дата и время прибытия на выгрузку факт\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"unloadingArrivalTime\",\"DisplayName\":\"Время прибытия к грузополучателю\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"unloadingDepartureDate\",\"DisplayName\":\"Фактическая дата убытия от грузополучателя\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"unloadingDepartureTime\",\"DisplayName\":\"Время убытия от грузополучателя\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"trucksDowntime\",\"DisplayName\":\"Кол-во часов простоя машин\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"returnInformation\",\"DisplayName\":\"Информация по возвратам\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"returnShippingAccountNo\",\"DisplayName\":\"№ счета за перевозку возврата\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"plannedReturnDate\",\"DisplayName\":\"Плановый срок возврата\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"actualReturnDate\",\"DisplayName\":\"Фактический срок возврата\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"majorAdoptionNumber\",\"DisplayName\":\"Номер приемного акта Мейджор\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"orderCreationDate\",\"DisplayName\":\"Дата создания\",\"isHidden\":false,\"isReadOnly\":true,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"waybillTorg12\",\"DisplayName\":\"Товарная накладная(Торг-12)\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"invoice\",\"DisplayName\":\"Счет-фактура\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"documentsReturnDate\",\"DisplayName\":\"Плановая дата возврата документов\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"actualDocumentsReturnDate\",\"DisplayName\":\"Фактическая дата возврата документов\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"shippingNumber\",\"DisplayName\":\"Перевозка\",\"isHidden\":false,\"isReadOnly\":true,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"orderShippingStatus\",\"DisplayName\":\"Статус перевозки\",\"isHidden\":false,\"isReadOnly\":true,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"shippingWarehouseId\",\"DisplayName\":\"Адрес отгрузки\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"deliveryWarehouseId\",\"DisplayName\":\"Адрес доставки\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"orderChangeDate\",\"DisplayName\":\"Время изменения накладной\",\"isHidden\":false,\"isReadOnly\":true,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"orderConfirmed\",\"DisplayName\":\"Накладная подтверждена\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"documentReturnStatus\",\"DisplayName\":\"Статус возврата документов\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"pickingFeatures\",\"DisplayName\":\"Особенности комплектации\",\"isHidden\":false,\"isReadOnly\":true,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"carrierId\",\"DisplayName\":\"Транспортная компания\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"deliveryType\",\"DisplayName\":\"Способ доставки\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"deviationsComment\",\"DisplayName\":\"Комментарий (причины отклонения от графика)\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"deliveryCost\",\"DisplayName\":\"Базовая стоимость, без НДС\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"actualDeliveryCost\",\"DisplayName\":\"Фактическая стоимость, без НДС\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"tarifficationType\",\"DisplayName\":\"Способ тарификации\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"vehicleTypeId\",\"DisplayName\":\"Тип ТС\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"companyId\",\"DisplayName\":\"Юр. лицо\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"productTypeId\",\"DisplayName\":\"productTypeId\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}},{\"FieldName\":\"itemsNumber\",\"DisplayName\":\"itemsNumber\",\"isHidden\":false,\"isReadOnly\":false,\"AccessTypes\":{\"created\":\"show\",\"canceled\":\"show\",\"inShipping\":\"show\",\"shipped\":\"show\",\"delivered\":\"show\",\"archive\":\"show\",\"fullReturn\":\"show\",\"lost\":\"show\"}}]";
            try
            {
                var fieldProperties = JsonConvert.DeserializeObject<IEnumerable<FieldForFieldProperties>>(res);
                foreach (var prop in fieldProperties)
                {
                    SetHiddenState(new FieldPropertyDto()
                    {
                        ForEntity = props.ForEntity,
                        FieldName = prop.FieldName,
                        RoleId = props.RoleId,
                        CompanyId = props.CompanyId
                    }, prop.isHidden);
                    foreach (var accessType in prop.AccessTypes)
                    {
                        Save(new FieldPropertyDto
                        {
                            ForEntity = props.ForEntity,
                            FieldName = prop.FieldName,
                            RoleId = props.RoleId,
                            CompanyId = props.CompanyId,
                            AccessType = accessType.Value,
                            State = accessType.Key
                        });
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public ValidateResult Save(FieldPropertyDto dto)
        {
            var dbSet = _dataService.GetDbSet<FieldPropertyItem>();

            var forEntity = Enum.Parse<FieldPropertiesForEntityType>(dto.ForEntity, true);

            var companyId = string.IsNullOrEmpty(dto.CompanyId)
                ? (Guid?) null
                : Guid.Parse(dto.CompanyId);

            var roleId = string.IsNullOrEmpty(dto.RoleId)
                ? (Guid?) null
                : Guid.Parse(dto.RoleId);

            var entities = dbSet.Where(x => x.ForEntity == forEntity
                                            && x.RoleId == roleId
                                            && x.CompanyId == companyId
                                            && x.FieldName == dto.FieldName)
                .ToList();

            var states = GetStates(forEntity, dto.State);

            foreach (var state in states)
            {
                var stateId = (int) state;
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

        private List<FieldInfo> GetFieldNames(FieldPropertiesForEntityType entityType)
        {
            switch (entityType)
            {
                case FieldPropertiesForEntityType.Orders:
                    return ExtractFieldNamesFromDto<OrderDto>();
                case FieldPropertiesForEntityType.OrderItems:
                    return ExtractFieldNamesFromDto<OrderItemDto>();
                case FieldPropertiesForEntityType.RoutePoints:
                    return ExtractFieldNamesFromDto<RoutePointDto>();
                case FieldPropertiesForEntityType.Shippings:
                    return ExtractFieldNamesFromDto<ShippingDto>();
                default:
                    return new List<FieldInfo>();
            }
        }

        private static Array GetStates(FieldPropertiesForEntityType entityType, string state = "")
        {
            Array states;
            if (string.IsNullOrEmpty(state))
            {
                states = entityType == FieldPropertiesForEntityType.Shippings ||
                         entityType == FieldPropertiesForEntityType.RoutePoints
                    ? Enum.GetValues(typeof(ShippingState))
                    : Enum.GetValues(typeof(OrderState));
            }
            else
            {
                states = new[]
                {
                    entityType == FieldPropertiesForEntityType.Shippings ||
                    entityType == FieldPropertiesForEntityType.RoutePoints
                        ? (int) Enum.Parse<ShippingState>(state, true)
                        : (int) Enum.Parse<OrderState>(state, true)
                };
            }

            return states;
        }

        private List<FieldInfo> ExtractFieldNamesFromDto<TDto>()
        {
            var result = _fieldDispatcherService.GetDtoFields<TDto>()
                .ToList();

            foreach (var fieldInfo in result)
                fieldInfo.Name = fieldInfo.Name?.ToLowerFirstLetter();

            return result;
        }
    }
}