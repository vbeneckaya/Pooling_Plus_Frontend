using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DAL;
using DAL.Queries;
using Domain.Persistables;
using Domain.Services;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Application.Shared
{
    public abstract class DictonaryServiceBase<TEntity, TListDto> where TEntity : class, IPersistable, new() where TListDto: IDto, new()
    {
        public abstract DbSet<TEntity> UseDbSet(AppDbContext dbContext);
        public abstract void MapFromDtoToEntity(TEntity entity, TListDto dto);
        public abstract TListDto MapFromEntityToDto(TEntity entity);

        protected AppDbContext db ;

        protected DictonaryServiceBase(AppDbContext appDbContext)
        {
            db = appDbContext;
        }

        public TListDto Get(Guid id)
        {
            var dbSet = UseDbSet(db);
            return MapFromEntityToDto(dbSet.GetById(id));
        }

        public virtual IEnumerable<LookUpDto> ForSelect()
        {
            return new List<LookUpDto>();
        }

        public virtual TEntity FindByKey(TListDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id) && Guid.TryParse(dto.Id, out Guid id))
            {
                var dbSet = UseDbSet(db);
                return dbSet.GetById(id);
            }
            else
            {
                return null;
            }
        }

        public SearchResult<TListDto> Search(SearchFormDto form)
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

            if (form.Take == 0)
                form.Take = 1000;
            
            var totalCount = query.Count();
            var entities = query.Skip(form.Skip)
                .Take(form.Take).ToList();

            var a = new SearchResult<TListDto>
            {
                TotalCount = totalCount,
                Items = entities.Select(entity => MapFromEntityToDto(entity))
            };
            return a;
        }

        public IEnumerable<ValidateResult> Import(IEnumerable<TListDto> entitiesFrom)
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

            var dtos = workSheet.ConvertSheetToObjects<TListDto>(out string parseErrors);
            if (!string.IsNullOrEmpty(parseErrors))
            {
                return new ValidateResult(parseErrors);
            }

            int ind = 1;
            StringBuilder errors = new StringBuilder();
            var results = Import(dtos);
            foreach (ValidateResult result in results)
            {
                ++ind;
                if (result.IsError)
                {
                    errors.AppendLine($"Строка {ind}: {result.Error}.");
                }
            }

            return new ValidateResult(errors.ToString());
        }
        
        
        
        public ValidateResult SaveOrCreate(TListDto entityFrom)
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