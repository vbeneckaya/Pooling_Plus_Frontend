using System;

namespace Domain.Services.History
{
    public interface IHistoryService
    {
        HistoryDto Get(Guid entityId, string lang);
        void Save(Guid entityId, string messageKey, params object[] messageArgs);
    }
}
