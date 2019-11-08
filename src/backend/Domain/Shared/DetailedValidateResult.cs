using System.Collections.Generic;
using System.Linq;

namespace Domain.Shared
{
    public class DetailedValidattionResult: ValidateResult
    {
        public DetailedValidattionResult() { }

        public List<ValidationResultError> Errors { get; set; } = new List<ValidationResultError>();

        public override string Error {
            get 
            {
                return this.Errors.SelectMany(i => i.Message).Join(", ");
            }
            set {  }
        }
        
        public override bool IsError
        {
            get
            {
                return Errors.Any();
            }
        }

        public void AddError(string name, string message, ValidationErrorType errorType)
        {
            this.Errors.Add(new ValidationResultError
            { 
                Name = name,
                Message = message,
                ErrorType = errorType
            });
        }
    }
}