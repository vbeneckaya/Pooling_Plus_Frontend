using Domain.Persistables;
using Domain.Services.Injections;
using Domain.Services.Orders;
using Domain.Services.ShippingWarehouses;
using Domain.Services.Warehouses;
using Microsoft.Extensions.Configuration;
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
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Tasks.Common;
using Tasks.Helpers;

namespace Tasks.Orders
{
    [Description("Импорт инжекций на создание нового заказа")]
    public class ImportOrderTask : TaskBase<ImportOrderProperties>, IScheduledTask
    {
        public string Schedule => "*/5 * * * *";

        protected override async Task Execute(IServiceProvider serviceProvider, ImportOrderProperties props, CancellationToken cancellationToken)
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
                IInjectionsService injectionsService = serviceProvider.GetService<IInjectionsService>();

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
                                bool isSuccess = ProcessOrderFile(serviceProvider, file.Name, content);
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

        private bool ProcessOrderFile(IServiceProvider serviceProvider, string fileName, string fileContent)
        {
            IWarehousesService warehousesService = serviceProvider.GetService<IWarehousesService>();
            IShippingWarehousesService shippingWarehousesService = serviceProvider.GetService<IShippingWarehousesService>();
            IOrdersService ordersService = serviceProvider.GetService<IOrdersService>();

            // Загружаем данные из файла
            XmlDocument doc = new XmlDocument();
            using (StringReader reader = new StringReader(fileContent))
            {
                doc.Load(reader);
            }

            List<OrderFormDto> orders = new List<OrderFormDto>();
            List<WarehouseDto> updWarehouses = new List<WarehouseDto>();
            var docRoots = doc.SelectNodes("//IDOC");

            int totalCount = docRoots.Count;
            int processedCount = 0;
            foreach (XmlNode docRoot in docRoots)
            {
                ++processedCount;

                string orderNumber = docRoot.SelectSingleNode("E1EDK02[QUALF='002']/BELNR")?.InnerText;
                OrderFormDto dto = ordersService.GetFormByNumber(orderNumber);
                bool isNew = dto == null;
                if (dto == null)
                {
                    dto = new OrderFormDto();
                }

                dto.AdditionalInfo = $"INJECTION - {fileName}";

                decimal weightUomCoeff = docRoot.ParseUom("E1EDK01/GEWEI", new[] { "GRM", "GR", "KGM", "KG" }, new[] { 0.001M, 0.001M, 1M, 1M }, 1);

                dto.OrderNumber = orderNumber;
                dto.OrderDate = docRoot.ParseDateTime("E1EDK02[QUALF='001']/DATUM")?.ToString("dd.MM.yyyy") ?? dto.OrderDate;
                dto.SoldTo = docRoot.SelectSingleNode("E1EDKA1[PARVW='AG']/PARTN")?.InnerText?.TrimStart('0') ?? dto.SoldTo;
                dto.WeightKg = docRoot.ParseDecimal("E1EDK01/BRGEW").ApplyDecimalUowCoeff(weightUomCoeff) ?? dto.WeightKg;
                dto.BoxesCount = docRoot.ParseDecimal("E1EDK01/Y0126SD_ORDERS05_TMS_01/YYCAR_H") ?? dto.BoxesCount;
                dto.DeliveryDate = docRoot.ParseDateTime("E1EDK03[IDDAT='002']/DATUM")?.ToString("dd.MM.yyyy") ?? dto.DeliveryDate;
                dto.OrderAmountExcludingVAT = docRoot.ParseDecimal("E1EDS01[SUMID='002']/SUMME") ?? dto.OrderAmountExcludingVAT;

                string shippingAddressCode = docRoot.SelectSingleNode("E1EDP01/WERKS")?.InnerText;
                var shippingWarehouse = shippingWarehousesService.GetByCode(shippingAddressCode);
                dto.ShippingAddress = shippingWarehouse?.Address ?? dto.ShippingAddress;
                dto.ShippingCity = shippingWarehouse?.City ?? dto.ShippingCity;
                dto.ShippingWarehouseId = shippingWarehouse?.Id.ToString() ?? dto.ShippingWarehouseId;

                string deliveryCity = docRoot.SelectSingleNode("E1EDKA1[PARVW='WE']/ORT01")?.InnerText;
                string deliveryAddress = docRoot.SelectSingleNode("E1EDKA1[PARVW='WE']/STRAS")?.InnerText;
                string deliveryAddress2 = docRoot.SelectSingleNode("E1EDKA1[PARVW='WE']/STRS2")?.InnerText;
                if (!string.IsNullOrEmpty(deliveryAddress2))
                {
                    deliveryAddress = (string.IsNullOrEmpty(deliveryAddress) ? string.Empty : deliveryAddress + " ") + deliveryAddress2;
                }

                var deliveryWarehouse = warehousesService.GetBySoldTo(dto.SoldTo);
                if (deliveryWarehouse == null)
                {
                    dto.ClientName = null;
                    dto.DeliveryAddress = deliveryAddress;
                    dto.DeliveryCity = deliveryCity;
                    dto.DeliveryRegion = null;
                    dto.PickingTypeId = null;
                    dto.TransitDays = null;
                    dto.DeliveryType = null;
                }
                else
                {
                    deliveryWarehouse.City = deliveryCity ?? deliveryWarehouse.City;
                    deliveryWarehouse.Address = deliveryAddress ?? deliveryWarehouse.Address;
                    updWarehouses.Add(deliveryWarehouse);

                    dto.DeliveryCity = deliveryWarehouse.City;
                    dto.DeliveryAddress = deliveryWarehouse.Address;
                }

                if (isNew)
                {
                    dto.ClientOrderNumber = docRoot.SelectSingleNode("E1EDK02[QUALF='001']/BELNR")?.InnerText ?? dto.ClientOrderNumber;
                    dto.Payer = docRoot.SelectSingleNode("E1EDKA1[PARVW='RG']/PARTN")?.InnerText?.TrimStart('0') ?? dto.Payer;
                }

                if (isNew || dto.ManualPalletsCount != true)
                {
                    dto.PalletsCount = docRoot.ParseInt("E1EDK01/Y0126SD_ORDERS05_TMS_01/YYPAL_H") ?? dto.PalletsCount;
                }

                IEnumerable<string> missedRequiredFields = ValidateRequiredFields(dto);
                if (missedRequiredFields.Any())
                {
                    string fields = string.Join(", ", missedRequiredFields);
                    Log.Error("В файле {fileName} отсутствуют следующие обязательные поля: {fields}. Заказ ({processedCount}/{totalCount}) не создан.", 
                              fileName, fields, processedCount, totalCount);
                }
                else
                {
                    int entryInd = 0;
                    var itemRoots = docRoot.SelectNodes("E1EDP01");
                    dto.Items = dto.Items ?? new List<OrderItemDto>();
                    var updatedItems = new HashSet<string>();
                    foreach (XmlNode itemRoot in itemRoots)
                    {
                        ++entryInd;

                        string posex = itemRoot.SelectSingleNode("POSEX")?.InnerText ?? string.Empty;
                        int posexNum = -1;
                        int.TryParse(posex.TrimStart('0'), out posexNum);
                        if ((posexNum % 10) != 0)
                        {
                            continue;
                        }

                        string nart = itemRoot.SelectSingleNode("E1EDP19/IDTNR")?.InnerText?.TrimStart('0');
                        if (string.IsNullOrEmpty(nart))
                        {
                            Log.Warning("Пустое значение NART в позиции #{entryInd} заказа ({processedCount}/{totalCount}) из файла {fileName}, пропуск.", 
                                        entryInd, processedCount, totalCount, fileName);
                            continue;
                        }

                        int? quantity = itemRoot.ParseInt("MENGE");
                        if (quantity == null || quantity == 0)
                        {
                            Log.Warning("Пустое количество в позиции #{entryInd} заказа ({processedCount}/{totalCount}) из файла {fileName}, пропуск.",
                                        entryInd, processedCount, totalCount, fileName);
                            continue;
                        }

                        OrderItemDto itemDto = dto.Items.Where(i => i.Nart == nart).FirstOrDefault();

                        if (itemDto == null)
                        {
                            itemDto = new OrderItemDto();
                            dto.Items.Add(itemDto);
                        }
                        else
                        {
                            updatedItems.Add(itemDto.Id);
                        }

                        itemDto.Nart = nart;
                        itemDto.Quantity = quantity ?? itemDto.Quantity;
                    }

                    var itemsToRemove = dto.Items.Where(x => !string.IsNullOrEmpty(x.Id) && !updatedItems.Contains(x.Id)).ToList();
                    itemsToRemove.ForEach(x => dto.Items.Remove(x));

                    if (isNew)
                    {
                        Log.Information("Создан новый заказ {OrderNumber} ({processedCount}/{totalCount}) на основании файла {fileName}.",
                                        dto.OrderNumber, processedCount, totalCount, fileName);
                    }
                    else
                    {
                        Log.Information("Обновлен заказ {OrderNumber} ({processedCount}/{totalCount}) на основании файла {fileName}.",
                                        dto.OrderNumber, processedCount, totalCount, fileName);
                    }

                    orders.Add(dto);
                }
            }

            bool isSuccess = orders.Any();
            if (isSuccess)
            {
                warehousesService.Import(updWarehouses);
                ordersService.Import(orders);
            }

            return isSuccess;
        }

        private IEnumerable<string> ValidateRequiredFields(OrderDto dto)
        {
            if (string.IsNullOrEmpty(dto.OrderNumber))
            {
                yield return "Номер накладной BDF";
            }
            if (string.IsNullOrEmpty(dto.ClientOrderNumber))
            {
                yield return "Номер заказа клиента";
            }
            if (dto.OrderDate == null)
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
