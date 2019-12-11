using Domain.Enums;
using Domain.Extensions;
using Domain.Services;
using Domain.Services.FieldProperties;
using Domain.Services.Translations;
using Domain.Services.UserProvider;
using Domain.Shared;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Application.Shared
{
    /// <summary>
    /// Validation service
    /// </summary>
    public class ValidationService : IValidationService
    {
        private readonly IFieldDispatcherService _fieldDispatcher;

        private readonly IUserProvider _userProvider;

        private readonly IConfiguration _configuration;

        /// <summary>
        /// Create ValidationService instance
        /// </summary>
        /// <param name="fieldDispatcher"></param>
        public ValidationService(IFieldDispatcherService fieldDispatcher, IUserProvider userProvider, IConfiguration configuration)
        {
            _fieldDispatcher = fieldDispatcher;
            _userProvider = userProvider;
            _configuration = configuration;
        }

        /// <summary>
        /// Validate Dto
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="dto"></param>
        /// <returns></returns>
        public DetailedValidationResult Validate<TDto>(TDto dto)
        {
            var prefix = typeof(TDto).Name.Replace("Dto", "");

            var fields = this._fieldDispatcher.GetDtoFields<TDto>();

            var lang = _userProvider.GetCurrentUser()?.Language;

            var validationResult = new DetailedValidationResult();

            foreach (var field in fields)
            {
                var property = typeof(TDto).GetProperty(field.Name);
                var value = property.GetValue(dto)?.ToString();
                var propertyName = property.Name.ToLowerFirstLetter();

                // Validate format

                if (!ValidatePropertyFormat(field, value))
                {
                    validationResult.AddError(new ValidationResultItem
                    {
                        Name = propertyName,
                        Message = $"{prefix}.{property.Name}.{ValidationErrorType.InvalidValueFormat}".Translate(lang),
                        ResultType = ValidationErrorType.InvalidValueFormat
                    });
                }

                // Validate IsRequred

                if (!this.ValidateIsRequired(field, value))
                {
                    validationResult.AddError(new ValidationResultItem
                    {
                        Name = propertyName,
                        Message = $"{prefix}.{property.Name}.{ValidationErrorType.ValueIsRequired}".Translate(lang),
                        ResultType = ValidationErrorType.ValueIsRequired
                    });
                }

                if (field.FieldType == FieldType.Password)
                {
                    var result = ValidatePassword(field, value, prefix, lang);

                    if (result != null)
                    {
                        validationResult.AddError(result);
                    }
                }

            }

            return validationResult;
        }

        /// <summary>
        /// Validate Password
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="prefix"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        private ValidationResultItem ValidatePassword(FieldInfo field, string value, string prefix, string lang)
        {
            if (string.IsNullOrEmpty(value)) return null;


            List<string> errorMessages = new List<string>();

            var passwordConfig = _configuration.GetSection("PasswordRules");

            var passwordMinLength = passwordConfig["MinLength"].ToInt();

            if (passwordMinLength.HasValue && value.Length < passwordMinLength)
            {
                errorMessages.Add("PasswordValidation.MinLength");
            }

            var isMatch = Regex.IsMatch(value, @"^[A-Za-z\d@$!%*?&]&");

            if (!isMatch)
            {
                errorMessages.Add("PasswordValidation.ValidCharacters");
            }

            var message = string.Join(", ", errorMessages.Select(i => i.Translate(lang)));

            return new ValidationResultItem
            {
                Name = field.Name.ToLowerFirstLetter(),
                Message = message,
                ResultType = ValidationErrorType.InvalidPassword
            };
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
