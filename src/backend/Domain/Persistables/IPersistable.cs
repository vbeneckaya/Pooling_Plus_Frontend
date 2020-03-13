using System;

namespace Domain.Persistables
{
    public interface IPersistableWithName : IPersistable
    {
        string Name { get; set; }
    }

    public interface IPersistableWithCreator : IPersistable
    {
        Guid? UserCreatorId { get; set; }
    }

    public interface IPersistable
    {
        Guid Id { get; set; }
    }
}