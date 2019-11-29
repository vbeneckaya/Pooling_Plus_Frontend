using Application.Shared.Excel.Columns;
using DAL.Services;
using Domain.Persistables;
using Domain.Services.UserProvider;
using Domain.Shared;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Shared.Excel
{
    public class ExcelMapper<TDto> where TDto: new()
    {
        public void AddColumn(IExcelColumn column)
        {
            string columnKey = GetColumnKey(column);
            _columns[columnKey] = column;
        }

        public void FillSheet(ExcelWorksheet worksheet, IEnumerable<TDto> entries, string lang, List<string> columns = null)
        {
            FillDefaultColumnOrder(columns);
            FillColumnTitles(lang);

            foreach (var column in _columns.Values.Where(c => c.ColumnIndex >= 0))
            {
                worksheet.Cells[1, column.ColumnIndex].Value = column.Title;
                worksheet.Cells[1, column.ColumnIndex].Style.Font.Bold = true;
            };

            int rowIndex = 1;
            foreach (var entry in entries)
            {
                ++rowIndex;
                foreach (var column in _columns.Values.Where(c => c.ColumnIndex >= 0))
                {
                    var cell = worksheet.Cells[rowIndex, column.ColumnIndex];
                    column.FillValue(entry, cell);
                };
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

            columnTitles = Unlocalize(columnTitles, _columns.Keys).ToList();

            FillColumnOrder(columnTitles);

            foreach(int rowIndex in rows.Skip(1))
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
                            validationResult.AddError(_columns.Keys.ElementAt(column.ColumnIndex), columnResult.Message, columnResult.ResultType);
                        }

                    }
                    catch (Exception ex)
                    {
                        validationResult.AddError("exception", $"Строка {rowIndex}: {ex.Message}.", ValidationErrorType.Exception);
                    }
                };

                _errors.Add(validationResult);

                yield return new ValidatedRecord<TDto>(entity, validationResult);
            }
        }

        public IEnumerable<ValidateResult> Errors => _errors;

        private void FillColumnTitles(string lang)
        {
            foreach (var column in _columns.Where(c => c.Value.ColumnIndex >= 0))
            {
                Translation local = _translations.FirstOrDefault(t => t.Name?.ToLower() == column.Key);
                column.Value.Title = (lang == "en" ? local?.En : local?.Ru) ?? column.Key;
            }
        }

        private void FillDefaultColumnOrder(List<string> columns)
        {
            List<string> propNames;
            if (columns != null && columns.Any())
            {
                propNames = columns.Select(s => s.ToLower()).ToList();
            }
            else
            {
                propNames = typeof(TDto).GetProperties().Select(p => p.Name.ToLower()).ToList();
            }

            foreach (var column in _columns)
            {
                column.Value.ColumnIndex = propNames.IndexOf(column.Key);
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

        private IEnumerable<string> Unlocalize(IEnumerable<string> titles, IEnumerable<string> fields)
        {
            var fieldNamesSet = new HashSet<string>(fields.Select(x => x.ToLower()));
            foreach (string title in titles)
            {
                if (string.IsNullOrEmpty(title))
                {
                    yield return title;
                }
                else
                {
                    Translation local = _translations.FirstOrDefault(t => (t.Ru == title || t.En == title) && fieldNamesSet.Contains(t.Name.ToLower()));
                    yield return (local?.Name ?? title)?.ToLower();
                }
            }
        }

        private string GetColumnKey(IExcelColumn column)
        {
            return column.Property.Name.ToLower();
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
            var avalibleTypes = new List<Type>
            {
                typeof(string),
                typeof(int),
                typeof(int?),
                typeof(DateTime),
                typeof(DateTime?),
                typeof(decimal),
                typeof(decimal?),
                typeof(bool),
                typeof(bool?),
            };

            string lang = _userProvider.GetCurrentUser()?.Language;

            typeof(TDto)
                .GetProperties()
                .Where(x => avalibleTypes.Contains(x.PropertyType))
                .Where(x => !x.GetCustomAttributes(typeof(ExcelIgnoreAttribute), false).Any())
                .Select((p, index) => new BaseExcelColumn { Property = p, Language = lang })
                .ToList()
                .ForEach(AddColumn);
        }

        public ExcelMapper(ICommonDataService dataService, IUserProvider userProvider)
        {
            _userProvider = userProvider;
            _translations = dataService.GetDbSet<Translation>().ToList();
            InitColumns();
        }

        private readonly IUserProvider _userProvider;
        private readonly List<Translation> _translations;
        private readonly Dictionary<string, IExcelColumn> _columns = new Dictionary<string, IExcelColumn>();
        private readonly List<ValidateResult> _errors = new List<ValidateResult>();
    }
}
