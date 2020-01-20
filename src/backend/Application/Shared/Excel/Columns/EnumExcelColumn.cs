﻿using Domain.Services.Translations;
using Domain.Shared;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FieldInfo = Domain.Services.FieldProperties.FieldInfo;

namespace Application.Shared.Excel.Columns
{
    public class EnumExcelColumn<TEnum> : IExcelColumn where TEnum : Enum
    {
        private readonly string _lang;

        public EnumExcelColumn(string lang)
        {
            _lang = lang;
        }

        public PropertyInfo Property { get; set; }
        public FieldInfo Field { get; set; }
        public string Title { get; set; }
        public int ColumnIndex { get; set; }

        public void FillValue(object entity, ExcelRange cell)
        {
            string value = (Property.GetValue(entity) as LookUpDto )?.Value;
            value = (value ?? string.Empty).Translate(_lang);
            cell.Value = value;
        }

        public ValidationResultItem SetValue(object entity, ExcelRange cell)
        {
            string cellValue = cell.GetValue<string>();
            if (string.IsNullOrEmpty(cellValue))
            {
                Property.SetValue(entity, null);
            }
            else
            {
                List<string> valueNames = new List<string>();
                foreach (TEnum value in Enum.GetValues(typeof(TEnum)))
                {
                    valueNames.Add(value.ToString());
                }

                var keys = TranslationProvider.GetKeysByTranslation(cellValue);
                keys = keys.Select(x => x.ToLower());
                string validCellValue = keys.FirstOrDefault(x => valueNames.Any(y => string.Compare(x, y, true) == 0));
                if (string.IsNullOrEmpty(validCellValue))
                {
                    string lowerCellValue = cellValue.ToLower();
                    validCellValue = valueNames.FirstOrDefault(n => n.ToLower() == lowerCellValue);
                }

                Property.SetValue(entity, new LookUpDto(validCellValue));
            }

            return null;
        }
    }
}
