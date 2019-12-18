using Application.BusinessModels.Shared.Actions;
using Application.BusinessModels.Shared.Handlers;
using Application.Services.Triggers;
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
using Microsoft.Extensions.DependencyInjection;
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
        where TFormDto : TDto, new()
    {
        public abstract void MapFromDtoToEntity(TEntity entity, TDto dto);
        public abstract void MapFromFormDtoToEntity(TEntity entity, TFormDto dto);
        public abstract TDto MapFromEntityToDto(TEntity entity);
        public abstract TFormDto MapFromEntityToFormDto(TEntity entity);
        public abstract LookUpDto MapFromEntityToLookupDto(TEntity entity);

        public abstract IEnumerable<EntityStatusDto> LoadStatusData(IEnumerable<Guid> ids);

        public abstract IQueryable<TEntity> ApplyRestrictions(IQueryable<TEntity> query);

        public abstract string GetNumber(TFormDto dto);

        public abstract TSummaryDto GetSummary(IEnumerable<Guid> ids);
        public abstract IQueryable<TEntity> ApplySearchForm(IQueryable<TEntity> query, FilterFormDto<TFilter> searchForm, List<string> columns = null);

        protected virtual void ApplyAfterSaveActions(TEntity entity, TDto dto) { }

        protected readonly IUserProvider _userIdProvider;
        protected readonly ICommonDataService _dataService;
        protected readonly IFieldDispatcherService _fieldDispatcherService;
        protected readonly IFieldPropertiesService _fieldPropertiesService;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ITriggersService _triggersService;
        protected readonly IValidationService _validationService;
        protected readonly IFieldSetterFactory _fieldSetterFactory;

        protected GridService(
            ICommonDataService dataService, 
            IUserProvider userIdProvider,
            IFieldDispatcherService fieldDispatcherService,
            IFieldPropertiesService fieldPropertiesService,
            IServiceProvider serviceProvider,
            ITriggersService triggersService, 
            IValidationService validationService, 
            IFieldSetterFactory fieldSetterFactory)
        {
            _userIdProvider = userIdProvider;
            _dataService = dataService;
            _fieldDispatcherService = fieldDispatcherService;
            _fieldPropertiesService = fieldPropertiesService;
            _serviceProvider = serviceProvider;
            _triggersService = triggersService;
            _validationService = validationService;
            _fieldSetterFactory = fieldSetterFactory;
        }

        protected virtual IFieldSetter<TEntity> ConfigureHandlers(IFieldSetter<TEntity> setter, TFormDto dto)
        {
            return null;
        }

        protected virtual IChangeTracker ConfigureChangeTacker()
        {
            return null;
        }

        public TDto Get(Guid id)
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var entity = _dataService.GetById<TEntity>(id);
            Log.Information("{entityName}.Get (Load from DB): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var result = MapFromEntityToDto(entity);
            Log.Information("{entityName}.Get (Convert to DTO): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            result = FillLookupNames(result);
            Log.Information("{entityName}.Get (Fill lookups): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            return result;
        }

        public TFormDto GetForm(Guid id)
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var entity = _dataService.GetById<TEntity>(id);
            Log.Information("{entityName}.GetForm (Load from DB): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var result = MapFromEntityToFormDto(entity);
            Log.Information("{entityName}.GetForm (Convert to DTO): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            result = (TFormDto)FillLookupNames(result);
            Log.Information("{entityName}.GetForm (Fill lookups): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            return result;
        }

        public IEnumerable<LookUpDto> ForSelect()
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var entries = _dataService.GetDbSet<TEntity>().ToList();
            Log.Information("{entityName}.ForSelect (Load from DB): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var result = entries.Select(MapFromEntityToLookupDto).ToList();
            Log.Information("{entityName}.ForSelect (Convert to DTO): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            return result;
        }

        public SearchResult<TDto> Search(FilterFormDto<TFilter> form)
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var dbSet = _dataService.GetDbSet<TEntity>();

            var query = ApplySearchForm(dbSet, form);
            query = ApplyRestrictions(query);
            
            Log.Information("{entityName}.Search (Apply search parameters): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            if (form.Take == 0)
                form.Take = 1000;

            var totalCount = query.Count();
            var entities = query.Skip(form.Skip)
                .Take(form.Take).ToList();
            Log.Information("{entityName}.Search (Load from DB): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var a = new SearchResult<TDto>
            {
                TotalCount = totalCount,
                Items = entities.Select(entity => MapFromEntityToDto(entity)).ToList()
            };
            Log.Information("{entityName}.Search (Convert to DTO): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            a.Items = FillLookupNames(a.Items).ToList();
            Log.Information("{entityName}.Search (Fill lookups): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            return a;
        }

        public IEnumerable<string> SearchIds(FilterFormDto<TFilter> form)
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var dbSet = _dataService.GetDbSet<TEntity>();
            
            var query = ApplySearchForm(dbSet, form);
            query = ApplyRestrictions(query);
            
            Log.Information("{entityName}.SearchIds (Apply search parameters): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var ids = query.Select(e => e.Id).ToList();
            Log.Information("{entityName}.SearchIds (Load from DB): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            var result = ids.Select(x => x.ToString()).ToList();
            return result;
        }

        protected virtual DetailedValidationResult ValidateDto(TDto dto)
        {
            return _validationService.Validate(dto);
        }

        public ValidateResult SaveOrCreate(TFormDto entityFrom)
        {
            var validateResult = ValidateDto(entityFrom);
            if (validateResult.IsError)
            {
                return validateResult;
            }

            return SaveOrCreateInner(entityFrom);
        }

        private ValidateResult SaveOrCreateInner(TFormDto entityFrom)
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            ValidateResult mapResult;
            var dbSet = _dataService.GetDbSet<TEntity>();

            // Validation step

            var result = ValidateDto(entityFrom);

            if (result.IsError)
            {
                return result;
            }

            var trackConfig = this.ConfigureChangeTacker();

            if (!string.IsNullOrEmpty(entityFrom.Id))
            {
                var entityFromDb = dbSet.GetById(Guid.Parse(entityFrom.Id));

                if (entityFromDb == null)
                    throw new Exception($"Order not found (Id = {entityFrom.Id})");

                Log.Information("{entityName}.SaveOrCreate (Find entity by Id): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
                sw.Restart();

                MapFromFormDtoToEntity(entityFromDb, entityFrom);

                // Change handlers

                var updateChanges = this._dataService.GetChanges<TEntity>().FirstOrDefault(x => x.Entity.Id == entityFromDb.Id);

                var setter = this.ConfigureHandlers(this._fieldSetterFactory.Create<TEntity>(), entityFrom);

                if (setter != null)
                {
                    setter.Appy(updateChanges);
                }

                var logChanges = this._dataService.GetChanges<TEntity>().FirstOrDefault(x => x.Entity.Id == entityFromDb.Id);
                if (trackConfig != null)
                {
                    trackConfig.LogTrackedChanges<TEntity>(logChanges);
                }
                Log.Information("{entityName}.SaveOrCreate (Update fields): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
                sw.Restart();

                //dbSet.Update(entityFromDb);

                _triggersService.Execute();
                Log.Information("{entityName}.SaveOrCreate (Execure triggers): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
                sw.Restart();
               

                _dataService.SaveChanges();
                Log.Information("{entityName}.SaveOrCreate (Save changes): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

                return new ValidateResult
                {
                    Id = entityFromDb.Id.ToString()
                };
            }
            else
            {
                var entity = new TEntity
                {
                    Id = Guid.NewGuid()
                };

                // Mapping

                MapFromFormDtoToEntity(entity, entityFrom);

                dbSet.Add(entity);

                var changes = this._dataService.GetChanges<TEntity>().FirstOrDefault(x => x.Entity.Id == entity.Id);

                // Change handlers

                var updateSetter = this.ConfigureHandlers(this._fieldSetterFactory.Create<TEntity>(), entityFrom);

                if (updateSetter != null)
                {
                    updateSetter.Appy(changes);
                }

                var logChanges = this._dataService.GetChanges<TEntity>().FirstOrDefault(x => x.Entity.Id == entity.Id);

                Log.Information("{entityName}.SaveOrCreate (Fill fields): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
                sw.Restart();

                _triggersService.Execute();
                Log.Information("{entityName}.SaveOrCreate (Execure triggers): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
                sw.Restart();

                if (trackConfig != null)
                {
                    trackConfig.LogTrackedChanges<TEntity>(logChanges);
                }

                _dataService.SaveChanges();
                Log.Information("{entityName}.SaveOrCreate (Save changes): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

                return new ValidateResult
                {
                    Id = entity.Id.ToString()
                };
            }
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
            Log.Information("{entityName}.GetActions (Load role): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var actionDtos = new Dictionary<string, ActionInfo>();

            var entities = dbSet.Where(x => ids.Contains(x.Id)).ToList();
            Log.Information("{entityName}.GetActions (Load data from DB): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var singleActions = _serviceProvider.GetService<IEnumerable<IAppAction<TEntity>>>();
            foreach (var action in singleActions)
            {
                string actionName = action.GetType().Name.ToLowerFirstLetter();
                if ((role?.Actions != null && !role.Actions.Contains(actionName)) || actionDtos.ContainsKey(actionName))
                {
                    continue;
                }

                var validEntities = entities.Where(e => action.IsAvailable(e));
                if (validEntities.Any() && ids.Count() == validEntities.Count())
                {
                    actionDtos[actionName] = ConvertActionToDto(action, actionName, ids);
                }
            }
            Log.Information("{entityName}.GetActions (Find single actions): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            if (ids.Count() > 1)
            {
                var groupActions = _serviceProvider.GetService<IEnumerable<IGroupAppAction<TEntity>>>();
                foreach (var action in groupActions)
                {
                    string actionName = action.GetType().Name.ToLowerFirstLetter();
                    if ((role?.Actions != null && !role.Actions.Contains(actionName)) || actionDtos.ContainsKey(actionName))
                    {
                        continue;
                    }

                    if (action.IsAvailable(entities))
                    {
                        actionDtos[actionName] = ConvertActionToDto(action, actionName, ids);
                    }                    
                }
            }
            Log.Information("{entityName}.GetActions (Find group actions): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            var result = actionDtos.OrderBy(x => x.Value.OrderNumber)
                                   .Select(x => x.Value.Dto)
                                   .ToList();
            return result;
        }

        private ActionInfo ConvertActionToDto<T>(IAction<T> action, string actionName, IEnumerable<Guid> ids)
        {
            string group = null;
            foreach (var attr in action.GetType().GetCustomAttributes(typeof(ActionGroupAttribute), false))
            {
                group = (attr as ActionGroupAttribute)?.Group;
            }

            int orderNumber = 0;
            foreach (var attr in action.GetType().GetCustomAttributes(typeof(OrderNumberAttribute), false))
            {
                orderNumber = (attr as OrderNumberAttribute)?.Value ?? orderNumber;
            }

            ActionAccess access = ActionAccess.Everywhere;
            foreach (var attr in action.GetType().GetCustomAttributes(typeof(ActionAccessAttribute), false))
            {
                access = (attr as ActionAccessAttribute)?.Access ?? access;
            }

            var dto = new ActionDto
            {
                Color = action.Color.ToString().ToLowerFirstLetter(),
                Name = actionName,
                Group = group,
                AllowedFromGrid = access != ActionAccess.FormOnly,
                AllowedFromForm = access != ActionAccess.GridOnly,
                Ids = ids.Select(x => x.ToString())
            };

            return new ActionInfo
            {
                Dto = dto,
                OrderNumber = orderNumber
            };
        }

        public AppActionResult InvokeAction(string name, Guid id)
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var singleActions = _serviceProvider.GetService<IEnumerable<IAppAction<TEntity>>>();
            var action = singleActions.FirstOrDefault(x => x.GetType().Name.ToLowerFirstLetter() == name);
            
            if(action == null)
                return new AppActionResult
                {
                    IsError = true,
                    Message = $"Action {name} not found"
                };

            var currentUser = _userIdProvider.GetCurrentUser();
            var role = currentUser.RoleId.HasValue ? _dataService.GetById<Role>(currentUser.RoleId.Value) : null;
            var entity = _dataService.GetById<TEntity>(id);
            Log.Information("{entityName}.InvokeAction (Load role): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            string actionName = action.GetType().Name.ToLowerFirstLetter();
            bool isActionAllowed = role?.Actions == null || role.Actions.Contains(actionName);

            var message = "";
            if (isActionAllowed && action.IsAvailable(entity)) 
                message += action.Run(currentUser, entity).Message;
            Log.Information("{entityName}.InvokeAction (Apply action): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            _triggersService.Execute();
            Log.Information("{entityName}.InvokeAction (Execure triggers): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            _dataService.SaveChanges();
            Log.Information("{entityName}.InvokeAction (Save changes): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

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

            var singleActions = _serviceProvider.GetService<IEnumerable<IAppAction<TEntity>>>();
            var singleAction = singleActions.FirstOrDefault(x => x.GetType().Name.ToLowerFirstLetter() == name);

            var groupActions = _serviceProvider.GetService<IEnumerable<IGroupAppAction<TEntity>>>();
            var groupAction = groupActions.FirstOrDefault(x => x.GetType().Name.ToLowerFirstLetter() == name);

            Log.Information("{entityName}.InvokeAction (Load role): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
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

            AppActionResult result;
            if (groupAction != null)
            {
                string actionName = groupAction.GetType().Name.ToLowerFirstLetter();
                bool isActionAllowed = role?.Actions == null || role.Actions.Contains(actionName);
                if (isActionAllowed && groupAction.IsAvailable(entities))
                {
                    result = groupAction.Run(currentUser, entities);
                }
                else
                {
                    result = new AppActionResult
                    {
                        IsError = false,
                        Message = "Done"
                    };
                }
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
                result = new AppActionResult
                {
                    IsError = false,
                    Message = string.Join(". ", messages)
                };
            }
            Log.Information("{entityName}.InvokeAction (Apply action): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            _triggersService.Execute();
            Log.Information("{entityName}.InvokeAction (Execure triggers): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            _dataService.SaveChanges();
            Log.Information("{entityName}.InvokeAction (Save changes): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            return result;
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
            Log.Information("{entityName}.GetBulkUpdates (Load role): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var fields = _fieldDispatcherService.GetDtoFields<TDto>();

            string forEntity = typeof(TEntity) == typeof(Order) 
                ? FieldPropertiesForEntityType.Orders.ToString() 
                : FieldPropertiesForEntityType.Shippings.ToString();
            var fieldsProperties = _fieldPropertiesService.GetFor(forEntity, null, role?.Id, null);
            Log.Information("{entityName}.GetBulkUpdates (Load field settings): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var result = new List<BulkUpdateDto>();

            var entities = LoadStatusData(ids);
            Log.Information("{entityName}.GetBulkUpdates (Load from DB): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
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
            Log.Information("{entityName}.GetBulkUpdates (Find bulk updates): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

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
            Log.Information("{entityName}.InvokeBulkUpdate (Load from DB): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
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
            if (propertyType.PropertyType == typeof(LookUpDto))
            {
                validValue = value == null ? null : new LookUpDto { Value = value };
            }

            foreach (var dto in dtos)
            {
                propertyType.SetValue(dto, validValue);
            }

            var importResult = new List<ValidateResult>();
            foreach (var dto in dtos)
                importResult.Add(SaveOrCreateInner(dto));
            Log.Information("{entityName}.InvokeBulkUpdate (Import): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

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

        protected virtual IEnumerable<TDto> FillLookupNames(IEnumerable<TDto> dtos)
        {
            return dtos;
        }

        protected TDto FillLookupNames(TDto dto)
        {
            return FillLookupNames(new[] { dto }).FirstOrDefault();
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
            {
                var validateResult = ValidateDto(dto);
                if (validateResult.IsError)
                {
                    result.Add(validateResult);
                }
                else
                {
                    result.Add(SaveOrCreateInner(dto));
                }
            }
            Log.Information("{entityName}.Import: {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

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
            var records = excelMapper.LoadEntries(workSheet).ToList();
            var dtos = records.Select(i => i.Data);

            Log.Information("{entityName}.ImportFromExcel (Load from file): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            if (excelMapper.Errors.Any(e => e.IsError))
            {
                string errors = string.Join(". ", excelMapper.Errors.Where(x => x.IsError).Select(x => x.Error));
                return new ValidateResult(errors);
            }

            var importResult = Import(dtos);
            Log.Information("{entityName}.ImportFromExcel (Import): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

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
            var query = this.ApplySearchForm(dbSet, dto, dto.Columns);
            
            query = ApplyRestrictions(query);

            Log.Information("{entityName}.ExportToExcel (Load from DB): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var entities = query.ToList();
            var dtos = entities.Select(MapFromEntityToDto);
            Log.Information("{entityName}.ExportToExcel (Convert to DTO): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            dtos = FillLookupNames(dtos).ToList();
            Log.Information("{entityName}.ExportToExcel (Fill lookups): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var excelMapper = CreateExportExcelMapper();//new ExcelMapper<TDto>(_dataService, _userIdProvider);
            excelMapper.FillSheet(workSheet, dtos, user.Language, dto?.Columns);
            Log.Information("{entityName}.ExportToExcel (Fill file): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            return new MemoryStream(excel.GetAsByteArray());
        }

        protected virtual ExcelMapper<TFormDto> CreateExcelMapper()
        {
            return new ExcelMapper<TFormDto>(_dataService, _userIdProvider, _fieldDispatcherService);
        }

        protected virtual ExcelMapper<TDto> CreateExportExcelMapper()
        {
            return new ExcelMapper<TDto>(_dataService, _userIdProvider, _fieldDispatcherService);
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

        private class ActionInfo
        {
            public ActionDto Dto { get; set; }
            public int OrderNumber { get; set; }
        }
    }
}