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
using Domain.Services.Shippings;

namespace Application.Shared.Excel
{
    public class ExcelDoubleMapper<TDto,TFormDto, TInnerDto>
        where TDto : new()
        where TFormDto : new()
        where TInnerDto : new()
    {
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
                    column.FillValue( entry , cell);
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

        public IEnumerable<ValidatedRecord<TDto>> LoadEntries(ExcelWorksheet worksheet)
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

            columnTitles = Unlocalize(columnTitles, _columns.Values.Select(x => x.Field)).ToList();

            FillColumnOrder(columnTitles);

            foreach (int rowIndex in rows.Skip(1))
            {
                bool isEmpty = IsEmptyRow(worksheet, rowIndex);
                if (isEmpty)
                {
                    continue;
                }

                var entity = new TDto();
                var validationResult = new DetailedValidationResult();

                foreach (var column in _columns.Values)
                {
                    try
                    {
                        var cell = worksheet.Cells[rowIndex, column.ColumnIndex];
                        var columnResult = column.SetValue(entity, cell);

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

                ;

                _errors.Add(validationResult);

                yield return new ValidatedRecord<TDto>(entity, validationResult);
            }
        }

        public IEnumerable<ValidateResult> Errors => _errors;

        private void FillColumnTitles(string lang)
        {
            foreach (var column in _columns.Where(c => c.Value.ColumnIndex >= 0))
            {
                Translation local = _translations.FirstOrDefault(t => t.Name == column.Value.Field.DisplayNameKey);
                column.Value.Title = (lang == "en" ? local?.En : local?.Ru) ?? column.Key.Split('_')[1];
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
            var fieldNamesSet = fields.ToDictionary(x => x.DisplayNameKey);
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

        private bool IsEmptyRow(ExcelWorksheet worksheet, int rowIndex)
        {
            foreach (var column in _columns.Values)
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