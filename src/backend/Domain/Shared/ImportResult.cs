using Domain.Services.Translations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Shared
{
    public class ImportResult
    {
        private readonly string _locale;

        public List<ValidateResult> Results { get; } = new List<ValidateResult>();

        public int ErrorsCount
        {
            get 
            {
                return this.Results.Where(i => i.IsError).Count();
            }
        }
        
        public int SuccessCount
        {
            get
            {
                return this.Results.Where(i => !i.IsError).Count();
            }
        }

        public int UpdatedCount
        {
            get
            {
                return this.Results.Where(i => i.ResultType == ValidateResultType.Updated).Count();
            }
        }

        public int CreatedCount
        {
            get
            {
                return this.Results.Where(i => i.ResultType == ValidateResultType.Created).Count();
            }
        }

        private IEnumerable<DetailedValidattionResult> DetailedResults
        {
            get 
            {
                return this.Results
                        .Where(i => i.GetType() == typeof(DetailedValidattionResult))
                        .Cast<DetailedValidattionResult>();
            }
        }

        public int RequiredErrorsCount
        {
            get
            {
                return this.DetailedResults
                    .Where(i => i.Errors.Any(e => e.ResultType == ValidationErrorType.ValueIsRequired))
                    .Count();
            }
        }

        public int InvalidDictionaryValueErrorsCount
        {
            get
            {
                return this.DetailedResults
                    .Where(i => i.Errors.Any(e => e.ResultType == ValidationErrorType.InvalidDictionaryValue))
                    .Count();
            }
        }

        public int DuplicatedRecordErrorsCount
        {
            get
            {
                return this.DetailedResults
                    .Where(i => i.Errors.Any(e => e.ResultType == ValidationErrorType.DuplicatedRecord))
                    .Count();
            }
        }

        public int InvalidValueFormatErrorsCount
        {
            get
            {
                return this.DetailedResults
                    .Where(i => i.Errors.Any(e => e.ResultType == ValidationErrorType.InvalidValueFormat))
                    .Count();
            }
        }

        public string RequiredErrorsMessage
        {
            get
            {
                return "requiredErrorsMessage".Translate(_locale, RequiredErrorsCount);
            }
        }

        public string InvalidDictionaryValueErrorsMessage
        {
            get
            {
                return "invalidDictionaryValueErrorsMessage".Translate(_locale, InvalidDictionaryValueErrorsCount);
            }
        }

        public string DuplicatedRecordErrorsMEssage
        {
            get
            {
                return "invalidDictionaryValueErrorsMessage".Translate(_locale, RequiredErrorsCount);
            }
        }



        public bool IsError 
        {
            get 
            {
                return Results.Any(i => i.IsError);
            }
        }
    }
}
