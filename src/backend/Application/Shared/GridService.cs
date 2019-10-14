using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DAL;
using DAL.Queries;
using Domain;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.UserProvider;
using Domain.Extensions;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Globalization;
using Application.Shared.Excel;

namespace Application.Shared
{
    public abstract class GridService<TEntity, TDto, TFormDto, TSummaryDto, TSearchForm>: IGridService<TEntity, TDto, TFormDto, TSummaryDto, TSearchForm>
        where TEntity : class, IPersistable, new() 
        where TDto : IDto, new() 
        where TFormDto : IDto, new()
        where TSearchForm: PagingFormDto
    {
        //public abstract DbSet<TEntity> UseDbSet(AppDbContext dbContext);
        public abstract ValidateResult MapFromDtoToEntity(TEntity entity, TDto dto);
        public abstract ValidateResult MapFromFormDtoToEntity(TEntity entity, TFormDto dto);
        public abstract TDto MapFromEntityToDto(TEntity entity);
        public abstract TFormDto MapFromEntityToFormDto(TEntity entity);
        public abstract LookUpDto MapFromEntityToLookupDto(TEntity entity);

        public abstract TSummaryDto GetSummary(IEnumerable<Guid> ids);
        public abstract IQueryable<TEntity> ApplySearchForm(IQueryable<TEntity> query, TSearchForm searchForm);

        protected virtual void ApplyAfterSaveActions(TEntity entity, TDto dto) { }

        private readonly IUserProvider userIdProvider;

        protected readonly ICommonDataService dataService;

        protected readonly IActionService<TEntity> actionService;

        //TODO: remove this
        private readonly AppDbContext db;

        protected GridService(ICommonDataService dataService, AppDbContext db, IUserProvider userIdProvider, IActionService<TEntity> actionService)
        {
            this.userIdProvider = userIdProvider;
            this.dataService = dataService;
            this.actionService = actionService;
            this.db = db;
        }

        public TDto Get(Guid id)
        {
            var entity = dataService.GetById<TEntity>(id);
            return MapFromEntityToDto(entity);
        }

        public TFormDto GetForm(Guid id)
        {
            var entity = dataService.GetById<TEntity>(id);
            return MapFromEntityToFormDto(entity);
        }

        public IEnumerable<LookUpDto> ForSelect()
        {
            var dbSet = dataService.GetDbSet<TEntity>();
            return  dbSet.ToList().Select(MapFromEntityToLookupDto);
        }

        public SearchResult<TDto> Search(TSearchForm form)
        {
            var dbSet = dataService.GetDbSet<TEntity>();

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
            var dbSet = dataService.GetDbSet<TEntity>();
            
            var query = this.ApplySearchForm(dbSet, form);

            var ids = query.Select(e => e.Id).ToList();

            var result = ids.Select(x => x.ToString());
            return result;
        }

        public ValidateResult SaveOrCreate(TFormDto entityFrom)
        {
            ValidateResult mapResult;
            var dbSet = dataService.GetDbSet<TEntity>();
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
                    
                    dataService.SaveChanges();
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

            dataService.SaveChanges();
            return new ValidateResult
            {
                Id = entity.Id.ToString()
            };
        }

        public IEnumerable<ActionDto> GetActions(IEnumerable<Guid> ids)
        {
            if (ids == null) 
                throw new ArgumentNullException(nameof(ids));
            
            var dbSet = dataService.GetDbSet<TEntity>();
            var currentUser = userIdProvider.GetCurrentUser();
            var role = currentUser.RoleId.HasValue ? dataService.GetById<Role>(currentUser.RoleId.Value) : null;

            var result = new List<ActionDto>();

            var entities = ids.Select(id => dbSet.GetById(id));

            var singleActions = actionService.GetActions();
            foreach (var action in singleActions)
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
                var groupActions = actionService.GetGroupActions();
                foreach (var action in groupActions)
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
            var action = actionService.GetActions().FirstOrDefault(x => x.GetType().Name.ToLowerfirstLetter() == name);
            
            if(action == null)
                return new AppActionResult
                {
                    IsError = true,
                    Message = $"Action {name} not found"
                };

            var currentUser = userIdProvider.GetCurrentUser();
            var role = currentUser.RoleId.HasValue ? dataService.GetById<Role>(currentUser.RoleId.Value) : null;
            var entity = dataService.GetById<TEntity>(id);
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
            var singleAction = actionService.GetActions().FirstOrDefault(x => x.GetType().Name.ToLowerfirstLetter() == name);
            var groupAction = actionService.GetGroupActions().FirstOrDefault(x => x.GetType().Name.ToLowerfirstLetter() == name);

            if (singleAction == null && groupAction == null)
                return new AppActionResult
                {
                    IsError = true,
                    Message = $"Action {name} not found"
                };

            var currentUser = userIdProvider.GetCurrentUser();
            var role = currentUser.RoleId.HasValue ? dataService.GetById<Role>(currentUser.RoleId.Value) : null;
            var dbSet = dataService.GetDbSet<TEntity>();

            var entities = ids.Select(dbSet.GetById);

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
            var dbSet = dataService.GetDbSet<TEntity>();
            var entities = dbSet.ToList();
            var dtos = entities.Select(MapFromEntityToDto);

            var user = userIdProvider.GetCurrentUser();

            var excelMapper = new ExcelMapper<TDto>(db);
            excelMapper.FillSheet(workSheet, dtos, user.Language);
            
            return new MemoryStream(excel.GetAsByteArray());
        }

        protected virtual ExcelMapper<TFormDto> CreateExcelMapper()
        {
            return new ExcelMapper<TFormDto>(db);
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
    }
}