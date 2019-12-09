using Domain.Shared;
using OfficeOpenXml;
using System.Reflection;
using FieldInfo = Domain.Services.FieldProperties.FieldInfo;

namespace Application.Shared.Excel.Columns
{
    public interface IExcelColumn
    {
        PropertyInfo Property { get; set; }
        FieldInfo Field { get; set; }
        string Title { get; set; }
        int ColumnIndex { get; set; }

        void FillValue(object entity, ExcelRange cell);
        ValidationResultItem SetValue(object entity, ExcelRange cell);
    }
}
