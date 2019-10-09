﻿using OfficeOpenXml;
using System.Reflection;

namespace Application.Shared.Excel.Columns
{
    public interface IExcelColumn
    {
        PropertyInfo Property { get; set; }
        string Title { get; set; }
        int ColumnIndex { get; set; }

        void FillValue(object entity, ExcelRange cell);
        void SetValue(object entity, ExcelRange cell);
    }
}
