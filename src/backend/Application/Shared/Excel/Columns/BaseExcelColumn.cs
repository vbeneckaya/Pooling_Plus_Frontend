using Domain.Services.Translations;
using Domain.Shared;
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
                    cell.Value = (value == true ? "Yes" : "No").Translate(Language);
                }
            }
            else
            {
                cell.Value = Property.GetValue(entity);
            }
        }

        public ValidationResultItem SetValue(object entity, ExcelRange cell)
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
                if (cellValue == "да" || cellValue == "д" || cellValue == "yes" || cellValue == "y")
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
            // Readng OLE Automation Date Format https://stackoverflow.com/questions/13176832/reading-a-date-from-xlsx-using-open-xml-sdk
            // Open Xml Date Format Codes
            else
            if ((cell.Style.Numberformat.NumFmtID >= 14 && cell.Style.Numberformat.NumFmtID <= 22)
                    || (cell.Style.Numberformat.NumFmtID >= 165u && cell.Style.Numberformat.NumFmtID <= 180u))
            {
                if (cell.Value is DateTime)
                {
                    var date = (DateTime)cell.Value;
                    Property.SetValue(entity, date.ToString("dd.MM.yyyy"));
                }
                else if (cell.Value is double)
                {
                    var dateNumber = (double)cell.Value;
                    try
                    {
                        var date = DateTime.FromOADate(dateNumber);
                        Property.SetValue(entity, date.ToString("dd.MM.yyyy"));
                    }
                    catch (Exception ex)
                    {
                        return new ValidationResultItem
                        {
                            Message = "invalidValueFormat",
                            ResultType = ValidationErrorType.InvalidValueFormat
                        };
                    }
                }
            }
            else
            {
                Property.SetValue(entity, cell.Value?.ToString());
            }

            return null;
        }
    }
}
