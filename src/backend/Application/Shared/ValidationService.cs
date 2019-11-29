using Domain.Enums;
using Domain.Extensions;
using Domain.Services;
using Domain.Shared;
using System.Linq;
using System.Reflection;

namespace Application.Shared
{
    public class ValidationService : IValidationService
    {
        public ValidatedRecord<TDto> Validate<TDto>(TDto dto)
        {
            var properties = typeof(TDto)
                .GetProperties()
                .Where(i => i.GetCustomAttributes(false).Any(j => j.GetType() == typeof(FieldTypeAttribute)));

            var validationResult = new ValidatedRecord<TDto>(dto);

            foreach (var property in properties)
            {
                // Validate format
                var formatResult = ValidatePropertyType(property, dto);

                if (formatResult != null)
                {
                    validationResult.Result.AddError(formatResult);
                }

                // Validate IsRequred
            }

            return validationResult;
        }

        private ValidationResultItem ValidatePropertyType<TDto>(PropertyInfo property, TDto dto)
        {
            var fieldType = (FieldTypeAttribute)property.GetCustomAttributes(false)
                .FirstOrDefault(j => j.GetType() == typeof(FieldTypeAttribute));

            if (fieldType == null) return null;

            switch (fieldType.Type)
            {
                case FieldType.Date: return ValidateDate(property, dto);
                default: return null;
            }
        }

        private ValidationResultItem ValidateDate<TDto>(PropertyInfo property, TDto dto)
        {
            var strValue = property.GetValue(dto)?.ToString();

            var dateValue = strValue.ToDate();

            if (!string.IsNullOrEmpty(strValue) && !dateValue.HasValue)
            {
                return new ValidationResultItem
                {
                    Name = property.Name/*.ToLowerfirstLetter()*/,
                    ResultType = ValidationErrorType.InvalidValueFormat
                };
            }

            return null;
        }

    }
}
