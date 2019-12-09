using Domain.Shared;
using OfficeOpenXml;
using System;
using System.Reflection;
using FieldInfo = Domain.Services.FieldProperties.FieldInfo;

namespace Application.Shared.Excel.Columns
{
    public class DictionaryReferenceExcelColumn : IExcelColumn
    {
        public PropertyInfo Property { get; set; }
        public FieldInfo Field { get; set; }
        public string Title { get; set; }
        public int ColumnIndex { get; set; }

        public void FillValue(object entity, ExcelRange cell)
        {
            string name = (Property.GetValue(entity) as LookUpDto)?.Name;
            cell.Value = name;
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

            Property.SetValue(entity, new LookUpDto(refId, refName));

            return null;
        }

        public DictionaryReferenceExcelColumn(Func<string, Guid?> guidLookupMethod)
        {
            _guidLookupMethod = guidLookupMethod;
        }

        private readonly Func<string, Guid?> _guidLookupMethod;
    }
}
