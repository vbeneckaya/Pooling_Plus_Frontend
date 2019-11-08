namespace Domain.Shared
{
    public class LookUpDto
    {
        public string Value { get; set; }
        public string Name { get; set; }

    }

    public class ValidateResult
    {
        public ValidateResult() 
        {
            ResultType = ValidateResultType.Updated;
        }

        public ValidateResult(string error)
        {
            Error = error;
            ResultType = ValidateResultType.Error;
        }

        public ValidateResult(string error, string id)
        {
            Error = error;
            Id = id;

            ResultType = string.IsNullOrEmpty(Error) ? ValidateResultType.Updated : ValidateResultType.Error;
        }

        public virtual string Error { get; set; }

        public string Id { get; set; }

        public virtual bool IsError
        {
            get
            {
                return ResultType == ValidateResultType.Error;
            }
        }

        public ValidateResultType ResultType { get; set; }

    }

    public enum ValidateResultType
    { 
        Updated,
        Created,
        Error
    }
}