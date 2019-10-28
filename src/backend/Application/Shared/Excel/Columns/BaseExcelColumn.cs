﻿using Domain.Services.Translations;
using OfficeOpenXml;
using System;
using System.Reflection;

namespace Application.Shared.Excel.Columns
{
    public class BaseExcelColumn : IExcelColumn
    {
        public PropertyInfo Property { get; set; }
        public string Title { get; set; }
        public int ColumnIndex { get; set; }
        public string Language { get; set; }

        public void FillValue(object entity, ExcelRange cell)
        {
            if (Property.PropertyType == typeof(DateTime?))
            {
                var numberformatFormat = "dd.MM.yyyy HH:mm:ss";

                cell.Style.Numberformat.Format = numberformatFormat;
                var dateTime = (DateTime?)Property.GetValue(entity);
                if (dateTime.HasValue)
                    cell.Value = dateTime.Value.ToString(numberformatFormat);
            }
            else if (Property.PropertyType == typeof(bool) || Property.PropertyType == typeof(bool?))
            {
                bool? value = (bool?)Property.GetValue(entity);
                if (value.HasValue)
                {
                    cell.Value = (value == true ? "Yes" : "No").translate(Language);
                }
            }
            else
            {
                cell.Value = Property.GetValue(entity);
            }
        }

        public void SetValue(object entity, ExcelRange cell)
        {
            if (cell.Value == null)
            {
                Property.SetValue(entity, null);
            }
            else if (Property.PropertyType == typeof(int))
            {
                Property.SetValue(entity, cell.GetValue<int>());
            }
            else if (Property.PropertyType == typeof(int?))
            {
                Property.SetValue(entity, cell.GetValue<int?>());
            }
            else if (Property.PropertyType == typeof(decimal))
            {
                Property.SetValue(entity, cell.GetValue<decimal>());
            }
            else if (Property.PropertyType == typeof(decimal?))
            {
                Property.SetValue(entity, cell.GetValue<decimal?>());
            }
            else if (Property.PropertyType == typeof(DateTime))
            {
                Property.SetValue(entity, cell.GetValue<DateTime>());
            }
            else if (Property.PropertyType == typeof(DateTime?))
            {
                Property.SetValue(entity, cell.GetValue<DateTime?>());
            }
            else if (Property.PropertyType == typeof(bool) || Property.PropertyType == typeof(bool?))
            {
                bool? value = null;
                string cellValue = cell.GetValue<string>()?.ToLower();
                if(cellValue == "да" || cellValue == "д" || cellValue == "yes" || cellValue == "y")
                {
                    value = true;
                }
                else if (cellValue == "нет" || cellValue == "н" || cellValue == "no" || cellValue == "n")
                {
                    value = false;
                }

                if (Property.PropertyType == typeof(bool))
                {
                    Property.SetValue(entity, value ?? false);
                }
                else
                {
                    Property.SetValue(entity, value);
                }
            }
            else
            {
                Property.SetValue(entity, cell.Value?.ToString());
            }
        }
    }
}
