using DAL;
using DAL.Queries;
using Domain.Extensions;
using Domain.Persistables;
using Domain.Services.History;
using Domain.Services.UserIdProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.History
{
    public class HistoryService : IHistoryService
    {
        public HistoryDto Get(Guid entityId, string lang)
        {
            var translations = _db.Translations.ToList();
            var dict = new Dictionary<string, Translation>();
            foreach (Translation local in translations)
            {
                dict[local.Name] = local;
            }

            var entries = _db.HistoryEntries.Where(e => e.PersistableId == entityId)
                                            .OrderByDescending(e => e.CreatedAt)
                                            .ToList();

            var dtos = entries.Select(e => ConvertEntityToDto(e, lang, dict)).ToList();

            return new HistoryDto
            {
                Entries = dtos
            };
        }

        public void Save(Guid entityId, string messageKey, params object[] messageArgs)
        {
            User user = _userProvider.GetCurrentUser();
            string userName = GetUserName(user);

            string[] valueArgs = messageArgs.Select(GetDisplayValue).ToArray();
            string strArgs = JsonConvert.SerializeObject(valueArgs);

            HistoryEntry entry = new HistoryEntry
            {
                PersistableId = entityId,
                UserId = user?.Id,
                UserName = userName,
                CreatedAt = DateTime.UtcNow,
                MessageKey = messageKey,
                MessageArgs = strArgs
            };
            _db.HistoryEntries.Add(entry);
        }

        private HistoryEntryDto ConvertEntityToDto(HistoryEntry entity, string lang, Dictionary<string, Translation> dict)
        {
            string[] args = JsonConvert.DeserializeObject<string[]>(entity.MessageArgs ?? string.Empty);
            string[] localArgs = args.Select(x => Localize(x, lang, dict)).ToArray();

            string localKey = Localize(entity.MessageKey, lang, dict) ?? string.Empty;
            string message = string.Format(localKey, localArgs);

            return new HistoryEntryDto
            {
                UserName = entity.UserName,
                CreatedAt = entity.CreatedAt,
                Message = message
            };
        }

        private string Localize(string key, string lang, Dictionary<string, Translation> dict)
        {
            if (string.IsNullOrEmpty(key))
            {
                return key;
            }
            if (dict.TryGetValue(key, out Translation local))
            {
                return lang == "en" ? local.En : local.Ru;
            }
            else
            {
                return key;
            }
        }

        private string GetDisplayValue(object value)
        {
            if (value == null || (value is string && (value as string).Length == 0))
            {
                return "emptyValue";
            }
            else if (value is DateTime)
            {
                return ((DateTime)value).ToString("dd.MM.yyyy HH:mm");
            }
            else if (value is Enum)
            {
                return Enum.GetName(value.GetType(), value).ToLowerfirstLetter();
            }
            else
            {
                return value?.ToString();
            }
        }

        private string GetUserName(User user)
        {
            if (user == null)
            {
                return "System";
            }
            else
            {
                Role role = _db.Roles.GetById(user.RoleId);
                if (role == null)
                {
                    return user.Name;
                }
                else
                {
                    return $"{user.Name} ({role.Name})";
                }
            }
        }

        public HistoryService(AppDbContext db, IUserIdProvider userProvider)
        {
            _db = db;
            _userProvider = userProvider;
        }

        private readonly AppDbContext _db;
        private readonly IUserIdProvider _userProvider;
    }
}
