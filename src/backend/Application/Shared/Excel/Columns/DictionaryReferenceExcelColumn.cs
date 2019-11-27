using System;
using System.Reflection;
using Domain.Services.Translations;
using Domain.Shared;
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

        public ValidationResultItem SetValue(object entity, ExcelRange cell)
        {
            string refName = cell.GetValue<string>();
            string refId = string.IsNullOrEmpty(refName) ? null : _guidLookupMethod(refName)?.ToString();

            if (!string.IsNullOrEmpty(refName) && string.IsNullOrEmpty(refId))
            {
                return new ValidationResultItem
                {
                    Name = refName,
                    Message = "invalidDictionaryValue",
                    ResultType = ValidationErrorType.InvalidDictionaryValue
                };
            }

            Property.SetValue(entity, refId);

            return null;
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
