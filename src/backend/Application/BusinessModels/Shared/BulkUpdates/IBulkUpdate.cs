using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Domain.Services.UserProvider;

namespace Application.BusinessModels.Shared.BulkUpdates
{
    public interface IBulkUpdate<T>
    {
        string FieldName { get; }
        FieldType FieldType { get; }

        AppActionResult Update(CurrentUserDto user, T target, string fieldName, string value);
        bool IsAvailable(Role role, T target);
    }
}
