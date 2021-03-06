﻿using Domain.Enums;
using Domain.Services.Translations;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Domain.Persistables;

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

        public static IEnumerable<T> GetOrderedEnum<T>()
        {
            Type type = typeof(T);
            var values = Enum.GetValues(type);

            var valuesDict = new Dictionary<T, int>();
            foreach (var rawValue in values)
            {
                int orderNumber = -1;
                var memInfo = type.GetMember(type.GetEnumName(rawValue));
                if (memInfo?.Length > 0)
                {
                    var orderNumberAttributes = memInfo[0].GetCustomAttributes(typeof(OrderNumberAttribute), false);
                    if (orderNumberAttributes?.Length > 0)
                    {
                        orderNumber = ((OrderNumberAttribute)orderNumberAttributes[0]).Value;
                    }
                }
                valuesDict[(T)rawValue] = orderNumber;
            }

            return valuesDict.OrderBy(x => x.Value)
                             .Select(x => x.Key);
        }

        public static LookUpDto GetEnumLookup<T>(this T value, string language)
        {
            if (value == null)
            {
                return null;
            }
            else
            {
                string strValue = value.ToString().ToLowerFirstLetter();
                string name = strValue.Translate(language);
                return new LookUpDto
                {
                    Value = strValue,
                    Name = name
                };
            }
        }

        public static bool TryParseDate(this string dateString, out DateTime result)
        {
            if (string.IsNullOrEmpty(dateString))
            {
                result = default;
                return false;
            }
            return DateTime.TryParseExact(dateString,
                                          "dd.MM.yyyy",
                                          CultureInfo.InvariantCulture,
                                          DateTimeStyles.None,
                                          out result);
        }

        public static bool TryParseDateTime(this string dateString, out DateTime result)
        {
            if (string.IsNullOrEmpty(dateString))
            {
                result = default;
                return false;
            }
            return DateTime.TryParseExact(dateString,
                                          "dd.MM.yyyy HH:mm:ss",
                                          CultureInfo.InvariantCulture,
                                          DateTimeStyles.None,
                                          out result);
        }

        public static DateTime ParseDate(this string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
            {
                return default;
            }
            return DateTime.ParseExact(dateString,
                                       "dd.MM.yyyy",
                                       CultureInfo.InvariantCulture,
                                       DateTimeStyles.None);
        }

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
            if (string.IsNullOrEmpty(guidString)) return null;

            return Guid.TryParse(guidString, out Guid guid) ? guid : (Guid?)null;
        }

        public static DateTime? ToDateTime(this string dateString)
        {
            if (string.IsNullOrEmpty(dateString)) return null;

            return dateString.TryParseDateTime(out DateTime date) ? date : (DateTime?)null;
        }

        public static TimeSpan? ToTimeSpan(this string timeString)
        {
            if (string.IsNullOrEmpty(timeString)) return null;

            bool result = TimeSpan.TryParse(timeString, CultureInfo.InvariantCulture, out TimeSpan time);

            return result ? time : (TimeSpan?)null;
        }

        public static DateTime? ToDate(this string dateString)
        {
            if (string.IsNullOrEmpty(dateString)) return null;

            return dateString.TryParseDate(out DateTime date) ? date : (DateTime?)null;
        }

        public static decimal? ToDecimal(this string decimalString)
        {
            if (string.IsNullOrEmpty(decimalString)) return null;

            return decimal.TryParse(decimalString.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal number) ? number : (decimal?)null;
        }

        public static TEnum? Parse<TEnum>(this string value) where TEnum: struct, Enum
        {
            if (string.IsNullOrEmpty(value)) return null;

            return (TEnum?)Enum.Parse(typeof(TEnum), value, true);
        }

        public static int? ToInt(this string intString)
        {
            return int.TryParse(intString, out int intValue) ? intValue : (int?)null;
        }

        public static bool? ToBool(this string boolString)
        {
            return bool.TryParse(boolString, out bool boolValue) ? boolValue : (bool?)null;
        }
    }
}
