namespace Domain.Shared
{
    public class LookUpDto
    {
        public string Value { get; set; }
        public string Name { get; set; }
    }

    public class ValidateResult
    {
        public ValidateResult() { }

        public ValidateResult(string error)
        {
            Error = error;
        }

        public ValidateResult(string error, string id)
        {
            Error = error;
            Id = id;
        }

        public virtual string Error { get; set; }

        public string Id { get; set; }

        public virtual bool IsError
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Error);
            }
        }
    }
}