﻿namespace Domain.Services.Translations
{
    public static class TranslationExtensions
    {
        public static string Translate(this string key, string lang, params object[] args)
        {
            string localizedKey = TranslationProvider.Translate(key, lang);
            return string.Format(localizedKey, args);
        }
    }
}
