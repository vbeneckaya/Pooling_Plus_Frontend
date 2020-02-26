using Application.Shared.Excel.Columns;
using DAL.Services;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services.FieldProperties;
using Domain.Services.UserProvider;
using Domain.Shared;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Domain.Services.Shippings;
using Microsoft.EntityFrameworkCore.Internal;

namespace Application.Shared.Excel
{
    public class ExcelDoubleMapper<TDto, TFormDto, TInnerDto>
        where TDto : new()
        where TFormDto : new()
        where TInnerDto : new()
    {
        private readonly string _delimeter = "`";

        public void AddColumn(IExcelColumn column)
        {
            string columnKey = GetColumnKey(column);
            column.Field = _fieldDispatcherService.GetDtoFields<TDto>()
                .FirstOrDefault(x => x.Name == column.Property.Name);
            _columns[columnKey] = column;
        }

        public void AddInnerColumn(IExcelColumn column)
        {
            string columnKey = GetColumnKey(column);
            column.Field = _fieldDispatcherService.GetDtoFields<TInnerDto>()
                .FirstOrDefault(x => x.Name == column.Property.Name);
            _columns[columnKey] = column;
        }

        public void FillSheet(ExcelWorksheet worksheet, IEnumerable<TFormDto> entries, string lang,
            List<string> columns = null)
        {
            FillDefaultColumnOrder(columns);
            FillColumnTitles(lang);

            foreach (var column in _columns.Values.Where(c => c.ColumnIndex >= 0))
            {
                worksheet.Cells[1, column.ColumnIndex].Value = column.Title;
                worksheet.Cells[1, column.ColumnIndex].Style.Font.Bold = true;
            }

            ;

            int rowIndex = 2;
            foreach (var entry in entries)
            {
                foreach (var column in _columns.Values.Where(c =>
                    c.ColumnIndex >= 0 && c.Property.ReflectedType == typeof(TDto)))
                {
                    var cell = worksheet.Cells[rowIndex, column.ColumnIndex];
                    column.FillValue(entry, cell);
                }

                var innerEntries = new List<TInnerDto>();

                ///todo: убратиь инлайн
                innerEntries = (List<TInnerDto>) typeof(TFormDto).GetProperty("Orders").GetValue(entry);

                foreach (var innerEntry in innerEntries)
                {
                    foreach (var column in _columns.Values.Where(c =>
                        c.ColumnIndex >= 0 && c.Property.ReflectedType == typeof(TInnerDto)))
                    {
                        var cell = worksheet.Cells[rowIndex, column.ColumnIndex];
                        column.FillValue(innerEntry, cell);
                    }

                    ++rowIndex;
                }

                if (!innerEntries.Any())
                    ++rowIndex;
            }
        }

        public IEnumerable<ValidatedRecord<TFormDto>> LoadEntries(ExcelWorksheet worksheet)
        {
            var rows = worksheet.Cells
                .Select(cell => cell.Start.Row)
                .Distinct()
                .OrderBy(x => x);

            if (!rows.Any())
            {
                yield break;
            }

            int headRowIndex = rows.First();

            int maxColumnInd = worksheet.Cells.Select(c => c.End.Column).Max();

            List<string> columnTitles = new List<string>();
            for (int colIndex = 1; colIndex <= maxColumnInd; colIndex++)
            {
                columnTitles.Add(worksheet.Cells[headRowIndex, colIndex]?.Value?.ToString());
            }

            var formType = typeof(TDto).Name;
            var formTypeTranslates = _translations.Where(t => t.Name == formType).Select(_ => _.Ru);
            formTypeTranslates =
                formTypeTranslates.Concat(_translations.Where(t => t.Name == formType).Select(_ => _.En));

            var columnFormTitles = columnTitles.Where(x => formTypeTranslates.Any(t => x.Contains(_delimeter + t)))
                .Select(_ => _.Split(_delimeter).FirstOrDefault());
            columnFormTitles = Unlocalize(columnFormTitles, _columns.Values.Select(x => x.Field)).ToList();


            var formInnerType = typeof(TInnerDto).Name;
            var formInnerTypeTranslates = _translations.Where(t => t.Name == formInnerType).Select(_ => _.Ru);
            formInnerTypeTranslates =
                formInnerTypeTranslates.Concat(_translations.Where(t => t.Name == formInnerType).Select(_ => _.En));

            var columnInnerTitles = columnTitles
                .Where(x => formInnerTypeTranslates.Any(t => x.Contains(_delimeter + t)))
                .Select(_ => _.Split(_delimeter).FirstOrDefault());
            columnInnerTitles = Unlocalize(columnInnerTitles, _columns.Values.Select(x => x.Field)).ToList();

            columnTitles = columnFormTitles.Select(c => formType.ToLower() + '_' + c)
                .Concat(columnInnerTitles.Select(ci => formInnerType.ToLower() + '_' + ci)).ToList();

            FillColumnOrder(columnTitles);

            var formColumns = _columns.Where(_ => _.Value.Property.DeclaringType.Name != typeof(TInnerDto).Name).ToDictionary(_=>_.Key, _=>_.Value);

            var formInnerColumns = _columns.Where(_ => _.Value.Property.DeclaringType.Name == typeof(TInnerDto).Name).ToDictionary(_=>_.Key, _=>_.Value);

            var entity = new TFormDto();
            var innerList = new List<TInnerDto>();
            var emptyFormRow = false;
            var validationResult = new DetailedValidationResult();
            
            foreach (int rowIndex in rows.Skip(1))
            {
                bool isEmpty = IsEmptyRow(worksheet, rowIndex, _columns);
                if (isEmpty)
                {
                    continue;
                }
                
                bool isEmptyForm = IsEmptyRow(worksheet, rowIndex, formColumns);
                
                if (!isEmptyForm && rowIndex != headRowIndex + 1)
                {
                    if (innerList.Any())
                    {
                        ///todo: убратиь инлайн
                        typeof(TFormDto).GetProperty("Orders").SetValue(entity, innerList);
                        
                        innerList = new List<TInnerDto>();
                    }

                    yield return new ValidatedRecord<TFormDto>( entity, validationResult);
                    
                    validationResult = new DetailedValidationResult();
                    
                    entity = new TFormDto();
                }
                
                bool isEmptyInnerForm = IsEmptyRow(worksheet, rowIndex, formInnerColumns);

                var innerEntity = new TInnerDto();
                
                foreach (var column in _columns.Values)
                {
                    try
                    {
                        var cell = worksheet.Cells[rowIndex, column.ColumnIndex];
                        ValidationResultItem columnResult;

                        if ( column.Property.DeclaringType != typeof(TInnerDto))
                        {
                            columnResult = column.SetValue(entity, cell);
                        }
                        else if (!isEmptyInnerForm)
                        {
                            columnResult = column.SetValue(innerEntity, cell); 
                        }
                        else
                        {
                            continue;
                        }

                        if (columnResult != null)
                        {
                            validationResult.AddError(_columns.Keys.ElementAt(column.ColumnIndex), columnResult.Message,
                                columnResult.ResultType);
                        }
                    }
                    catch (Exception ex)
                    {
                        validationResult.AddError("exception", $"Строка {rowIndex}: {ex.Message}.",
                            ValidationErrorType.Exception);
                    }
                }

                if (!isEmptyInnerForm)
                {
                    innerList.Add(innerEntity);
                }

                _errors.Add(validationResult);

               // yield return new ValidatedRecord<TFormDto>(entity, validationResult);
            }
        }

        public IEnumerable<ValidateResult> Errors => _errors;

        private void FillColumnTitles(string lang)
        {
            foreach (var column in _columns.Where(c => c.Value.ColumnIndex >= 0))
            {
                Translation localField = _translations.FirstOrDefault(t => t.Name == column.Value.Field.DisplayNameKey);
                Translation localGrid =
                    _translations.FirstOrDefault(t => t.Name == column.Value.Property.DeclaringType.Name);
                column.Value.Title =
                    (lang == "en" ? localField?.En + '.' + localGrid?.En : localField?.Ru + _delimeter + localGrid?.Ru)
                    ?? column.Key;
            }
        }

        private void FillDefaultColumnOrder(List<string> columns)
        {
            if (columns != null && columns.Any())
            {
                List<string> propNames = columns.Select(s => s.ToLower()).ToList();
                foreach (var column in _columns)
                {
                    column.Value.ColumnIndex = propNames.IndexOf(column.Key);
                }
            }
            else
            {
                foreach (var column in _columns)
                {
                    if (column.Value == null || column.Value.Field == null)
                    {
                        throw new Exception($"{column.Key} not found");
                    }

                    column.Value.ColumnIndex = column.Value.Field.OrderNumber;
                }
            }

            int columnIndex = 0;
            foreach (var column in _columns.Values.Where(c => c.ColumnIndex >= 0).OrderBy(c => c.ColumnIndex))
            {
                ++columnIndex;
                column.ColumnIndex = columnIndex;
            }
        }

        private void FillColumnOrder(List<string> columnTitles)
        {
            foreach (var columnKey in _columns.Keys.ToList())
            {
                int colInd = columnTitles.IndexOf(columnKey);
                if (colInd >= 0)
                {
                    IExcelColumn column = _columns[columnKey];
                    column.ColumnIndex = colInd + 1;
                }
                else
                {
                    _columns.Remove(columnKey);
                }
            }
        }

        private IEnumerable<string> Unlocalize(IEnumerable<string> titles, IEnumerable<FieldInfo> fields)
        {
            var fieldNamesSet = fields.GroupBy(_ => _.DisplayNameKey).Select(_ => _.FirstOrDefault())
                .ToDictionary(x => x.DisplayNameKey);

            foreach (string title in titles)
            {
                if (string.IsNullOrEmpty(title))
                {
                    yield return string.Empty;
                }
                else
                {
                    Translation local = _translations.FirstOrDefault(t =>
                        (t.Ru == title || t.En == title) && fieldNamesSet.ContainsKey(t.Name));
                    if (local == null)
                    {
                        yield return title?.ToLower();
                    }
                    else
                    {
                        yield return fieldNamesSet[local.Name].Name.ToLower();
                    }
                }
            }
        }

        private string GetColumnKey(IExcelColumn column)
        {
            var res = column.Property.ReflectedType.Name.ToLower() + "_" + column.Property.Name.ToLower();
            return res;
        }

        private bool IsEmptyRow(ExcelWorksheet worksheet, int rowIndex, Dictionary<string, IExcelColumn> columns)
        {
            foreach (var column in columns.Values)
            {
                if (column.ColumnIndex >= 0)
                {
                    var value = worksheet.Cells[rowIndex, column.ColumnIndex];
                    var strValue = value.Value?.ToString();
                    if (!string.IsNullOrEmpty(strValue))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        
    

        private void InitColumns()
        {
            Type type = typeof(TDto);
            string lang = _userProvider.GetCurrentUser()?.Language;

            _fieldDispatcherService.GetDtoFields<TDto>()
                .Where(f => f.FieldType != FieldType.Enum && f.FieldType != FieldType.State)
                .Select(f => new BaseExcelColumn {Property = type.GetProperty(f.Name), Field = f, Language = lang})
                .ToList()
                .ForEach(AddColumn);
        }

        public ExcelDoubleMapper(ICommonDataService dataService, IUserProvider userProvider,
            IFieldDispatcherService fieldDispatcherService)
        {
            _userProvider = userProvider;
            _fieldDispatcherService = fieldDispatcherService;
            _translations = dataService.GetDbSet<Translation>().ToList();
            InitColumns();
        }

        private readonly IUserProvider _userProvider;
        private readonly IFieldDispatcherService _fieldDispatcherService;
        private readonly List<Translation> _translations;
        private readonly Dictionary<string, IExcelColumn> _columns = new Dictionary<string, IExcelColumn>();
        private readonly List<ValidateResult> _errors = new List<ValidateResult>();
    }
}