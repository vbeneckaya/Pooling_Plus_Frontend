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
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Application.Shared
{
    public abstract class DictonaryServiceBase<TEntity, TListDto> where TEntity : class, IPersistable, new() where TListDto: IDto, new()
    {
        public abstract DetailedValidationResult MapFromDtoToEntity(TEntity entity, TListDto dto);
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
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var entity = _dataService.GetDbSet<TEntity>().GetById(id);
            Log.Debug("{entityName}.Get (Load from DB): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var result = MapFromEntityToDto(entity);
            Log.Debug("{entityName}.Get (Convert to DTO): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            return result;
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
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var dbSet = _dataService.GetDbSet<TEntity>();
            var query = this.ApplySearch(dbSet, form);
            Log.Debug("{entityName}.Search (Apply search parameters): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            if (form.Take == 0)
                form.Take = 1000;
            
            var totalCount = query.Count();
            var entities = ApplySort(query, form)
                .Skip(form.Skip)
                .Take(form.Take).ToList();
            Log.Debug("{entityName}.Search (Load from DB): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var a = new SearchResult<TListDto>
            {
                TotalCount = totalCount,
                Items = entities.Select(entity => MapFromEntityToDto(entity)).ToList()
            };
            Log.Debug("{entityName}.Search (Convert to DTO): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

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

        public ImportResultDto Import(IEnumerable<ValidatedRecord<TListDto>> entitiesFrom)
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            foreach (var record in entitiesFrom)
            {
                if (record.Result.IsError) continue;

                var validationResult = SaveOrCreateInner(record.Data, true);

                if (validationResult.IsError)
                {
                    record.Result.AddErrors(validationResult.Errors);
                }
            }

            Log.Debug("{entityName}.Import (Import): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var importResult = new ImportResult();
            importResult.Results.AddRange(entitiesFrom.Select(i => i.Result));

            var result = MapFromImportResult(importResult);
            Log.Debug("{entityName}.Import (Convert result to DTO): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            return result;
        }

        public ImportResultDto Import(IEnumerable<TListDto> entitiesFrom)
        {
            return Import(entitiesFrom.Select(i => new ValidatedRecord<TListDto>(i)));
        }

        public ImportResultDto ImportFromExcel(Stream fileStream)
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var excel = new ExcelPackage(fileStream);
            var workSheet = excel.Workbook.Worksheets.ElementAt(0);

            var excelMapper = CreateExcelMapper();
            var dtos = excelMapper.LoadEntries(workSheet).ToList();
            Log.Debug("{entityName}.ImportFromExcel (Load from file): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            //if (excelMapper.Errors.Any(e => e.IsError))
            //{
            //    var result = new ImportResult();
            //    result.Results.AddRange(excelMapper.Errors);

            //    return MapFromImportResult(result);
            //}

            var importResult = Import(dtos);
            Log.Debug("{entityName}.ImportFromExcel (Import): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

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

        public Stream ExportToExcel(SearchFormDto form)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var excel = new ExcelPackage();

            var user = _userProvider.GetCurrentUser();

            string entityName = typeof(TEntity).Name.Pluralize().ToLowerFirstLetter();
            string entityDisplayName = entityName.Translate(user.Language);
            var workSheet = excel.Workbook.Worksheets.Add(entityDisplayName);
            
            var dbSet = _dataService.GetDbSet<TEntity>();
            var query = ApplySearch(dbSet, form);
            Log.Debug("{entityName}.ExportToExcel (Apply search parameters): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var entities = ApplySort(query, form).ToList();
            Log.Debug("{entityName}.ExportToExcel (Load from DB): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var dtos = entities.Select(MapFromEntityToDto);
            Log.Debug("{entityName}.ExportToExcel (Convert to DTO): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var excelMapper = CreateExcelMapper();
            excelMapper.FillSheet(workSheet, dtos, user.Language);
            Log.Debug("{entityName}.ExportToExcel (Fill file): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

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

        protected DetailedValidationResult SaveOrCreateInner(TListDto entityFrom, bool isImport)
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var dbSet = _dataService.GetDbSet<TEntity>();

            var entityFromDb = isImport ? FindByKey(entityFrom) : FindById(entityFrom);
            var isNew = entityFromDb == null;
            Log.Debug("{entityName}.SaveOrCreateInner (Find entity): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            if (isNew)
            {
                entityFromDb = new TEntity
                {
                    Id = Guid.NewGuid()
                };
            }
            else if (isImport)
            {
                entityFrom.Id = entityFromDb.Id.ToString();
            }

            var result = MapFromDtoToEntity(entityFromDb, entityFrom);
            Log.Debug("{entityName}.SaveOrCreateInner (Apply updates): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

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
                Log.Debug("{entityName}.SaveOrCreateInner (Save changes): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

                Log.Information($"Запись {entityFromDb.Id} в справочнике {typeof(TEntity)} {(isNew ? "создана" : "обновлена")}.");
            }
            else
            {
                Log.Information($"Не удалось сохранить запись в справочник {typeof(TEntity)}: {result.Error}.");
            }

            return result;
        }

        protected T MapFromStateDto<T>(string dtoStatus) where T : struct
        {
            var mapFromStateDto = Enum.Parse<T>(dtoStatus, true);

            return mapFromStateDto;
        }

        protected virtual ExcelMapper<TListDto> CreateExcelMapper()
        {
            return new ExcelMapper<TListDto>(_dataService, _userProvider);
        }

        public ValidateResult Delete(Guid id)
        {
            var entity = _dataService.GetById<TEntity>(id);

            if (entity == null) return new ValidateResult("Запись не найдена", id.ToString());

            _dataService.Remove(entity);
            _dataService.SaveChanges();

            return new ValidateResult(null, id.ToString());
        }
    }
}