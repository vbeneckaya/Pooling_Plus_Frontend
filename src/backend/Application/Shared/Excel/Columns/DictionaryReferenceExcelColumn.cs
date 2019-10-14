using System;
using System.Reflection;
using OfficeOpenXml;

namespace Application.Shared.Excel.Columns
{
    public class DictionaryReferenceExcelColumn : IExcelColumn
    {
        public PropertyInfo Property { get; set; }
        public string Title { get; set; }
        public int ColumnIndex { get; set; }

        public void FillValue(object entity, ExcelRange cell)
        {
            string strId = Property.GetValue(entity)?.ToString();
            if (!string.IsNullOrEmpty(strId) && Guid.TryParse(strId, out Guid id))
            {
                string name = _nameLookupMethod(id);
                cell.Value = name;
            }
        }

        public void SetValue(object entity, ExcelRange cell)
        {
            string refName = cell.GetValue<string>();
            string refId = string.IsNullOrEmpty(refName) ? null : _guidLookupMethod(refName)?.ToString();
            Property.SetValue(entity, refId);
        }

        public DictionaryReferenceExcelColumn(Func<string, Guid?> guidLookupMethod, Func<Guid, string> nameLookupMethod)
        {
            _guidLookupMethod = guidLookupMethod;
            _nameLookupMethod = nameLookupMethod;
        }

        private readonly Func<string, Guid?> _guidLookupMethod;
        private readonly Func<Guid, string> _nameLookupMethod;
    }
}
