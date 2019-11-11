using Application.Shared.Excel;
using DAL.Queries;
using DAL.Services;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;
using OfficeOpenXml;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Application.Shared
{
    public abstract class DictonaryServiceBase<TEntity, TListDto> where TEntity : class, IPersistable, new() where TListDto: IDto, new()
    {
        public abstract ValidateResult MapFromDtoToEntity(TEntity entity, TListDto dto);
        public abstract TListDto MapFromEntityToDto(TEntity entity);

        protected readonly ICommonDataService _dataService;
        protected readonly IUserProvider _userProvider;

        protected DictonaryServiceBase(ICommonDataService dataService, IUserProvider userProvider)
        {
            _dataService = dataService;
            _userProvider = userProvider;
        }

        public TListDto Get(Guid id)
        {
            var dbSet = _dataService.GetDbSet<TEntity>();
            return MapFromEntityToDto(dbSet.GetById(id));
        }

        public virtual IEnumerable<LookUpDto> ForSelect()
        {
            return new List<LookUpDto>();
        }

        public virtual TEntity FindByKey(TListDto dto)
        {
            return FindById(dto);
        }

        public SearchResult<TListDto> Search(SearchFormDto form)
        {
            var dbSet = _dataService.GetDbSet<TEntity>();
            var query = this.ApplySearch(dbSet, form);

            if (form.Take == 0)
                form.Take = 1000;
            
            var totalCount = query.Count();
            var entities = ApplySort(query, form)
                .Skip(form.Skip)
                .Take(form.Take).ToList();

            var a = new SearchResult<TListDto>
            {
                TotalCount = totalCount,
                Items = entities.Select(entity => MapFromEntityToDto(entity))
            };
            return a;
        }

        protected virtual IQueryable<TEntity> ApplySearch(IQueryable<TEntity> query, SearchFormDto form)
        {
            if (string.IsNullOrEmpty(form.Search)) return query;

            var stringProperties = typeof(TEntity).GetProperties()
                .Where(prop => prop.PropertyType == form.Search.GetType());

            return query.Where(customer =>
                stringProperties.Any(prop =>
                    (string)prop.GetValue(customer, null) == form.Search));
        }

        protected virtual IQueryable<TEntity> ApplySort(IQueryable<TEntity> query, SearchFormDto form)
        {
            return query.OrderBy(i => i.Id);
        }

        public ImportResultDto Import(IEnumerable<TListDto> entitiesFrom)
        {
            var result = new ImportResult();
            
            foreach (var dto in entitiesFrom)
                result.Results.Add(SaveOrCreateInner(dto, true));

            return MapFromImportResult(result);
        }
        
        public ImportResultDto ImportFromExcel(Stream fileStream)
        {
            var excel = new ExcelPackage(fileStream);
            var workSheet = excel.Workbook.Worksheets.ElementAt(0);

            var excelMapper = CreateExcelMapper();
            var dtos = excelMapper.LoadEntries(workSheet).ToList();

            if (excelMapper.Errors.Any(e => e.IsError))
            {
                string errors = string.Join(". ", excelMapper.Errors.Where(x => x.IsError).Select(x => x.Error));
                var result = new ImportResult();
                result.Results.AddRange(excelMapper.Errors);

                return MapFromImportResult(result);
            }

            var importResult = Import(dtos);

            return importResult;
        }

        private ImportResultDto MapFromImportResult(ImportResult importResult)
        {
            var user = _userProvider.GetCurrentUser();

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("validation.createdCountMessage".Translate(user.Language, importResult.CreatedCount));
            sb.AppendLine("validation.updatedCountMessage".Translate(user.Language, importResult.UpdatedCount));

            if (importResult.DuplicatedRecordErrorsCount > 0)
            { 
                sb.AppendLine("validation.duplicatedRecordErrorMessage".Translate(user.Language, importResult.DuplicatedRecordErrorsCount));
            }

            if (importResult.InvalidDictionaryValueErrorsCount > 0)
            {
                sb.AppendLine("validation.invalidDictionaryValueErrorMessage".Translate(user.Language, importResult.InvalidDictionaryValueErrorsCount));
            }

            if (importResult.InvalidValueFormatErrorsCount > 0)
            {
                sb.AppendLine("validation.invalidFormatErrorCountMessage".Translate(user.Language, importResult.InvalidValueFormatErrorsCount));
            }

            if (importResult.RequiredErrorsCount > 0)
            {
                sb.AppendLine("validation.requiredErrorMessage".Translate(user.Language, importResult.RequiredErrorsCount));
            }

            return new ImportResultDto
            {
                Message = sb.ToString()
            };
        }

        public Stream ExportToExcel()
        {
            var excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add(typeof(TEntity).Name);
            var dbSet = _dataService.GetDbSet<TEntity>();
            var entities = dbSet.ToList();
            var dtos = entities.Select(MapFromEntityToDto);

            var user = _userProvider.GetCurrentUser();

            var excelMapper = CreateExcelMapper();
            excelMapper.FillSheet(workSheet, dtos, user.Language);

            return new MemoryStream(excel.GetAsByteArray());
        }

        public ValidateResult SaveOrCreate(TListDto entityFrom)
        {
            return SaveOrCreateInner(entityFrom, false);
        }

        protected TEntity FindById(TListDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Id) && Guid.TryParse(dto.Id, out Guid id))
            {
                var dbSet = _dataService.GetDbSet<TEntity>();
                return dbSet.GetById(id);
            }
            else
            {
                return null;
            }
        }

        protected ValidateResult SaveOrCreateInner(TListDto entityFrom, bool isImport)
        {
            var dbSet = _dataService.GetDbSet<TEntity>();

            var entityFromDb = isImport ? FindByKey(entityFrom) : FindById(entityFrom);
            var isNew = entityFromDb == null;

            if (isNew)
            {
                entityFromDb = new TEntity
                {
                    Id = Guid.NewGuid()
                };
            }

            var result = MapFromDtoToEntity(entityFromDb, entityFrom);

            if (!result.IsError)
            {
                if (isNew)
                {
                    dbSet.Add(entityFromDb);
                    result.ResultType = ValidateResultType.Created;
                }
                else
                {
                    dbSet.Update(entityFromDb);
                    result.ResultType = ValidateResultType.Updated;
                }

                _dataService.SaveChanges();

                Log.Information($"Запись {entityFromDb.Id} в справочнике {typeof(TEntity)} {(isNew ? "создана" : "обновлена")}.");
            }
            else
            {
                Log.Information($"Не удалось сохранить запись в справочник {typeof(TEntity)}: {result.Error}.");
            }

            return result;
        }

        protected virtual ExcelMapper<TListDto> CreateExcelMapper()
        {
            return new ExcelMapper<TListDto>(_dataService, _userProvider);
        }
    }
}