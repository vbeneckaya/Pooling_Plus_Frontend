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
    public abstract class GridServiceBase<TEntity, TDto, TFormDto> where TEntity : class, IPersistable, new() where TDto : IDto, new() where TFormDto : IDto, new()
    {
        public abstract DbSet<TEntity> UseDbSet(AppDbContext dbContext);
        public abstract IEnumerable<IAction<TEntity>> Actions();
        public abstract IEnumerable<IAction<IEnumerable<TEntity>>> GroupActions();
        public abstract ValidateResult MapFromDtoToEntity(TEntity entity, TDto dto);
        public abstract ValidateResult MapFromFormDtoToEntity(TEntity entity, TFormDto dto);
        public abstract TDto MapFromEntityToDto(TEntity entity);
        public abstract TFormDto MapFromEntityToFormDto(TEntity entity);
        public abstract LookUpDto MapFromEntityToLookupDto(TEntity entity);

        protected virtual void ApplyAfterSaveActions(TEntity entity, TDto dto) { }

        protected AppDbContext db;
        private readonly IUserIdProvider userIdProvider;
        
        protected GridServiceBase(AppDbContext appDbContext, IUserIdProvider userIdProvider)
        {
            db = appDbContext;
            this.userIdProvider = userIdProvider;
        }

        public TDto Get(Guid id)
        {
            var dbSet = UseDbSet(db);
            return MapFromEntityToDto(dbSet.GetById(id));
        }

        public TFormDto GetForm(Guid id)
        {
            var dbSet = UseDbSet(db);
            return MapFromEntityToFormDto(dbSet.GetById(id));
        }

        public IEnumerable<LookUpDto> ForSelect()
        {
            var dbSet = UseDbSet(db);
            return  dbSet.ToList().Select(MapFromEntityToLookupDto);
        }

        public SearchResult<TDto> Search(SearchForm form)
        {
            var dbSet = UseDbSet(db);
            var query = dbSet.AsQueryable();


            if (!string.IsNullOrEmpty(form.Search))
            {
                var stringProperties = typeof(TEntity).GetProperties().Where(prop =>
                    prop.PropertyType == form.Search.GetType());
         
                //TODO Вернуть полнотекстовый поиск
                query = query.Where(entity =>  entity.Id.ToString() == form.Search);
            }

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
            var dbSet = UseDbSet(db);
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
                    
                    db.SaveChanges();
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

            db.SaveChanges();
            return new ValidateResult
            {
                Id = entity.Id.ToString()
            };
        }

        public IEnumerable<ActionDto> GetActions(IEnumerable<Guid> ids)
        {
            if (ids == null) 
                throw new ArgumentNullException(nameof(ids));
            
            var dbSet = UseDbSet(db);
            var currentUser = userIdProvider.GetCurrentUser();
            var role = db.Roles.GetById(currentUser.RoleId);

            var result = new List<ActionDto>();

            if (ids.Count() == 1)
            {
                var id = ids.First();

                var entity = dbSet.GetById(id);
                var actions = Actions();

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
                var actions = GroupActions();
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
            var action = Actions().FirstOrDefault(x => x.GetType().Name.ToLowerfirstLetter() == name);
            
            if(action == null)
                return new AppActionResult
                {
                    IsError = true,
                    Message = $"Action {name} not found"
                };

            var currentUser = userIdProvider.GetCurrentUser();
            var role = db.Roles.GetById(currentUser.RoleId);
            var dbSet = UseDbSet(db);
            var entity = dbSet.GetById(id);
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

            var action = GroupActions().FirstOrDefault(x => x.GetType().Name.ToLowerfirstLetter() == name);
            
            if(action == null)
                return new AppActionResult
                {
                    IsError = true,
                    Message = $"Action {name} not found"
                };

            var currentUser = userIdProvider.GetCurrentUser();
            var role = db.Roles.GetById(currentUser.RoleId);
            var dbSet = UseDbSet(db);

            var entities = ids.Select(dbSet.GetById);
            
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
            var dbSet = UseDbSet(db);
            var entities = dbSet.ToList();
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
    }
}