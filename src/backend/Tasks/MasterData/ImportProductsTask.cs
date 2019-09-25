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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using Tasks.Helpers;

namespace Tasks.MasterData
{
    [Description("Импорт инжекций MatMas на сихронизацию мастер-данных по продуктам")]
    public class ImportProductsTask : TaskBase
    {
        public void Execute(ImportProductsProperties props)
        {
            if (string.IsNullOrEmpty(props.ConnectionString))
            {
                throw new Exception("ConnectionString является обязательным параметром");
            }

            if (string.IsNullOrEmpty(props.Folder))
            {
                props.Folder = "/";
            }

            if (string.IsNullOrEmpty(props.FileNamePattern))
            {
                props.FileNamePattern = @"^.*MATMAS.*\.xml$";
            }

            if (string.IsNullOrEmpty(props.ViewHours))
            {
                props.ViewHours = "24";
            }

            int viewHours;
            if (!int.TryParse(props.ViewHours, out viewHours))
            {
                throw new Exception("Параметр ViewHours должен быть целым числом");
            }

            try
            {
                Regex fileNameRe = new Regex(props.FileNamePattern, RegexOptions.IgnoreCase);
                IInjectionsService injectionsService = ServiceProvider.GetService<IInjectionsService>();

                ConnectionInfo sftpConnection = GetSftpConnection(props.ConnectionString);
                using (SftpClient sftpClient = new SftpClient(sftpConnection))
                {
                    sftpClient.Connect();

                    DateTime barrierTime = DateTime.UtcNow.AddHours(-viewHours);
                    IEnumerable<InjectionDto> processedInjections = injectionsService.GetByTaskName(TaskName);
                    HashSet<string> processedFileNames = new HashSet<string>(processedInjections.Select(i => i.FileName));

                    var files = sftpClient.ListDirectory(props.Folder);
                    files = files.Where(f => f.LastWriteTimeUtc >= barrierTime && f.IsRegularFile)
                                 .OrderBy(f => f.LastWriteTimeUtc);

                    foreach (SftpFile file in files)
                    {
                        if (!fileNameRe.IsMatch(file.Name))
                        {
                            continue;
                        }

                        if (!processedFileNames.Contains(file.Name))
                        {
                            Log.Information("Найден новый файл: {FullName}.", file.FullName);

                            InjectionDto injection = new InjectionDto
                            {
                                Type = TaskName,
                                FileName = file.Name,
                                ProcessTimeUtc = DateTime.UtcNow
                            };

                            try
                            {
                                string content = sftpClient.ReadAllText(file.FullName);
                                bool isSuccess = ProcessProductsFile(file.Name, content);
                                injection.Status = isSuccess ? InjectionStatus.Success : InjectionStatus.Failed;
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, "Не удалось обработать файл {Name}.", file.Name);
                                injection.Status = InjectionStatus.Failed;
                            }

                            injectionsService.SaveOrCreate(injection);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Ошибка при обработке {TaskName} инжекции.");
                throw ex;
            }
        }

        private ConnectionInfo GetSftpConnection(string connectionString)
        {
            Uri connection = new Uri(connectionString);
            string[] authParts = connection.UserInfo.Split(':');
            string login = authParts.Length == 2 ? HttpUtility.UrlDecode(authParts[0]) : null;
            string password = authParts.Length == 2 ? HttpUtility.UrlDecode(authParts[1]) : null;
            int port = connection.Port > 0 ? connection.Port : 22;
            ConnectionInfo result = new ConnectionInfo(connection.Host, port, login, new PasswordAuthenticationMethod(login, password));
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

                string nart = pRoot.SelectSingleNode("MATNR")?.InnerText?.TrimStart('0');
                if (string.IsNullOrEmpty(nart))
                {
                    Log.Warning("Пустое значение NART в записи #{entryInd} файла {fileName}, запись пропущена.", entryInd, fileName);
                    continue;
                }

                // Ищем имеющуюся запись по Nart
                ArticleDto product;
                if (!currentLookup.TryGetValue(nart, out product))
                {
                    // Не нашли - создаем новую
                    product = new ArticleDto();
                    Log.Information("Найден новый продукт NART = {nart} в файле {fileName}.", nart, fileName);
                }
                else
                {
                    Log.Information("Обновлен продукт {Id} (NART = {nart}) данными из файла {fileName}.", product.Id, nart, fileName);
                }

                // Получение единиц измерения по каждому полю или группе смежных полей

                int weightNetUowCoeff = pRoot.ParseUom("GEWEI", new[] { "GRM", "KGM" }, new[] { 1, 1000 }, 1, entryInd);
                int weightPieceUowCoeff = pRoot.ParseUom("E1MARMM[MEINH='PCE']/GEWEI", new[] { "GRM", "KGM" }, new[] { 1, 1000 }, 1, entryInd);
                int weightShrinkUowCoeff = pRoot.ParseUom("E1MARMM[MEINH='#2R']/GEWEI", new[] { "GRM", "KGM" }, new[] { 1, 1000 }, 1, entryInd);
                int weightBoxUowCoeff = pRoot.ParseUom("E1MARMM[MEINH='CT']/GEWEI", new[] { "GRM", "KGM" }, new[] { 1, 1000 }, 1, entryInd);
                int weightLayerUowCoeff = pRoot.ParseUom("E1MARMM[MEINH='#18']/GEWEI", new[] { "GRM", "KGM" }, new[] { 1, 1000 }, 1, entryInd);
                int weightPalletUowCoeff = pRoot.ParseUom("E1MARMM[MEINH='PF']/GEWEI", new[] { "GRM", "KGM" }, new[] { 1, 1000 }, 1, entryInd);

                int sizePieceUowCoeff = pRoot.ParseUom("E1MARMM[MEINH='PCE']/MEABM", new[] { "MMT", "MTR" }, new[] { 1, 1000 }, 1, entryInd);
                int sizeShrinkUowCoeff = pRoot.ParseUom("E1MARMM[MEINH='#2R']/MEABM", new[] { "MMT", "MTR" }, new[] { 1, 1000 }, 1, entryInd);
                int sizeBoxUowCoeff = pRoot.ParseUom("E1MARMM[MEINH='CT']/MEABM", new[] { "MMT", "MTR" }, new[] { 1, 1000 }, 1, entryInd);
                int sizeLayerUowCoeff = pRoot.ParseUom("E1MARMM[MEINH='#18']/MEABM", new[] { "MMT", "MTR" }, new[] { 1, 1000 }, 1, entryInd);
                int sizePalletUowCoeff = pRoot.ParseUom("E1MARMM[MEINH='PF']/MEABM", new[] { "MMT", "MTR" }, new[] { 1, 1000 }, 1, entryInd);

                // Непосредственно заполнение полей

                product.SPGR = pRoot.SelectSingleNode("PRDHA")?.InnerText?.ExtractSPGR();
                product.Description = pRoot.SelectSingleNode("E1MAKTM[SPRAS_ISO='RU']/MAKTX")?.InnerText
                                   ?? pRoot.SelectSingleNode("E1MAKTM[SPRAS_ISO='EN']/MAKTX")?.InnerText;
                product.Nart = nart;
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
                    Log.Information("Деактивирован устаревший продукт {Id} (NART = {Nart}) на основании файла {fileName}.", product.Id, product.Nart, fileName);
                }
            }

            return resultLookup.Values;
        }
    }
}
