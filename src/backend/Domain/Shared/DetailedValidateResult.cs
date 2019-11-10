using System.Collections.Generic;
using System.Linq;

namespace Domain.Shared
{
    public class DetailedValidattionResult: ValidateResult
    {
        public DetailedValidattionResult() { }

        public List<ValidationResultItem> Errors { get; set; } = new List<ValidationResultItem>();

        public override string Error {
            get 
            {
                return string.Join(", ", this.Errors.Select(i => i.Message));
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
            this.ResultType = ValidateResultType.Error;
            this.Errors.Add(new ValidationResultItem
            { 
                Name = name,
                Message = message,
                ResultType = errorType
            });
        }
    }
}