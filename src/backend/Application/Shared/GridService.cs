using Application.BusinessModels.Shared.Actions;
using Application.Shared.Excel;
using DAL.Queries;
using DAL.Services;
using Domain.Enums;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.FieldProperties;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;
using OfficeOpenXml;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Application.Shared
{
    public abstract class GridService<TEntity, TDto, TFormDto, TSummaryDto, TFilter>: IGridService<TEntity, TDto, TFormDto, TSummaryDto, TFilter>
        where TEntity : class, IPersistable, new() 
        where TDto : IDto, new() 
        where TFormDto : IDto, new()
    {
        public abstract ValidateResult MapFromDtoToEntity(TEntity entity, TDto dto);
        public abstract ValidateResult MapFromFormDtoToEntity(TEntity entity, TFormDto dto);
        public abstract TDto MapFromEntityToDto(TEntity entity);
        public abstract TFormDto MapFromEntityToFormDto(TEntity entity);
        public abstract LookUpDto MapFromEntityToLookupDto(TEntity entity);

        public abstract IEnumerable<EntityStatusDto> LoadStatusData(IEnumerable<Guid> ids);

        public abstract string GetNumber(TFormDto dto);

        public abstract TSummaryDto GetSummary(IEnumerable<Guid> ids);
        public abstract IQueryable<TEntity> ApplySearchForm(IQueryable<TEntity> query, FilterFormDto<TFilter> searchForm);

        protected virtual void ApplyAfterSaveActions(TEntity entity, TDto dto) { }

        protected readonly IUserProvider _userIdProvider;

        protected readonly ICommonDataService _dataService;

        protected readonly IFieldDispatcherService _fieldDispatcherService;

        protected readonly IFieldPropertiesService _fieldPropertiesService;

        protected readonly IEnumerable<IAppAction<TEntity>> _singleActions;

        protected readonly IEnumerable<IGroupAppAction<TEntity>> _groupActions;

        protected GridService(
            ICommonDataService dataService, 
            IUserProvider userIdProvider,
            IFieldDispatcherService fieldDispatcherService,
            IFieldPropertiesService fieldPropertiesService,
            IEnumerable<IAppAction<TEntity>> singleActions,
            IEnumerable<IGroupAppAction<TEntity>> groupActions)
        {
            _userIdProvider = userIdProvider;
            _dataService = dataService;
            _fieldDispatcherService = fieldDispatcherService;
            _fieldPropertiesService = fieldPropertiesService;
            _singleActions = singleActions;
            _groupActions = groupActions;
        }

        public TDto Get(Guid id)
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var entity = _dataService.GetById<TEntity>(id);
            Log.Debug("{entityName}.Get (Load from DB): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var result = MapFromEntityToDto(entity);
            Log.Debug("{entityName}.Get (Convert to DTO): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            return result;
        }

        public TFormDto GetForm(Guid id)
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var entity = _dataService.GetById<TEntity>(id);
            Log.Debug("{entityName}.GetForm (Load from DB): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var result = MapFromEntityToFormDto(entity);
            Log.Debug("{entityName}.GetForm (Convert to DTO): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            return result;
        }

        public IEnumerable<LookUpDto> ForSelect()
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var entries = _dataService.GetDbSet<TEntity>().ToList();
            Log.Debug("{entityName}.ForSelect (Load from DB): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var result = entries.Select(MapFromEntityToLookupDto).ToList();
            Log.Debug("{entityName}.ForSelect (Convert to DTO): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            return result;
        }

        public SearchResult<TDto> Search(FilterFormDto<TFilter> form)
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var dbSet = _dataService.GetDbSet<TEntity>();

            var query = this.ApplySearchForm(dbSet, form);
            Log.Debug("{entityName}.Search (Apply search parameters): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            if (form.Take == 0)
                form.Take = 1000;

            var totalCount = query.Count();
            var entities = query.Skip(form.Skip)
                .Take(form.Take).ToList();
            Log.Debug("{entityName}.Search (Load from DB): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var a = new SearchResult<TDto>
            {
                TotalCount = totalCount,
                Items = entities.Select(entity => MapFromEntityToDto(entity)).ToList()
            };
            Log.Debug("{entityName}.Search (Convert to DTO): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            return a;
        }

        public IEnumerable<string> SearchIds(FilterFormDto<TFilter> form)
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var dbSet = _dataService.GetDbSet<TEntity>();
            
            var query = this.ApplySearchForm(dbSet, form);
            Log.Debug("{entityName}.SearchIds (Apply search parameters): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var ids = query.Select(e => e.Id).ToList();
            Log.Debug("{entityName}.SearchIds (Load from DB): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            var result = ids.Select(x => x.ToString()).ToList();
            return result;
        }

        public ValidateResult SaveOrCreate(TFormDto entityFrom)
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            ValidateResult mapResult;
            var dbSet = _dataService.GetDbSet<TEntity>();
            if (!string.IsNullOrEmpty(entityFrom.Id))
            {
                var entityFromDb = dbSet.GetById(Guid.Parse(entityFrom.Id));
                Log.Debug("{entityName}.SaveOrCreate (Find entity by Id): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
                sw.Restart();

                if (entityFromDb != null)
                {
                    mapResult = MapFromFormDtoToEntity(entityFromDb, entityFrom);
                    Log.Debug("{entityName}.SaveOrCreate (Update fields): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
                    sw.Restart();

                    if (mapResult.IsError)
                    {
                        return mapResult;
                    }

                    dbSet.Update(entityFromDb);
                    
                    _dataService.SaveChanges();
                    Log.Debug("{entityName}.SaveOrCreate (Save changes): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

                    return new ValidateResult
                    {
                        Id = entityFromDb.Id.ToString()
                    };
                }
            }

            var entity = new TEntity
            {
                Id = Guid.NewGuid()
            };

            mapResult = MapFromFormDtoToEntity(entity, entityFrom);
            Log.Debug("{entityName}.SaveOrCreate (Fill fields): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            if (mapResult.IsError)
            {
                return mapResult;
            }

            dbSet.Add(entity);

            _dataService.SaveChanges();
            Log.Debug("{entityName}.SaveOrCreate (Save changes): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            return new ValidateResult
            {
                Id = entity.Id.ToString()
            };
        }

        public IEnumerable<ActionDto> GetActions(IEnumerable<Guid> ids)
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (ids == null) 
                throw new ArgumentNullException(nameof(ids));
            
            var dbSet = _dataService.GetDbSet<TEntity>();
            var currentUser = _userIdProvider.GetCurrentUser();
            var role = currentUser.RoleId.HasValue ? _dataService.GetById<Role>(currentUser.RoleId.Value) : null;
            Log.Debug("{entityName}.GetActions (Load role): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var result = new List<ActionDto>();

            var entities = dbSet.Where(x => ids.Contains(x.Id)).ToList();
            Log.Debug("{entityName}.GetActions (Load data from DB): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            foreach (var action in _singleActions)
            {
                string actionName = action.GetType().Name.ToLowerFirstLetter();
                if (role?.Actions != null && !role.Actions.Contains(actionName))
                {
                    continue;
                }

                var validEntities = entities.Where(e => action.IsAvailable(e));
                if (validEntities.Any())
                {
                    var actionDto = result.FirstOrDefault(x => x.Name == actionName);
                    if (actionDto == null)
                    {
                        result.Add(new ActionDto
                        {
                            Color = action.Color.ToString().ToLowerFirstLetter(),
                            Name = actionName,
                            Ids = validEntities.Select(x => x.Id.ToString())
                        });
                    }
                }
            }
            Log.Debug("{entityName}.GetActions (Find single actions): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            if (ids.Count() > 1)
            {
                foreach (var action in _groupActions)
                {
                    string actionName = action.GetType().Name.ToLowerFirstLetter();
                    if (role?.Actions != null && !role.Actions.Contains(actionName))
                    {
                        continue;
                    }

                    if (action.IsAvailable(entities))
                    {
                        var actionDto = result.FirstOrDefault(x => x.Name == actionName);
                        if (actionDto == null)
                        {
                            result.Add(new ActionDto
                            {
                                Color = action.Color.ToString().ToLowerFirstLetter(),
                                Name = actionName,
                                Ids = ids.Select(x=>x.ToString())
                            });                        
                        }
                    }                    
                }
            }
            Log.Debug("{entityName}.GetActions (Find group actions): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            return result;
        }

        public AppActionResult InvokeAction(string name, Guid id)
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var action = _singleActions.FirstOrDefault(x => x.GetType().Name.ToLowerFirstLetter() == name);
            
            if(action == null)
                return new AppActionResult
                {
                    IsError = true,
                    Message = $"Action {name} not found"
                };

            var currentUser = _userIdProvider.GetCurrentUser();
            var role = currentUser.RoleId.HasValue ? _dataService.GetById<Role>(currentUser.RoleId.Value) : null;
            var entity = _dataService.GetById<TEntity>(id);
            Log.Debug("{entityName}.InvokeAction (Load role): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            string actionName = action.GetType().Name.ToLowerFirstLetter();
            bool isActionAllowed = role?.Actions == null || role.Actions.Contains(actionName);

            var message = "";
            if (isActionAllowed && action.IsAvailable(entity)) 
                message += action.Run(currentUser, entity).Message;
            Log.Debug("{entityName}.InvokeAction (Apply action): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            return new AppActionResult
            {
                IsError = false,
                Message = message
            };
        }
        
        public AppActionResult InvokeAction(string name, IEnumerable<Guid> ids)
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var singleAction = _singleActions.FirstOrDefault(x => x.GetType().Name.ToLowerFirstLetter() == name);
            var groupAction = _groupActions.FirstOrDefault(x => x.GetType().Name.ToLowerFirstLetter() == name);
            Log.Debug("{entityName}.InvokeAction (Load role): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            if (singleAction == null && groupAction == null)
                return new AppActionResult
                {
                    IsError = true,
                    Message = $"Action {name} not found"
                };

            var currentUser = _userIdProvider.GetCurrentUser();
            var role = currentUser.RoleId.HasValue ? _dataService.GetById<Role>(currentUser.RoleId.Value) : null;
            var dbSet = _dataService.GetDbSet<TEntity>();

            var entities = dbSet.Where(x => ids.Contains(x.Id));

            if (groupAction != null)
            {
                string actionName = groupAction.GetType().Name.ToLowerFirstLetter();
                bool isActionAllowed = role?.Actions == null || role.Actions.Contains(actionName);
                if (isActionAllowed && groupAction.IsAvailable(entities))
                    return groupAction.Run(currentUser, entities);
            }
            else
            {
                List<string> messages = new List<string>();
                string actionName = singleAction.GetType().Name.ToLowerFirstLetter();
                bool isActionAllowed = role?.Actions == null || role.Actions.Contains(actionName);
                if (isActionAllowed)
                {
                    foreach (var entity in entities)
                    {
                        if (isActionAllowed && singleAction.IsAvailable(entity))
                        {
                            string message = singleAction.Run(currentUser, entity)?.Message;
                            if (!string.IsNullOrEmpty(message))
                            {
                                messages.Add(message);
                            }
                        }
                    }
                }
                return new AppActionResult
                {
                    IsError = false,
                    Message = string.Join(". ", messages)
                };
            }
            Log.Debug("{entityName}.InvokeAction (Apply action): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            return new AppActionResult
            {
                IsError = false,
                Message = "Done"
            };
        }

        public IEnumerable<BulkUpdateDto> GetBulkUpdates(IEnumerable<Guid> ids)
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var dbSet = _dataService.GetDbSet<TEntity>();
            var currentUser = _userIdProvider.GetCurrentUser();
            var role = currentUser.RoleId.HasValue ? _dataService.GetById<Role>(currentUser.RoleId.Value) : null;
            Log.Debug("{entityName}.GetBulkUpdates (Load role): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var fields = _fieldDispatcherService.GetDtoFields<TDto>();

            string forEntity = typeof(TEntity) == typeof(Order) 
                ? FieldPropertiesForEntityType.Orders.ToString() 
                : FieldPropertiesForEntityType.Shippings.ToString();
            var fieldsProperties = _fieldPropertiesService.GetFor(forEntity, null, role?.Id, null);
            Log.Debug("{entityName}.GetBulkUpdates (Load field settings): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var result = new List<BulkUpdateDto>();

            var entities = LoadStatusData(ids);
            Log.Debug("{entityName}.GetBulkUpdates (Load from DB): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            foreach (var field in fields.Where(x => x.IsBulkUpdateAllowed))
            {
                var fieldProperties = fieldsProperties.FirstOrDefault(x => string.Compare(x.FieldName, field.Name, true) == 0);
                var validEntities = entities.Where(e => CanEdit(e, fieldProperties));
                if (validEntities.Any())
                {
                    var dto = result.FirstOrDefault(x => x.Name == field.Name.ToLowerFirstLetter());
                    if (dto == null)
                    {
                        result.Add(new BulkUpdateDto
                        {
                            Name = field.Name.ToLowerFirstLetter(),
                            Type = field.FieldType.ToString(),
                            Ids = validEntities.Select(x => x.Id)
                        });
                    }
                }
            }
            Log.Debug("{entityName}.GetBulkUpdates (Find bulk updates): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            return result;
        }

        public AppActionResult InvokeBulkUpdate(string fieldName, IEnumerable<Guid> ids, string value)
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var propertyType = typeof(TFormDto).GetProperty(fieldName?.ToUpperFirstLetter());
            if (propertyType == null)
                throw new ArgumentException("Unknown field", nameof(fieldName));

            var dbSet = _dataService.GetDbSet<TEntity>();

            var entities = dbSet.Where(x => ids.Contains(x.Id));
            var dtos = entities.Select(MapFromEntityToFormDto).ToArray();
            Log.Debug("{entityName}.InvokeBulkUpdate (Load from DB): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            object validValue = value;
            if (propertyType.PropertyType == typeof(int?))
            {
                validValue = ParseInt(value);
            }
            if (propertyType.PropertyType == typeof(decimal?))
            {
                validValue = ParseDecimal(value);
            }
            if (propertyType.PropertyType == typeof(bool?))
            {
                validValue = ParseBool(value);
            }

            foreach (var dto in dtos)
            {
                propertyType.SetValue(dto, validValue);
            }

            var importResult = Import(dtos);
            Log.Debug("{entityName}.InvokeBulkUpdate (Import): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            string errors = string.Join(" ", importResult.Where(x => x.IsError).Select(x => x.Error));
            var result = new AppActionResult
            {
                IsError = !string.IsNullOrWhiteSpace(errors),
                Message = errors
            };

            if (!result.IsError)
            {
                string lang = _userIdProvider.GetCurrentUser()?.Language;
                string entityType = typeof(TEntity).Name.ToLower();
                string numbers = string.Join(", ", dtos.Select(GetNumber));
                result.Message = $"field_bulk_updated_{entityType}".Translate(lang, numbers);
            }

            return result;
        }

        protected T MapFromStateDto<T>(string dtoStatus) where T : struct
        {
            var mapFromStateDto = Enum.Parse<T>(dtoStatus.ToUpperFirstLetter());
            
            return mapFromStateDto;
        }
        
        public IEnumerable<ValidateResult> Import(IEnumerable<TFormDto> entitiesFrom)
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var result = new List<ValidateResult>();
            
            foreach (var dto in entitiesFrom) 
                result.Add(SaveOrCreate(dto));
            Log.Debug("{entityName}.Import: {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            return result;
        }        
        
        public ValidateResult ImportFromExcel(Stream fileStream)
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var excel = new ExcelPackage(fileStream);
            var workSheet = excel.Workbook.Worksheets.ElementAt(0);

            var excelMapper = CreateExcelMapper();
            var dtos = excelMapper.LoadEntries(workSheet).ToList();
            Log.Debug("{entityName}.ImportFromExcel (Load from file): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            if (excelMapper.Errors.Any(e => e.IsError))
            {
                string errors = string.Join(". ", excelMapper.Errors.Where(x => x.IsError).Select(x => x.Error));
                return new ValidateResult(errors);
            }

            var importResult = Import(dtos);
            Log.Debug("{entityName}.ImportFromExcel (Import): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            if (importResult.Any(e => e.IsError))
            {
                string errors = string.Join(". ", importResult.Where(x => x.IsError).Select(x => x.Error));
                return new ValidateResult(errors);
            }

            return new ValidateResult();
        }
        
        public Stream ExportToExcel(ExportExcelFormDto<TFilter> dto)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var excel = new ExcelPackage();

            var user = _userIdProvider.GetCurrentUser();

            string entityName = typeof(TEntity).Name.Pluralize().ToLowerFirstLetter();
            string entityDisplayName = entityName.Translate(user.Language);
            var workSheet = excel.Workbook.Worksheets.Add(entityDisplayName);

            var dbSet = _dataService.GetDbSet<TEntity>();
            var query = this.ApplySearchForm(dbSet, dto);
            Log.Debug("{entityName}.ExportToExcel (Load from DB): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var entities = query.ToList();
            var dtos = entities.Select(MapFromEntityToDto);
            Log.Debug("{entityName}.ExportToExcel (Convert to DTO): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var excelMapper = new ExcelMapper<TDto>(_dataService, _userIdProvider);
            excelMapper.FillSheet(workSheet, dtos, user.Language, dto?.Columns);
            Log.Debug("{entityName}.ExportToExcel (Fill file): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            return new MemoryStream(excel.GetAsByteArray());
        }

        protected virtual ExcelMapper<TFormDto> CreateExcelMapper()
        {
            return new ExcelMapper<TFormDto>(_dataService, _userIdProvider);
        }

        protected TimeSpan? ParseTime(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            if (TimeSpan.TryParseExact(value, new[] { @"hh\:mm\:ss", @"hh\:mm" },
                                       CultureInfo.InvariantCulture, TimeSpanStyles.None, out TimeSpan exactResult))
            {
                return exactResult;
            }
            if (TimeSpan.TryParse(value, out TimeSpan result))
            {
                return result;
            }
            return null;
        }

        protected decimal? ParseDecimal(string value)
        {
            if (decimal.TryParse((value ?? string.Empty).Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal decValue))
            {
                return decValue;
            }
            return null;
        }

        protected int? ParseInt(string value)
        {
            if (int.TryParse(value ?? string.Empty, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intValue))
            {
                return intValue;
            }
            return null;
        }

        protected bool? ParseBool(string value)
        {
            if (bool.TryParse(value ?? string.Empty, out bool boolValue))
            {
                return boolValue;
            }
            return null;
        }

        protected DateTime? ParseDateTime(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            if (DateTime.TryParseExact(
                    value, 
                    new[] { 
                        "dd.MM.yyyy HH:mm:ss", "dd.MM.yyyy HH:mm", "dd.MM.yyyy",
                        "MM/dd/yyyy HH:mm:ss", "MM/dd/yyyy HH:mm", "MM/dd/yyyy",
                        "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-dd"
                    },
                    CultureInfo.InvariantCulture, 
                    DateTimeStyles.None, 
                    out DateTime exactResult))
            {
                return exactResult;
            }
            if (DateTime.TryParse(value, out DateTime result))
            {
                return result;
            }
            return null;
        }

        protected decimal? Round(decimal? value, int decimals)
        {
            if (value == null)
            {
                return null;
            }
            else
            {
                return decimal.Round(value.Value, decimals);
            }
        }

        private bool CanEdit(EntityStatusDto dto, FieldForFieldProperties fieldProperties)
        {
            string editValue = FieldPropertiesAccessType.Edit.ToString();
            string accessType = fieldProperties?.AccessTypes?
                                                .Where(x => string.Compare(x.Key, dto.Status, true) == 0)
                                                .Select(x => x.Value)
                                                .FirstOrDefault();
            return string.Compare(accessType, editValue, true) == 0;
        }
    }
}