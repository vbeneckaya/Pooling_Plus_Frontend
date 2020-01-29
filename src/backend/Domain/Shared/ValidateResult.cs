namespace Domain.Shared
{
    public class LookUpDto
    {
        public const string EmptyValue = "###EMPTY###";

        public string Value { get; set; }
        public string Name { get; set; }
        public bool IsFilterOnly { get; set; }

        public LookUpDto() { }

        public LookUpDto(string value) : this(value, value) { }

        public LookUpDto(string value, string name)
        {
            Value = value;
            Name = name;
        }
    }
    
    public class LookUpAddressDto : LookUpDto
    {
        public const string EmptyValue = "###EMPTY###";

        public string Address { get; set; }
    }
    
    public class ValidateResult
    {
        public ValidateResult() 
        {
            ResultType = ValidateResultType.Updated;
        }

        public ValidateResult(string error)
        {
            ResultType = string.IsNullOrEmpty(Error) ? ValidateResultType.Updated : ValidateResultType.Error;
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