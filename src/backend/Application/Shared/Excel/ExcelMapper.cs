using Application.Shared.Excel.Columns;
using DAL.Services;
using Domain.Persistables;
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

        public void FillSheet(ExcelWorksheet worksheet, IEnumerable<TDto> entries, string lang)
        {
            FillDefaultColumnOrder();
            FillColumnTitles(lang);

            foreach(var column in _columns.Values)
            {
                worksheet.Cells[1, column.ColumnIndex].Value = column.Title;
                worksheet.Cells[1, column.ColumnIndex].Style.Font.Bold = true;
            };

            int rowIndex = 1;
            foreach(var entry in entries)
            {
                ++rowIndex;
                foreach(var column in _columns.Values)
                {
                    var cell = worksheet.Cells[rowIndex, column.ColumnIndex];
                    column.FillValue(entry, cell);
                };
            }
        }

        public IEnumerable<TDto> LoadEntries(ExcelWorksheet worksheet)
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

            columnTitles = Unlocalize(columnTitles).ToList();

            FillColumnOrder(columnTitles);

            foreach(int rowIndex in rows.Skip(1))
            {
                bool isEmpty = IsEmptyRow(worksheet, rowIndex);
                if (isEmpty)
                {
                    continue;
                }

                StringBuilder errors = new StringBuilder();
                var entity = new TDto();
                foreach (var column in _columns.Values)
                {
                    try
                    {
                        var cell = worksheet.Cells[rowIndex, column.ColumnIndex];
                        column.SetValue(entity, cell);
                    }
                    catch (Exception ex)
                    {
                        errors.AppendLine($"Строка {rowIndex}: {ex.Message}.");
                    }
                };
                _errors.Add(new ValidateResult(errors.ToString()));
                yield return entity;
            }
        }

        public IEnumerable<ValidateResult> Errors => _errors;

        private void FillColumnTitles(string lang)
        {
            foreach (var column in _columns)
            {
                Translation local = _translations.FirstOrDefault(t => t.Name?.ToLower() == column.Key);
                column.Value.Title = (lang == "en" ? local?.En : local?.Ru) ?? column.Key;
            }
        }

        private void FillDefaultColumnOrder()
        {
            var propNames = typeof(TDto).GetProperties().Select(p => p.Name.ToLower()).ToList();

            foreach (var column in _columns)
            {
                int orderIndex = propNames.IndexOf(column.Key);
                if (orderIndex < 0)
                {
                    orderIndex = int.MaxValue;
                }
                column.Value.ColumnIndex = orderIndex;
            }

            int columnIndex = 0;
            foreach (var column in _columns.Values.OrderBy(c => c.ColumnIndex))
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

        private IEnumerable<string> Unlocalize(IEnumerable<string> titles)
        {
            foreach (string title in titles)
            {
                if (string.IsNullOrEmpty(title))
                {
                    yield return title;
                }
                else
                {
                    Translation local = _translations.FirstOrDefault(t => t.Ru == title || t.En == title);
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

            typeof(TDto)
                .GetProperties()
                .Where(x => avalibleTypes.Contains(x.PropertyType))
                .Select((p, index) => new BaseExcelColumn { Property = p })
                .ToList()
                .ForEach(AddColumn);
        }

        public ExcelMapper(ICommonDataService dataService)
        {
            _translations = dataService.GetDbSet<Translation>().ToList();
            InitColumns();
        }

        private readonly List<Translation> _translations;
        private readonly Dictionary<string, IExcelColumn> _columns = new Dictionary<string, IExcelColumn>();
        private readonly List<ValidateResult> _errors = new List<ValidateResult>();
    }
}
