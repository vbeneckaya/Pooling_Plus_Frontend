namespace Domain.Shared
{
    public class PagingForm
    {
        public int Page { get; set; }
        public bool LoadPrevious { get; set; } = false;
    }
}