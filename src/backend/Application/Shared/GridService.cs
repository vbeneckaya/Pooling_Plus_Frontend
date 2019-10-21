using Application.BusinessModels.Shared.Actions;
using Application.BusinessModels.Shared.BulkUpdates;
using Application.Shared.Excel;
using DAL.Queries;
using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Application.Shared
{
    public abstract class GridService<TEntity, TDto, TFormDto, TSummaryDto, TSearchForm>: IGridService<TEntity, TDto, TFormDto, TSummaryDto, TSearchForm>
        where TEntity : class, IPersistable, new() 
        where TDto : IDto, new() 
        where TFormDto : IDto, new()
        where TSearchForm: PagingFormDto
    {
        public abstract ValidateResult MapFromDtoToEntity(TEntity entity, TDto dto);
        public abstract ValidateResult MapFromFormDtoToEntity(TEntity entity, TFormDto dto);
        public abstract TDto MapFromEntityToDto(TEntity entity);
        public abstract TFormDto MapFromEntityToFormDto(TEntity entity);
        public abstract LookUpDto MapFromEntityToLookupDto(TEntity entity);

        public abstract TSummaryDto GetSummary(IEnumerable<Guid> ids);
        public abstract IQueryable<TEntity> ApplySearchForm(IQueryable<TEntity> query, TSearchForm searchForm);

        protected virtual void ApplyAfterSaveActions(TEntity entity, TDto dto) { }

        private readonly IUserProvider _userIdProvider;

        protected readonly ICommonDataService _dataService;

        protected readonly IEnumerable<IAppAction<TEntity>> _singleActions;

        protected readonly IEnumerable<IGroupAppAction<TEntity>> _groupActions;

        protected readonly IEnumerable<IBulkUpdate<TEntity>> _bulkUpdates;

        protected GridService(
            ICommonDataService dataService, 
            IUserProvider userIdProvider,
            IEnumerable<IAppAction<TEntity>> singleActions,
            IEnumerable<IGroupAppAction<TEntity>> groupActions,
            IEnumerable<IBulkUpdate<TEntity>> bulkUpdates)
        {
            _userIdProvider = userIdProvider;
            _dataService = dataService;
            _singleActions = singleActions;
            _groupActions = groupActions;
            _bulkUpdates = bulkUpdates;
        }

        public TDto Get(Guid id)
        {
            var entity = _dataService.GetById<TEntity>(id);
            return MapFromEntityToDto(entity);
        }

        public TFormDto GetForm(Guid id)
        {
            var entity = _dataService.GetById<TEntity>(id);
            return MapFromEntityToFormDto(entity);
        }

        public IEnumerable<LookUpDto> ForSelect()
        {
            var dbSet = _dataService.GetDbSet<TEntity>();
            return  dbSet.ToList().Select(MapFromEntityToLookupDto);
        }

        public SearchResult<TDto> Search(TSearchForm form)
        {
            var dbSet = _dataService.GetDbSet<TEntity>();

            //if (!string.IsNullOrEmpty(form.Search))
            //{
            //    var stringProperties = typeof(TEntity).GetProperties().Where(prop =>
            //        prop.PropertyType == form.Search.GetType());

            //    //TODO Вернуть полнотекстовый поиск
            //    query = query.Where(entity =>  entity.Id.ToString() == form.Search);
            //}

            var query = this.ApplySearchForm(dbSet, form);

            if (form.Take == 0)
                form.Take = 1000;

            
            var totalCount = query.Count();
            var entities = query.Skip(form.Skip)
                .Take(form.Take).ToList();

            var a = new SearchResult<TDto>
            {
                TotalCount = totalCount,
                Items = entities.Select(entity => MapFromEntityToDto(entity))
            };
            return a;
        }

        public IEnumerable<string> SearchIds(TSearchForm form)
        {
            var dbSet = _dataService.GetDbSet<TEntity>();
            
            var query = this.ApplySearchForm(dbSet, form);

            var ids = query.Select(e => e.Id).ToList();

            var result = ids.Select(x => x.ToString());
            return result;
        }

        public ValidateResult SaveOrCreate(TFormDto entityFrom)
        {
            ValidateResult mapResult;
            var dbSet = _dataService.GetDbSet<TEntity>();
            if (!string.IsNullOrEmpty(entityFrom.Id))
            {
                var entityFromDb = dbSet.GetById(Guid.Parse(entityFrom.Id));
                if (entityFromDb != null)
                {
                    mapResult = MapFromFormDtoToEntity(entityFromDb, entityFrom);
                    if (mapResult.IsError)
                    {
                        return mapResult;
                    }

                    dbSet.Update(entityFromDb);
                    
                    _dataService.SaveChanges();
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
            if (mapResult.IsError)
            {
                return mapResult;
            }

            dbSet.Add(entity);

            _dataService.SaveChanges();
            return new ValidateResult
            {
                Id = entity.Id.ToString()
            };
        }

        public IEnumerable<ActionDto> GetActions(IEnumerable<Guid> ids)
        {
            if (ids == null) 
                throw new ArgumentNullException(nameof(ids));
            
            var dbSet = _dataService.GetDbSet<TEntity>();
            var currentUser = _userIdProvider.GetCurrentUser();
            var role = currentUser.RoleId.HasValue ? _dataService.GetById<Role>(currentUser.RoleId.Value) : null;

            var result = new List<ActionDto>();

            var entities = dbSet.Where(x => ids.Contains(x.Id));

            foreach (var action in _singleActions)
            {
                var validEntities = entities.Where(e => action.IsAvailable(role, e));
                if (validEntities.Any())
                {
                    var actionDto = result.FirstOrDefault(x => x.Name == action.GetType().Name.ToLowerfirstLetter());
                    if (actionDto == null)
                    {
                        result.Add(new ActionDto
                        {
                            Color = action.Color.ToString().ToLowerfirstLetter(),
                            Name = action.GetType().Name.ToLowerfirstLetter(),
                            Ids = validEntities.Select(x => x.Id.ToString())
                        });
                    }
                }
            }

            if (ids.Count() > 1)
            {
                foreach (var action in _groupActions)
                {
                    if (action.IsAvailable(role, entities))
                    {
                        var actionDto = result.FirstOrDefault(x => x.Name == action.GetType().Name.ToLowerfirstLetter());
                        if (actionDto == null)
                        {
                            result.Add(new ActionDto
                            {
                                Color = action.Color.ToString().ToLowerfirstLetter(),
                                Name = action.GetType().Name.ToLowerfirstLetter(),
                                Ids = ids.Select(x=>x.ToString())
                            });                        
                        }
                    }                    
                }
            }

            return result;
        }

        public AppActionResult InvokeAction(string name, Guid id)
        {
            var action = _singleActions.FirstOrDefault(x => x.GetType().Name.ToLowerfirstLetter() == name);
            
            if(action == null)
                return new AppActionResult
                {
                    IsError = true,
                    Message = $"Action {name} not found"
                };

            var currentUser = _userIdProvider.GetCurrentUser();
            var role = currentUser.RoleId.HasValue ? _dataService.GetById<Role>(currentUser.RoleId.Value) : null;
            var entity = _dataService.GetById<TEntity>(id);
            var message = "";
            if (action.IsAvailable(role, entity)) 
                message += action.Run(currentUser, entity).Message;
            
            return new AppActionResult
            {
                IsError = false,
                Message = message
            };
        }
        
        public AppActionResult InvokeAction(string name, IEnumerable<Guid> ids)
        {
            var singleAction = _singleActions.FirstOrDefault(x => x.GetType().Name.ToLowerfirstLetter() == name);
            var groupAction = _groupActions.FirstOrDefault(x => x.GetType().Name.ToLowerfirstLetter() == name);

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
                if (groupAction.IsAvailable(role, entities))
                    return groupAction.Run(currentUser, entities);
            }
            else
            {
                List<string> messages = new List<string>();
                foreach (var entity in entities)
                {
                    if (singleAction.IsAvailable(role, entity))
                    {
                        string message = singleAction.Run(currentUser, entity)?.Message;
                        if (!string.IsNullOrEmpty(message))
                        {
                            messages.Add(message);
                        }
                    }
                }
                return new AppActionResult
                {
                    IsError = false,
                    Message = string.Join(". ", messages)
                };
            }

            return new AppActionResult
            {
                IsError = false,
                Message = "Done"
            };
        }

        public IEnumerable<BulkUpdateDto> GetBulkUpdates(IEnumerable<Guid> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var dbSet = _dataService.GetDbSet<TEntity>();
            var currentUser = _userIdProvider.GetCurrentUser();
            var role = currentUser.RoleId.HasValue ? _dataService.GetById<Role>(currentUser.RoleId.Value) : null;

            var result = new List<BulkUpdateDto>();

            var entities = dbSet.Where(x => ids.Contains(x.Id));

            foreach (var bulkUpdate in _bulkUpdates)
            {
                var validEntities = entities.Where(e => bulkUpdate.IsAvailable(role, e));
                if (validEntities.Any())
                {
                    var dto = result.FirstOrDefault(x => x.Name == bulkUpdate.FieldName.ToLowerfirstLetter());
                    if (dto == null)
                    {
                        result.Add(new BulkUpdateDto
                        {
                            Name = bulkUpdate.FieldName.ToLowerfirstLetter(),
                            Type = bulkUpdate.FieldType.ToString(),
                            Ids = validEntities.Select(x => x.Id.ToString())
                        });
                    }
                }
            }

            return result;
        }

        public AppActionResult InvokeBulkUpdate(string fieldName, IEnumerable<Guid> ids, string value)
        {
            var bulkUpdate = _bulkUpdates.FirstOrDefault(x => x.FieldName.ToLowerfirstLetter() == fieldName);

            if (bulkUpdate == null)
                return new AppActionResult
                {
                    IsError = true,
                    Message = $"Bulk update {fieldName} not found"
                };

            var currentUser = _userIdProvider.GetCurrentUser();
            var role = currentUser.RoleId.HasValue ? _dataService.GetById<Role>(currentUser.RoleId.Value) : null;
            var dbSet = _dataService.GetDbSet<TEntity>();

            var entities = dbSet.Where(x => ids.Contains(x.Id));

            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

            foreach (var entity in entities)
            {
                if (bulkUpdate.IsAvailable(role, entity))
                {
                    string message = bulkUpdate.Update(currentUser, entity, value)?.Message;
                    if (!string.IsNullOrEmpty(message))
                    {
                        List<string> numbers;
                        if (!errors.TryGetValue(message, out numbers))
                        {
                            numbers = new List<string>();
                            errors[message] = numbers;
                        }
                        numbers.Add(entity.ToString());
                    }
                }
            }

            _dataService.SaveChanges();

            List<string> messages = new List<string>();
            foreach (var error in errors)
            {
                string numbers = string.Join(", ", error.Value);
                messages.Add("bulkUpdateOrderErrors".translate(currentUser.Language, numbers, error.Key));
            }

            return new AppActionResult
            {
                IsError = false,
                Message = string.Join(" ", messages)
            };
        }

        protected T MapFromStateDto<T>(string dtoStatus) where T : struct
        {
            var mapFromStateDto = Enum.Parse<T>(dtoStatus.ToUpperfirstLetter());
            
            return mapFromStateDto;
        }
        
        public IEnumerable<ValidateResult> Import(IEnumerable<TFormDto> entitiesFrom)
        {
            var result = new List<ValidateResult>();
            
            foreach (var dto in entitiesFrom) 
                result.Add(SaveOrCreate(dto));

            return result;
        }        
        
        public ValidateResult ImportFromExcel(Stream fileStream)
        {
            var excel = new ExcelPackage(fileStream);
            var workSheet = excel.Workbook.Worksheets.ElementAt(0);

            var excelMapper = CreateExcelMapper();
            var dtos = excelMapper.LoadEntries(workSheet).ToList();

            if (excelMapper.Errors.Any(e => e.IsError))
            {
                string errors = string.Join(". ", excelMapper.Errors.Where(x => x.IsError).Select(x => x.Error));
                return new ValidateResult(errors);
            }

            var importResult = Import(dtos);
            if (importResult.Any(e => e.IsError))
            {
                string errors = string.Join(". ", importResult.Where(x => x.IsError).Select(x => x.Error));
                return new ValidateResult(errors);
            }

            return new ValidateResult();
        }
        
        public Stream ExportToExcel()
        {
            var excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add(typeof(TEntity).Name);
            var dbSet = _dataService.GetDbSet<TEntity>();
            var entities = dbSet.ToList();
            var dtos = entities.Select(MapFromEntityToDto);

            var user = _userIdProvider.GetCurrentUser();

            var excelMapper = new ExcelMapper<TDto>(_dataService);
            excelMapper.FillSheet(workSheet, dtos, user.Language);
            
            return new MemoryStream(excel.GetAsByteArray());
        }

        protected virtual ExcelMapper<TFormDto> CreateExcelMapper()
        {
            return new ExcelMapper<TFormDto>(_dataService);
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
    }
}