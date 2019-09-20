using System;
using System.Collections.Generic;
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

namespace Application.Shared
{
    public abstract class GridServiceBase<TEntity, TDto> where TEntity : class, IPersistable, new() where TDto: IDto
    {
        public abstract DbSet<TEntity> UseDbSet(AppDbContext dbContext);
        public abstract IEnumerable<IAction<TEntity>> Actions();
        public abstract IEnumerable<IAction<IEnumerable<TEntity>>> GroupActions();
        public abstract void MapFromDtoToEntity(TEntity entity, TDto dto);
        public abstract TDto MapFromEntityToDto(TEntity entity);

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

        public IEnumerable<TDto> Search(SearchForm form)
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

            var entities = query.Skip(form.Skip)
                .Take(form.Take).ToList();

            return entities.Select(entity => MapFromEntityToDto(entity));
        }

        public ValidateResult SaveOrCreate(TDto entityFrom)
        {
            var dbSet = UseDbSet(db);
            if (!string.IsNullOrEmpty(entityFrom.Id))
            {
                var entityFromDb = dbSet.GetById(Guid.Parse(entityFrom.Id));
                if (entityFromDb != null)
                {
                    MapFromDtoToEntity(entityFromDb, entityFrom);
                    
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
            MapFromDtoToEntity(entity, entityFrom);
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
            if (action.IsAvailable(role, entity)) 
                action.Run(currentUser, entity);
            
            return new AppActionResult
            {
                IsError = false,
                Message = "Done"
            };
        }
        
        public AppActionResult InvokeAction(string name, IEnumerable<Guid> ids)
        {
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
                action.Run(currentUser, entities);

            return new AppActionResult
            {
                IsError = false,
                Message = "Done"
            };
        }        
    }
}