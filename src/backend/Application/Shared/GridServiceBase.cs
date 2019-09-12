using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using DAL.Queries;
using Domain.Persistables;
using Domain.Services;
using Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace Application.Shared
{
    public abstract class GridServiceBase<TEntity, TDto> where TEntity : class, IPersistable, new() where TDto: IDto
    {
        public abstract DbSet<TEntity> UseDbSet(AppDbContext dbContext);
        public abstract void MapFromDtoToEntity(TEntity entity, TDto dto);
        public abstract TDto MapFromEntityToDto(TEntity entity);

        protected AppDbContext db;

        protected GridServiceBase(AppDbContext appDbContext)
        {
            db = appDbContext;
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
                query = query.Where(customer =>
                    stringProperties.Any(prop =>
                        prop.GetValue(customer, null) == form.Search));
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
    }
}