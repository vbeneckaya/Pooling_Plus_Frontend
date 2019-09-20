using Domain.Persistables;
using Domain.Services.Articles;
using Domain.Services.Injections;
using Domain.Shared;
using Microsoft.Extensions.DependencyInjection;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace Tasks.MasterData
{
    [Description("Импорт инжекций MatMas на сихронизацию мастер-данных по продуктам")]
    public class ImportMatMasTask : TaskBase
    {
        private const string InjectionType = "MatMasProducts";
        private const int ViewPortHours = 24;

        public void Execute(ImportMatMasProperties props)
        {
            try
            {
                IInjectionsService injectionsService = ServiceProvider.GetService<IInjectionsService>();
                ConnectionInfo sftpConnection = GetSftpConnection(props.ConnectionString);
                using (SftpClient sftpClient = new SftpClient(sftpConnection))
                {
                    DateTime barrierTime = DateTime.UtcNow.AddHours(-ViewPortHours);
                    IEnumerable<InjectionDto> processedInjections = injectionsService.GetLast(InjectionType, ViewPortHours);
                    HashSet<string> processedFileNames = new HashSet<string>(processedInjections.Select(i => i.FileName));

                    var files = sftpClient.ListDirectory(props.Folder);
                    files = files.Where(f => f.LastWriteTimeUtc >= barrierTime && f.IsRegularFile)
                                 .OrderBy(f => f.LastWriteTimeUtc);

                    foreach (SftpFile file in files)
                    {
                        string filePath = file.FullName;
                        if (!processedFileNames.Contains(file.Name))
                        {
                            Log.Information("Find new file {filePath}", filePath);

                            InjectionDto injection = new InjectionDto
                            {
                                Type = InjectionType,
                                FileName = file.Name,
                                ProcessTimeUtc = DateTime.UtcNow
                            };

                            try
                            {
                                string content = sftpClient.ReadAllText(filePath);
                                bool isSuccess = ProcessProductsFile(filePath, content);
                                injection.Status = isSuccess ? InjectionStatus.Success : InjectionStatus.Failed;
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, "Failed to process file {filePath}", filePath);
                                injection.Status = InjectionStatus.Failed;
                            }

                            injectionsService.SaveOrCreate(injection);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to process MatMas injections");
                throw ex;
            }
        }

        private ConnectionInfo GetSftpConnection(string connectionString)
        {
            Uri connection = new Uri(connectionString);
            string[] authParts = connection.UserInfo.Split(':');
            string login = authParts.Length == 2 ? authParts[0] : null;
            string password = authParts.Length == 2 ? authParts[1] : null;
            ConnectionInfo result = new ConnectionInfo(connection.Host, connection.Port, login, new PasswordAuthenticationMethod(login, password));
            return result;
        }

        private bool ProcessProductsFile(string fileName, string fileContent)
        {
            // Загружаем текущий список продуктов из базы
            IArticlesService articlesService = ServiceProvider.GetService<IArticlesService>();
            IEnumerable<ArticleDto> currentProducts = articlesService.Search(new SearchForm { Take = int.MaxValue }).Items;

            // Получаем список продуктов из файла, обновляем имеющиеся продукты
            bool hasErrors;
            IEnumerable<ArticleDto> products = LoadProductsFromFile(fileName, fileContent, currentProducts, out hasErrors);

            if (!hasErrors)
            {
                // Сохраняем изменения в базу
                articlesService.Import(products);
            }

            return !hasErrors;
        }

        private IEnumerable<ArticleDto> LoadProductsFromFile(string fileName, string fileContent, IEnumerable<ArticleDto> currentProducts, out bool hasErrors)
        {
            hasErrors = false;

            // Формируем справочники старых и новых значений
            Dictionary<string, ArticleDto> currentLookup = currentProducts.ToDictionary(p => p.Nart);
            Dictionary<string, ArticleDto> resultLookup = new Dictionary<string, ArticleDto>();

            // Загружаем данные из файла
            XmlDocument doc = new XmlDocument();
            using (StringReader reader = new StringReader(fileContent))
            {
                doc.Load(reader);
            }

            // Разбираем записи в файле по полям Dto
            int entryInd = 0;
            var productRoots = doc.DocumentElement.SelectNodes("//E1MARAM");
            foreach (XmlNode pRoot in productRoots)
            {
                ++entryInd;

                string nart = pRoot.SelectSingleNode("MATNR")?.InnerText;
                if (string.IsNullOrEmpty(nart))
                {
                    Log.Warning("Empty NART in entry #{entryInd} of file {fileName}, skipped.", entryInd, fileName);
                    continue;
                }

                // Ищем имеющуюся запись по Nart
                ArticleDto product;
                if (!currentLookup.TryGetValue(nart, out product))
                {
                    // Не нашли - создаем новую
                    product = new ArticleDto();
                    Log.Information("Find new product with NART = {nart} in file {fileName}", nart, fileName);
                }
                else
                {
                    Log.Information("Find update for product {Id} (NART = {nart}) in file {fileName}", product.Id, nart, fileName);
                }

                // Получение единиц измерения по каждому полю или группе смежных полей

                int weightNetUowCoeff = pRoot.ParseUom("GEWEI", entryInd, new[] { "GRM", "KGM" }, new[] { 1, 1000 }, 1);
                int weightPieceUowCoeff = pRoot.ParseUom("E1MARMM[MEINH='PCE']/GEWEI", entryInd, new[] { "GRM", "KGM" }, new[] { 1, 1000 }, 1);
                int weightShrinkUowCoeff = pRoot.ParseUom("E1MARMM[MEINH='#2R']/GEWEI", entryInd, new[] { "GRM", "KGM" }, new[] { 1, 1000 }, 1);
                int weightBoxUowCoeff = pRoot.ParseUom("E1MARMM[MEINH='CT']/GEWEI", entryInd, new[] { "GRM", "KGM" }, new[] { 1, 1000 }, 1);
                int weightLayerUowCoeff = pRoot.ParseUom("E1MARMM[MEINH='#18']/GEWEI", entryInd, new[] { "GRM", "KGM" }, new[] { 1, 1000 }, 1);
                int weightPalletUowCoeff = pRoot.ParseUom("E1MARMM[MEINH='PF']/GEWEI", entryInd, new[] { "GRM", "KGM" }, new[] { 1, 1000 }, 1);

                int sizePieceUowCoeff = pRoot.ParseUom("E1MARMM[MEINH='PCE']/MEABM", entryInd, new[] { "MMT", "MTR" }, new[] { 1, 1000 }, 1);
                int sizeShrinkUowCoeff = pRoot.ParseUom("E1MARMM[MEINH='#2R']/MEABM", entryInd, new[] { "MMT", "MTR" }, new[] { 1, 1000 }, 1);
                int sizeBoxUowCoeff = pRoot.ParseUom("E1MARMM[MEINH='CT']/MEABM", entryInd, new[] { "MMT", "MTR" }, new[] { 1, 1000 }, 1);
                int sizeLayerUowCoeff = pRoot.ParseUom("E1MARMM[MEINH='#18']/MEABM", entryInd, new[] { "MMT", "MTR" }, new[] { 1, 1000 }, 1);
                int sizePalletUowCoeff = pRoot.ParseUom("E1MARMM[MEINH='PF']/MEABM", entryInd, new[] { "MMT", "MTR" }, new[] { 1, 1000 }, 1);

                // Непосредственно заполнение полей

                product.SPGR = pRoot.SelectSingleNode("PRDHA")?.InnerText?.ExtractSPGR();
                product.Description = pRoot.SelectSingleNode("E1MAKTM[SPRAS_ISO='RU']/MAKTX")?.InnerText
                                   ?? pRoot.SelectSingleNode("E1MAKTM[SPRAS_ISO='EN']/MAKTX")?.InnerText;
                product.Nart = pRoot.SelectSingleNode("MATNR")?.InnerText;
                product.CountryOfOrigin = pRoot.SelectSingleNode("E1MARCM/HERKL")?.InnerText;
                product.ShelfLife = pRoot.ParseInt("MHDHB", entryInd);
                //product.Status = pRoot.SelectSingleNode("E1MARCM/MMSTA")?.InnerText;

                product.UnitLengthGoodsMm = pRoot.ParseDecimal("E1MARMM[MEINH='PCE']/LAENG", entryInd).ApplyUowCoeff(sizePieceUowCoeff);
                product.WidthUnitsGoodsMm = pRoot.ParseDecimal("E1MARMM[MEINH='PCE']/BREIT", entryInd).ApplyUowCoeff(sizePieceUowCoeff);
                product.UnitHeightGoodsMm = pRoot.ParseDecimal("E1MARMM[MEINH='PCE']/HOEHE", entryInd).ApplyUowCoeff(sizePieceUowCoeff);
                product.WeightUnitsGrossProductG = pRoot.ParseDecimal("E1MARMM[MEINH='PCE']/BRGEW", entryInd).ApplyUowCoeff(weightPieceUowCoeff);
                product.WeightUnitsNetGoodsG = pRoot.ParseDecimal("NTGEW", entryInd).ApplyUowCoeff(weightNetUowCoeff);

                product.EANShrink = pRoot.SelectSingleNode("E1MARMM[MEINH='#2R' and NUMTP='HK']/EAN11")?.InnerText
                                 ?? pRoot.SelectSingleNode("E1MARMM[MEINH='#2R' and NUMTP='HE']/EAN11")?.InnerText;
                product.PiecesInShrink = pRoot.ParseInt("E1MARMM[MEINH='#2R']/UMREZ", entryInd);
                product.LengthShrinkMm = pRoot.ParseDecimal("E1MARMM[MEINH='#2R']/LAENG", entryInd).ApplyUowCoeff(sizeShrinkUowCoeff);
                product.WidthShrinkMm = pRoot.ParseDecimal("E1MARMM[MEINH='#2R']/BREIT", entryInd).ApplyUowCoeff(sizeShrinkUowCoeff);
                product.HeightShrinkMm = pRoot.ParseDecimal("E1MARMM[MEINH='#2R']/HOEHE", entryInd).ApplyUowCoeff(sizeShrinkUowCoeff);
                product.GrossShrinkWeightG = pRoot.ParseDecimal("E1MARMM[MEINH='#2R']/BRGEW", entryInd).ApplyUowCoeff(weightShrinkUowCoeff);

                product.EANBox = pRoot.SelectSingleNode("E1MARMM[MEINH='CT' and NUMTP='HK']/EAN11")?.InnerText
                              ?? pRoot.SelectSingleNode("E1MARMM[MEINH='CT' and NUMTP='HE']/EAN11")?.InnerText;
                product.PiecesInABox = pRoot.ParseInt("E1MARMM[MEINH='CT']/UMREZ", entryInd);
                product.BoxLengthMm = pRoot.ParseDecimal("E1MARMM[MEINH='CT']/LAENG", entryInd).ApplyUowCoeff(sizeBoxUowCoeff);
                product.WidthOfABoxMm = pRoot.ParseDecimal("E1MARMM[MEINH='CT']/BREIT", entryInd).ApplyUowCoeff(sizeBoxUowCoeff);
                product.BoxHeightMm = pRoot.ParseDecimal("E1MARMM[MEINH='CT']/HOEHE", entryInd).ApplyUowCoeff(sizeBoxUowCoeff);
                product.GrossBoxWeightG = pRoot.ParseDecimal("E1MARMM[MEINH='CT']/BRGEW", entryInd).ApplyUowCoeff(weightBoxUowCoeff);

                product.PiecesInALayer = pRoot.ParseInt("E1MARMM[MEINH='#18']/UMREZ", entryInd);
                product.LayerLengthMm = pRoot.ParseDecimal("E1MARMM[MEINH='#18']/LAENG", entryInd).ApplyUowCoeff(sizeLayerUowCoeff);
                product.LayerWidthMm = pRoot.ParseDecimal("E1MARMM[MEINH='#18']/BREIT", entryInd).ApplyUowCoeff(sizeLayerUowCoeff);
                product.LayerHeightMm = pRoot.ParseDecimal("E1MARMM[MEINH='#18']/HOEHE", entryInd).ApplyUowCoeff(sizeLayerUowCoeff);
                product.GrossLayerWeightMm = pRoot.ParseDecimal("E1MARMM[MEINH='#18']/BRGEW", entryInd).ApplyUowCoeff(weightLayerUowCoeff);

                product.EANPallet = pRoot.SelectSingleNode("E1MARMM[MEINH='PF' and NUMTP='HK']/EAN11")?.InnerText
                                 ?? pRoot.SelectSingleNode("E1MARMM[MEINH='PF' and NUMTP='HE']/EAN11")?.InnerText;
                product.PiecesOnAPallet = pRoot.ParseInt("E1MARMM[MEINH='PF']/UMREZ", entryInd);
                product.PalletLengthMm = pRoot.ParseDecimal("E1MARMM[MEINH='PF']/LAENG", entryInd).ApplyUowCoeff(sizePalletUowCoeff);
                product.WidthOfPalletsMm = pRoot.ParseDecimal("E1MARMM[MEINH='PF']/BREIT", entryInd).ApplyUowCoeff(sizePalletUowCoeff);
                product.PalletHeightMm = pRoot.ParseDecimal("E1MARMM[MEINH='PF']/HOEHE", entryInd).ApplyUowCoeff(sizePalletUowCoeff);
                product.GrossPalletWeightG = pRoot.ParseDecimal("E1MARMM[MEINH='PF']/BRGEW", entryInd).ApplyUowCoeff(weightPalletUowCoeff);

                product.Status = "Активный";

                resultLookup[nart] = product;
            }

            // Если среди текущих есть такие записи, Nart которых отсутствовал в файле, деактивируем их
            foreach (ArticleDto product in currentProducts)
            {
                if (!resultLookup.ContainsKey(product.Nart))
                {
                    product.Status = "Неактивный";
                    resultLookup[product.Nart] = product;
                    Log.Information("Deactivate obsolete product {Id} (NART = {Nart}) based on file {fileName}", product.Id, product.Nart, fileName);
                }
            }

            return resultLookup.Values;
        }
    }

    internal static class Extensions
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
                Log.Warning("Length of SPGR should equal 5 but {rawValue} found.", rawValue);
                return rawValue;
            }
            else
            {
                return rawValue.Substring(rawValue.Length - 5);
            }
        }

        public static int ParseUom(this XmlNode node, string xPath, int entryInd, string[] keys, int[] values, int defaultValue)
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
                Log.Warning("Unkown value {actualKey} in element {xPath} of entry #{entryInd}.", actualKey, xPath, entryInd);
            }
            else
            {
                Log.Warning("Element {xPath} not found in entry #{entryInd}.", xPath, entryInd);
            }
            return defaultValue;
        }

        public static decimal? ParseDecimal(this XmlNode node, string xPath, int entryInd)
        {
            string value = node.SelectSingleNode(xPath)?.InnerText;
            if (!string.IsNullOrEmpty(value))
            {
                decimal result;
                if (decimal.TryParse(value.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out result))
                {
                    return result;
                }
                else
                {
                    Log.Warning("Invalid number {value} in element {xPath} of entry #{entryInd}.", value, xPath, entryInd);
                }
            }
            return null;
        }

        public static int? ParseInt(this XmlNode node, string xPath, int entryInd)
        {
            string value = node.SelectSingleNode(xPath)?.InnerText;
            if (!string.IsNullOrEmpty(value))
            {
                int result;
                if (int.TryParse(value.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out result))
                {
                    return result;
                }
                else
                {
                    Log.Warning("Invalid number {value} in element {xPath} of entry #{entryInd}.", value, xPath, entryInd);
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
