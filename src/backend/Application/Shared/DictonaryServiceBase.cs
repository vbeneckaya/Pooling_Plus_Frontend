using Application.BusinessModels.Shared.Handlers;
using Application.Services.Triggers;
using Application.Shared.Excel;
using DAL.Queries;
using DAL.Services;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.AppConfiguration;
using Domain.Services.FieldProperties;
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
    public abstract class DictionaryServiceBase<TEntity, TListDto> where TEntity : class, IPersistable, new() where TListDto: IDto, new()
    {
        public abstract DetailedValidationResult MapFromDtoToEntity(TEntity entity, TListDto dto);
        public abstract TListDto MapFromEntityToDto(TEntity entity);


        protected readonly ICommonDataService _dataService;

        protected readonly IUserProvider _userProvider;

        protected readonly ITriggersService _triggersService;

        protected readonly IFieldDispatcherService _fieldDispatcherService;

        private readonly IValidationService _validationService;

        private readonly IFieldSetterFactory _fieldSetterFactory;

        private readonly IAppConfigurationService _configurationService;

        protected DictionaryServiceBase(ICommonDataService dataService, IUserProvider userProvider, ITriggersService triggersService, 
                                       IValidationService validationService, IFieldDispatcherService fieldDispatcherService, 
                                       IFieldSetterFactory fieldSetterFactory, IAppConfigurationService configurationService)
        {
            _dataService = dataService;
            _userProvider = userProvider;
            _triggersService = triggersService;
            _validationService = validationService;
            _fieldDispatcherService = fieldDispatcherService;
            _fieldSetterFactory = fieldSetterFactory;
            _configurationService = configurationService;
        }

        protected virtual IFieldSetter<TEntity> ConfigureHandlers(IFieldSetter<TEntity> setter, TListDto dto)
        {
            return null;
        }
        protected virtual IChangeTracker ConfigureChangeTacker()
        {
            return null;
        }

        public TListDto Get(Guid id)
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var entity = _dataService.GetDbSet<TEntity>().GetById(id);
            Log.Information("{entityName}.Get (Load from DB): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var result = MapFromEntityToDto(entity);
            Log.Information("{entityName}.Get (Convert to DTO): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            result = FillLookupNames(result);
            Log.Information("{entityName}.Get (Fill lookups): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

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

        public virtual IQueryable<TEntity> ApplyRestrictions(IQueryable<TEntity> query)
        {
            return query;
        }

        public SearchResult<TListDto> Search(SearchFormDto form)
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var dbSet = _dataService.GetDbSet<TEntity>();
            var query = this.ApplySearch(dbSet, form);
            query = ApplyRestrictions(query);

            Log.Information("{entityName}.Search (Apply search parameters): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            if (form.Take == 0)
                form.Take = 1000;
            
            var totalCount = query.Count();
            var entities = ApplySort(query, form)
                .Skip(form.Skip)
                .Take(form.Take).ToList();
            Log.Information("{entityName}.Search (Load from DB): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var a = new SearchResult<TListDto>
            {
                TotalCount = totalCount,
                Items = entities.Select(entity => MapFromEntityToDto(entity)).ToList()
            };
            Log.Information("{entityName}.Search (Convert to DTO): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            a.Items = FillLookupNames(a.Items).ToList();
            Log.Information("{entityName}.Search (Fill lookups): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            return a;
        }

        protected virtual IQueryable<TEntity> ApplySearch(IQueryable<TEntity> query, SearchFormDto form)
        {
            if (string.IsNullOrEmpty(form.Search)) return query;

            var stringProperties = typeof(TEntity).GetProperties()
                .Where(prop => prop.PropertyType == form.Search.GetType());

            return query.Where(customer =>
                stringProperties.Any(prop =>
                    ((string)prop.GetValue(customer, null) ?? "").ToLower().Contains(form.Search.ToLower())));
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

                record.Result.ResultType = validationResult.ResultType;

                if (validationResult.IsError)
                {
                    record.Result.AddErrors(validationResult.Errors);
                }
            }

            Log.Information("{entityName}.Import (Import): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var importResult = new ImportResult();
            importResult.Results.AddRange(entitiesFrom.Select(i => i.Result));

            var result = MapFromImportResult(importResult);
            Log.Information("{entityName}.Import (Convert result to DTO): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

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
            var workSheet = excel.Workbook.Worksheets[0];//.ElementAt(0);

            var excelMapper = CreateExcelMapper();
            var dtos = excelMapper.LoadEntries(workSheet).ToList();
            Log.Information("{entityName}.ImportFromExcel (Load from file): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var importResult = Import(dtos);
            Log.Information("{entityName}.ImportFromExcel (Import): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

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
            Log.Information("{entityName}.ExportToExcel (Apply search parameters): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var entities = ApplySort(query, form).ToList();
            Log.Information("{entityName}.ExportToExcel (Load from DB): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var dtos = entities.Select(MapFromEntityToDto);
            Log.Information("{entityName}.ExportToExcel (Convert to DTO): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            dtos = FillLookupNames(dtos);
            Log.Information("{entityName}.ExportToExcel (Fill lookups): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            var excelMapper = CreateExcelMapper();
            excelMapper.FillSheet(workSheet, dtos, user.Language);
            Log.Information("{entityName}.ExportToExcel (Fill file): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            return new MemoryStream(excel.GetAsByteArray());
        }

        public DetailedValidationResult SaveOrCreate(TListDto entityFrom)
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

        protected DetailedValidationResult SaveOrCreateInner(TListDto dto, bool isImport)
        {
            string entityName = typeof(TEntity).Name;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var dbSet = _dataService.GetDbSet<TEntity>();

            var entity = isImport ? FindByKey(dto) : FindById(dto);
            var isNew = entity == null;
            Log.Information("{entityName}.SaveOrCreateInner (Find entity): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            if (isNew)
            {
                entity = new TEntity
                {
                    Id = Guid.NewGuid()
                };
            }
            else if (isImport)
            {
                dto.Id = entity.Id.ToString();
            }

            // Validation step

            var result = ValidateDto(dto);
            
            if (result.IsError)
            {
                Log.Information($"Не удалось сохранить запись в справочник {typeof(TEntity)}: {result.Error}.");
                return result;
            }

            // Mapping

            MapFromDtoToEntity(entity, dto);

            if (isNew)
            {
                dbSet.Add(entity);
            }

            var changes = this._dataService.GetChanges<TEntity>().FirstOrDefault(x => x.Entity.Id == entity.Id);

            // Change handlers

            var setter = this.ConfigureHandlers(this._fieldSetterFactory.Create<TEntity>(), dto);

            if (setter != null)
            {
                setter.Appy(changes);
            }
            
            var trackConfig = this.ConfigureChangeTacker();

            if (trackConfig != null)
            {
                trackConfig.LogTrackedChanges(changes);
            }

            Log.Information("{entityName}.SaveOrCreateInner (Apply updates): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
            sw.Restart();

            if (isNew)
            {
                result.ResultType = ValidateResultType.Created;
            }
            else
            {
                //dbSet.Update(entityFromDb);
                result.ResultType = ValidateResultType.Updated;
            }

                _triggersService.Execute();
                Log.Information("{entityName}.SaveOrCreateInner (Execure triggers): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);
                sw.Restart();

                _dataService.SaveChanges();
                Log.Information("{entityName}.SaveOrCreateInner (Save changes): {ElapsedMilliseconds}ms", entityName, sw.ElapsedMilliseconds);

            Log.Information($"Запись {entity.Id} в справочнике {typeof(TEntity)} {(isNew ? "создана" : "обновлена")}.");

            return result;
        }
        
        protected virtual DetailedValidationResult ValidateDto(TListDto dto)
        {
            return _validationService.Validate(dto);
        }

        protected virtual IEnumerable<TListDto> FillLookupNames(IEnumerable<TListDto> dtos)
        {
            return dtos;
        }

        protected TListDto FillLookupNames(TListDto dto)
        {
            return FillLookupNames(new[] { dto }).FirstOrDefault();
        }

        protected T MapFromStateDto<T>(string dtoStatus) where T : struct
        {
            var mapFromStateDto = Enum.Parse<T>(dtoStatus, true);

            return mapFromStateDto;
        }

        protected virtual ExcelMapper<TListDto> CreateExcelMapper()
        {
            return new ExcelMapper<TListDto>(_dataService, _userProvider, _fieldDispatcherService);
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