using System;
using System.ComponentModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Domain.Enums;

namespace Domain.Extensions
{
    public static class Extensions
    {
        public static string ToUpperFirstLetter(this string input)
        {
            return Char.ToUpperInvariant(input[0]) + input.Substring(1);
        }
        
        public static string ToLowerFirstLetter(this string input)
        {
            return Char.ToLowerInvariant(input[0]) + input.Substring(1);
        }

        public static string Pluralize(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            else if (input.EndsWith('s'))
            {
                return input + "es";
            }
            else if (input.EndsWith('y'))
            {
                return input.Substring(0, input.Length - 1) + "es";
            }
            else
            {
                return input + "s";
            }
        }

        public static string GetHash(this string text)
        {
            return Convert.ToBase64String(new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(text)));
        }

        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            string description = null;

            if (e is Enum)
            {
                Type type = e.GetType();
                Array values = Enum.GetValues(type);

                foreach (int val in values)
                {
                    if (val == e.ToInt32(CultureInfo.InvariantCulture))
                    {
                        var memInfo = type.GetMember(type.GetEnumName(val));
                        var descriptionAttributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                        if (descriptionAttributes.Length > 0)
                        {
                            // we're only getting the first description we find
                            // others will be ignored
                            description = ((DescriptionAttribute)descriptionAttributes[0]).Description;
                        }

                        break;
                    }
                }
            }

            return description;
        }
        
        public static AppColor GetColor<T>(this T e)
        {
            AppColor description = AppColor.Black;

            if (e is Enum)
            {
                Type type = e.GetType();
                Array values = Enum.GetValues(type);

                foreach (int val in values)
                {
                    if (val == ((IConvertible) e).ToInt32(CultureInfo.InvariantCulture))
                    {
                        var memInfo = type.GetMember(type.GetEnumName(val));
                        var attributes = memInfo[0].GetCustomAttributes(typeof(StateColorAttribute), false);
                        if (attributes.Length > 0)
                        {
                            // we're only getting the first description we find
                            // others will be ignored
                            description = ((StateColorAttribute)attributes[0]).Color;
                        }

                        break;
                    }
                }
            }

            return description;
        }

        public static object GetEnum<T>(this string e) where T : IConvertible
        {
            string description = null;

            Type type = typeof(T);
            Array values = Enum.GetValues(type);

            foreach (int val in values)
            {
                var memInfo = type.GetMember(type.GetEnumName(val));
                var descriptionAttributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (descriptionAttributes.Length > 0)
                {
                    description = ((DescriptionAttribute)descriptionAttributes[0]).Description;

                    if (e.Equals(description))
                    {
                        return Enum.Parse(type, type.GetEnumName(val));
                    }
                }
            }

            return null;
        }

        public static bool TryParseDate(this string dateString, out DateTime result) =>
            DateTime.TryParseExact(dateString,
                "dd.MM.yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out result);

        public static bool TryParseDateTime(this string dateString, out DateTime result) =>
            DateTime.TryParseExact(dateString,
                "dd.MM.yyyy HH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out result);

        public static DateTime ParseDate(this string dateString) =>
            DateTime.ParseExact(dateString,
                "dd.MM.yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None);

        public static bool TryParseDateAsUtc(this string dateString, out DateTime result)
        {
            if (dateString.TryParseDate(out result))
            {
                result = DateTime.SpecifyKind(result, DateTimeKind.Utc);
                return true;
            }
            return false;
        }

        public static bool TryParseDateTimeAsUtc(this string dateString, out DateTime result)
        {
            if (dateString.TryParseDateTime(out result))
            {
                result = DateTime.SpecifyKind(result, DateTimeKind.Utc);
                return true;
            }
            return false;
        }

        public static string ToDateString(this DateTime date) =>
            date.ToString("dd.MM.yyyy");
        public static string ToDateTimeString(this DateTime dateTime) =>
            dateTime.ToString("dd.MM.yyyy HH:mm");

        public static DateTime ToUtc(this DateTime date, TimeZoneInfo tz) =>
            TimeZoneInfo.ConvertTimeToUtc(date, tz);
        public static DateTime FromUtc(this DateTime date, TimeZoneInfo tz) =>
            TimeZoneInfo.ConvertTimeFromUtc(date, tz);

        public static Guid? ToGuid(this string guidString)
        {
            return Guid.TryParse(guidString, out Guid guid) ? guid : (Guid?)null;
        }

        public static TEnum? Parse<TEnum>(this string value) where TEnum: struct, Enum
        {
            if (string.IsNullOrEmpty(value)) return null;

            return (TEnum?)Enum.Parse(typeof(TEnum), value, true);
        }

        public static bool? ToBool(this string boolString)
        {
            return bool.TryParse(boolString, out bool boolValue) ? boolValue : (bool?)null;
        }
    }
}
