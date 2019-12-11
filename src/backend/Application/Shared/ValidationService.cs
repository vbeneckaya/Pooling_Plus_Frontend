using Domain.Enums;
using Domain.Extensions;
using Domain.Services;
using Domain.Services.FieldProperties;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;

namespace Application.Shared
{
    /// <summary>
    /// Validation service
    /// </summary>
    public class ValidationService : IValidationService
    {
        private readonly IFieldDispatcherService _fieldDispatcher;

        private readonly IUserProvider _userProvider;

        /// <summary>
        /// Create ValidationService instance
        /// </summary>
        /// <param name="fieldDispatcher"></param>
        public ValidationService(IFieldDispatcherService fieldDispatcher, IUserProvider userProvider)
        {
            _fieldDispatcher = fieldDispatcher;
            _userProvider = userProvider;
        }

        /// <summary>
        /// Validate Dto
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="dto"></param>
        /// <returns></returns>
        public DetailedValidationResult Validate<TDto>(TDto dto)
        {
            var fields = _fieldDispatcher.GetDtoFields<TDto>();

            var lang = _userProvider.GetCurrentUser()?.Language;

            var validationResult = new DetailedValidationResult();

            foreach (var field in fields)
            {
                var property = typeof(TDto).GetProperty(field.Name);
                var value = property.GetValue(dto)?.ToString();
                var propertyName = property.Name.ToLowerFirstLetter();
                var propertyDisplayName = propertyName.Translate(lang);

                // Validate format

                if (!ValidatePropertyFormat(field, value))
                {
                    validationResult.AddError(new ValidationResultItem
                    {
                        Name = propertyName,
                        Message = "InvalidValueFormat".Translate(lang, propertyDisplayName),
                        ResultType = ValidationErrorType.InvalidValueFormat
                    });
                }

                // Validate IsRequred

                if (!ValidateIsRequired(field, value))
                {
                    validationResult.AddError(new ValidationResultItem
                    {
                        Name = propertyName,
                        Message = "ValueIsRequired".Translate(lang, propertyDisplayName),
                        ResultType = ValidationErrorType.ValueIsRequired
                    });
                }
            }

            return validationResult;
        }

        /// <summary>
        /// Validate mandatory field
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool ValidateIsRequired(FieldInfo field, string value)
        {
            return !field.IsRequired || !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Validate field format
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool ValidatePropertyFormat(FieldInfo field, string value)
        {
            switch (field.FieldType)
            {
                case FieldType.Date: return ValidateDate(field, value);
                case FieldType.Time: return ValidateTime(field, value);

                default: return true;
            }
        }

        /// <summary>
        /// Validate date
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool ValidateDate(FieldInfo field, string value)
        {
            var dateValue = value.ToDate();
            return string.IsNullOrEmpty(value) || dateValue.HasValue;
        }

        /// <summary>
        /// Validate time
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool ValidateTime(FieldInfo field, string value)
        {
            var timeValue = value.ToTimeSpan();
            return string.IsNullOrEmpty(value) || timeValue.HasValue;
        }
    }
}
