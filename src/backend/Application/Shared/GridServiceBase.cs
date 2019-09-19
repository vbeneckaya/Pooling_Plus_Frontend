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
        public abstract IEnumerable<IAppAction<TEntity>> Actions();
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
            var dbSet = UseDbSet(db);
            var currentUser = userIdProvider.GetCurrentUser();
            var role = db.Roles.GetById(currentUser.RoleId);
            var actions = Actions();

            var result = new List<ActionDto>();
            foreach (var id in ids)
            {
                var entity = dbSet.GetById(id);

                foreach (var action in actions)
                {
                    if (action.IsAvalible(role, entity))
                    {
                        var actionDto = result.FirstOrDefault(x => x.Name == action.GetType().Name.ToLowerfirstLetter());
                        if (actionDto == null)
                        {
                            result.Add(new ActionDto
                            {
                                Color = action.Color.ToString().ToLowerfirstLetter(),
                                Name = action.GetType().Name.ToLowerfirstLetter(),
                                Ids = new List<string>
                                {
                                    id.ToString()
                                }
                            });                        
                        }

                        var newIds = actionDto.Ids.ToList();
                        newIds.Add(id.ToString());
                        actionDto.Ids = newIds;
                    }
                }
            }
            
            return result;
        }        
    }
}