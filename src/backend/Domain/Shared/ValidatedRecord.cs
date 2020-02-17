using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Shared
{
    public class ValidatedRecord<TDto>
    {
        public TDto Data { get; set; }

        public DetailedValidationResult Result { get; set; } = new DetailedValidationResult();

        public ValidatedRecord(TDto data)
        {
            this.Data = data;
        }

        public ValidatedRecord(TDto data, DetailedValidationResult result)
        {
            this.Data = data;
            this.Result = result;
        }
    }
}
