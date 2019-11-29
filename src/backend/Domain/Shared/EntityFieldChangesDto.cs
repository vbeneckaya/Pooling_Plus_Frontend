namespace Domain.Shared
{
    public class EntityFieldChangesDto
    {
        public string FieldName { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
    }
}
