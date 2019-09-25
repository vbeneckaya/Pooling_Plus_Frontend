using Serilog;
using System.Globalization;
using System.Xml;

namespace Tasks.Helpers
{
    public static class ParseXmlExtensions
    {
        public static string ExtractSPGR(this string rawValue)
        {
            if (string.IsNullOrEmpty(rawValue))
            {
                return null;
            }
            rawValue = rawValue.Trim();
            if (rawValue.Length < 5)
            {
                Log.Warning("Длина поля SPGR должна быть равна 5, но обнаружено значение: {rawValue}.", rawValue);
                return rawValue;
            }
            else
            {
                return rawValue.Substring(rawValue.Length - 5);
            }
        }

        public static int ParseUom(this XmlNode node, string xPath, string[] keys, int[] values, int defaultValue, int? entryInd = null)
        {
            string actualKey = node.SelectSingleNode(xPath)?.InnerText?.ToUpper();
            if (!string.IsNullOrEmpty(actualKey))
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    if (keys[i].ToUpper() == actualKey)
                    {
                        return values[i];
                    }
                }
                Log.Warning("Неизвестное значение {actualKey} элемента {xPath} записи #{entryInd}.", actualKey, xPath, entryInd);
            }
            else if (entryInd.HasValue)
            {
                Log.Warning("Элемент {xPath} не найден в записи #{entryInd}.", xPath, entryInd);
            }
            else
            {
                Log.Warning("Элемент {xPath} не найден.", xPath);
            }
            return defaultValue;
        }

        public static decimal? ParseDecimal(this XmlNode node, string xPath, int? entryInd = null)
        {
            string value = node.SelectSingleNode(xPath)?.InnerText;
            if (!string.IsNullOrEmpty(value))
            {
                decimal result;
                if (decimal.TryParse(value.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out result))
                {
                    return result;
                }
                else if (entryInd.HasValue)
                {
                    Log.Warning("Некорректное значение {value} элемента {xPath} записи #{entryInd}, ожидалось число.", value, xPath, entryInd);
                }
                else
                {
                    Log.Warning("Некорректное значение {value} элемента {xPath}, ожидалось число.", value, xPath);
                }
            }
            return null;
        }

        public static int? ParseInt(this XmlNode node, string xPath, int? entryInd = null)
        {
            string value = node.SelectSingleNode(xPath)?.InnerText;
            if (!string.IsNullOrEmpty(value))
            {
                int result;
                if (int.TryParse(value.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out result))
                {
                    return result;
                }
                else if (entryInd.HasValue)
                {
                    Log.Warning("Некорректное значение {value} элемента {xPath} записи #{entryInd}, ожидалось целое число.", value, xPath, entryInd);
                }
                else
                {
                    Log.Warning("Некорректное значение {value} элемента {xPath}, ожидалось целое число.", value, xPath);
                }
            }
            return null;
        }

        public static int? ApplyUowCoeff(this decimal? value, int coeff)
        {
            return value == null ? (int?)null : (int)(value * coeff);
        }
    }
}
