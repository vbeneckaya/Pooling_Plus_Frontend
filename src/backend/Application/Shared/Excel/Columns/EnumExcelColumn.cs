using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OfficeOpenXml;

namespace Application.Shared.Excel.Columns
{
    public class EnumExcelColumn<TEnum> : IExcelColumn where TEnum : Enum
    {
        public PropertyInfo Property { get; set; }
        public string Title { get; set; }
        public int ColumnIndex { get; set; }

        public void FillValue(object entity, ExcelRange cell)
        {
            cell.Value = Property.GetValue(entity);
        }

        public void SetValue(object entity, ExcelRange cell)
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

                string lowerCellValue = cellValue.ToLower();
                string validCellValue = valueNames.FirstOrDefault(n => n.ToLower() == lowerCellValue);
                Property.SetValue(entity, validCellValue);
            }
        }
    }
}
