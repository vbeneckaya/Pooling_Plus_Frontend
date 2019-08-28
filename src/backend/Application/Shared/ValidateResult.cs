namespace Application.Shared
{
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

        public string Error { get; set; }
        public string Id { get; set; }
        public bool IsError
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Error);
            }
        }
    }
}