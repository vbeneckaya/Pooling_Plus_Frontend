namespace Domain.Services
{
    public interface IDto
    {
        string Id { get; set; }

        bool IsEditable { get; set; }
    }
}