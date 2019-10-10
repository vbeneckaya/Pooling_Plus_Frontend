namespace Domain.Services.Translations
{
    public static class TranslationExtensions
    {
        public static string translate(this string key, string lang)
        {
            return TranslationProvider.Translate(key, lang);
        }

        public static string translateFormat(this string key, string lang, params object[] args)
        {
            string localizedKey = translate(key, lang);
            return string.Format(localizedKey, args);
        }
    }
}
