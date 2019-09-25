using Domain.Persistables;
using Domain.Services.Injections;
using Domain.Services.Orders;
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

namespace Tasks.Orders
{
    [Description("Импорт инжекций на создание нового заказа")]
    public class ImportOrderTask : TaskBase
    {
        public void Execute(ImportOrderProperties props)
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
                props.FileNamePattern = @"^.*ORD.*\.xml$";
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
                    IEnumerable<InjectionDto> processedInjections = injectionsService.GetLast(TaskName, viewHours);
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
                                bool isSuccess = ProcessOrderFile(file.Name, content);
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

        private bool ProcessOrderFile(string fileName, string fileContent)
        {
            IOrdersService ordersService = ServiceProvider.GetService<IOrdersService>();

            // Загружаем данные из файла
            XmlDocument doc = new XmlDocument();
            using (StringReader reader = new StringReader(fileContent))
            {
                doc.Load(reader);
            }

            OrderDto dto = new OrderDto();

            int weightUomCoeff = doc.ParseUom("//E1EDK01/GEWEI", new[] { "GRM", "KGM" }, new[] { 1, 1000 }, 1);

            dto.SalesOrderNumber = doc.SelectSingleNode("//E1EDK02[QUALF='001']/BELNR")?.InnerText;
            dto.OrderDate = doc.SelectSingleNode("//E1EDK02[QUALF='001']/DATUM")?.InnerText;
            dto.Payer = doc.SelectSingleNode("//E1EDKA1[PARVW='RG']/PARTN")?.InnerText?.TrimStart('0');
            dto.SoldTo = doc.SelectSingleNode("//E1EDKA1[PARVW='AG']/PARTN")?.InnerText?.TrimStart('0');
            dto.WeightKg = doc.ParseDecimal("//E1EDK01/BRGEW").ApplyUowCoeff(weightUomCoeff)?.ToString();
            dto.PreliminaryNumberOfPallets = doc.SelectSingleNode("//Y0126SD_ORDERS05_TMS_01/YYPAL_H")?.InnerText;
            dto.TheNumberOfBoxes = doc.SelectSingleNode("//Y0126SD_ORDERS05_TMS_01/YYCAR_H")?.InnerText;
            dto.DeliveryDate = doc.SelectSingleNode("//E1EDK03[IDDAT='002']/DATUM")?.InnerText;
            dto.OrderAmountExcludingVAT = doc.SelectSingleNode("//E1EDS01[SUMID='002']/SUMME")?.InnerText;

            IEnumerable<string> missedRequiredFields = ValidateRequiredFields(dto);
            if (missedRequiredFields.Any())
            {
                string fields = string.Join(", ", missedRequiredFields);
                Log.Error("В файле {fileName} отсутствуют следующие обязательные поля: {fields}. Заказ не создан.", fileName, fields);
                return false;
            }

            int entryInd = 0;
            var itemRoots = doc.SelectNodes("//E1EDP01");
            dto.Items = new List<OrderItemDto>();
            foreach (XmlNode itemRoot in itemRoots)
            {
                ++entryInd;
                OrderItemDto itemDto = new OrderItemDto();

                itemDto.Quantity = itemRoot.ParseInt("MENGE", entryInd) ?? 0;
                itemDto.Nart = itemRoot.SelectSingleNode("E1EDP19/IDTNR")?.InnerText?.TrimStart('0');

                dto.Items.Add(itemDto);
            }

            Log.Information("Создан новый заказ {SalesOrderNumber} на основании файла {fileName}.", dto.SalesOrderNumber, fileName);

            ordersService.SaveOrCreate(dto);

            return true;
        }

        private IEnumerable<string> ValidateRequiredFields(OrderDto dto)
        {
            if (string.IsNullOrEmpty(dto.SalesOrderNumber))
            {
                yield return "Номер заказа клиента";
            }
            if (string.IsNullOrEmpty(dto.OrderDate))
            {
                yield return "Дата заказа";
            }
            if (string.IsNullOrEmpty(dto.Payer))
            {
                yield return "Плательщик";
            }
            if (string.IsNullOrEmpty(dto.SoldTo))
            {
                yield return "Номер заказа клиента";
            }
        }
    }
}
