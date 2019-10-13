using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DAL;
using DAL.Queries;
using Domain;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.UserIdProvider;
using Domain.Extensions;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Globalization;

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

        private readonly IUserIdProvider userIdProvider;

        protected readonly ICommonDataService dataService;

        protected readonly IActionService<TEntity> actionService;

        protected GridService(ICommonDataService dataService, IUserIdProvider userIdProvider, IActionService<TEntity> actionService)
        {
            this.userIdProvider = userIdProvider;
            this.dataService = dataService;
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
            var role = dataService.GetById<Role>(currentUser.RoleId);

            var result = new List<ActionDto>();

            if (ids.Count() == 1)
            {
                var id = ids.First();

                var entity = dbSet.GetById(id);
                var actions = actionService.GetActions();

                return actions.Where(x => x.IsAvailable(role, entity))
                    .Select(action => new ActionDto
                {
                    Color = action.Color.ToString().ToLowerfirstLetter(),
                    Name = action.GetType().Name.ToLowerfirstLetter(),
                    Ids = new List<string>
                    {
                        id.ToString()
                    }
                });
            }
            else
            {
                var actions = actionService.GetGroupActions();
                var entities = ids.Select(id => dbSet.GetById(id));

                foreach (var action in actions)
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
            var role = dataService.GetById<Role>(currentUser.RoleId);
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
            if (ids.Count() == 1)
                return InvokeAction(name, ids.First());

            var action = actionService.GetGroupActions().FirstOrDefault(x => x.GetType().Name.ToLowerfirstLetter() == name);
            
            if(action == null)
                return new AppActionResult
                {
                    IsError = true,
                    Message = $"Action {name} not found"
                };

            var currentUser = userIdProvider.GetCurrentUser();
            var role = dataService.GetById<Role>(currentUser.RoleId);
            var entities = dataService.GetDbSet<TEntity>().Where(i => ids.Contains(i.Id));

            
            if (action.IsAvailable(role, entities)) 
                return action.Run(currentUser, entities);

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
        
        public IEnumerable<ValidateResult> ImportFromExcel(Stream fileStream)
        {
            var excel = new ExcelPackage(fileStream);
            var workSheet = excel.Workbook.Worksheets.ElementAt(0);

            var dtos = workSheet.ConvertSheetToObjects<TFormDto>(out string parseErrors);
            if (!string.IsNullOrEmpty(parseErrors))
            {
                return new[] { new ValidateResult(parseErrors) };
            }

            return Import(dtos);
        }
        
        public Stream ExportToExcel()
        {
            var excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add(typeof(TEntity).Name);
            var entities = dataService.GetDbSet<TEntity>().ToList();
            var dtos = entities.Select(MapFromEntityToDto);
            workSheet.ConvertObjectsToSheet(dtos);//.Cells[1, 1].LoadFromCollection(dtos);
            
            return new MemoryStream(excel.GetAsByteArray());
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