using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Shared
{
    public class ValidationResultError
    {
        public string Name { get; set; }

        public string Message {get; set; }

        public ValidationErrorType ErrorType { get; set; }
    }

    public enum ValidationErrorType
    { 
        InvalidDictionaryValue,

        InvalidValueFormat,

        ValueIsRequired,

        DuplicatedRecord
    }
}
