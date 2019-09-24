using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using Domain;
using Domain.Enums;
using Domain.Persistables;
using Domain.Services;
using Newtonsoft.Json;

namespace Application.Actions.Orders
{
    public class SaveOrder : IAppAction<Order>
    {
        private readonly AppDbContext db;

        public SaveOrder(AppDbContext db)
        {
            this.db = db;
            Color = AppColor.Blue;
        }

        public AppColor Color { get; set; }

        public AppActionResult Run(User user, Order order)
        {
            //var order = orders.ElementAt(0);
            if(!order.SalesOrderNumber.StartsWith("1") && !order.SalesOrderNumber.StartsWith("2"))
                return new AppActionResult
                {
                    IsError = true,
                    Message = $"Неравильный 'Номер заказа клиента', не начинается с 1 или 2"
                };

            if(string.IsNullOrEmpty(order.SoldTo))
                return new AppActionResult
                {
                    IsError = true,
                    Message = $"Отсутствует SoldTo"
                };

            if(string.IsNullOrEmpty(order.DeliveryDate))
                return new AppActionResult
                {
                    IsError = true,
                    Message = $"Отсутствует Дата доставки"
                };

            var soldToWarehouse = db.Warehouses.FirstOrDefault(x => x.SoldToNumber == order.SoldTo);
            var fromWarehouse = db.Warehouses.FirstOrDefault(x => x.CustomerWarehouse == "Нет");
            
            if(soldToWarehouse == null)
                return new AppActionResult
                {
                    IsError = true,
                    Message = $"Не найден склад для SoldToNumber = {order.SoldTo}"
                };

            if(fromWarehouse == null)
                return new AppActionResult
                {
                    IsError = true,
                    Message = $"в базе не найден склад клиента у которого Склад клиента = Нет"
                };

            order.CustomerName = soldToWarehouse.TheNameOfTheWarehouse;
            
            //TODO Сделать флаг для склада, в зависимости от него брать или нет Тип комплектации из склада
            if (!new List<string> {"Атак", "Ашан", "Перекресток"}.Contains(order.CustomerName))
                order.TypeOfEquipment = soldToWarehouse.TypeOfEquipment;

            if(string.IsNullOrEmpty(soldToWarehouse.LeadtimeDays))
                return new AppActionResult
                {
                    IsError = true,
                    Message = $"Не заполнено дней в пути у склада"
                };
            
            order.ShippingDate = DateTime.Parse(order.DeliveryDate).AddDays(0 - int.Parse(soldToWarehouse.LeadtimeDays)).ToString();

            order.DaysOnTheRoad = int.Parse(soldToWarehouse.LeadtimeDays);

            order.ShippingAddress = fromWarehouse.Address;
            
            order.DeliveryAddress = soldToWarehouse.Address;

            order.Status = OrderState.Created;
            order.TypeOfOrder = "OR";
            
            if(order.SalesOrderNumber.StartsWith("1"))
                order.TypeOfOrder = "OR";
            else
                order.TypeOfOrder = "OR";

            //Убираем вначале нули
            order.Payer = order.Payer.TrimStart('0');


            var orderPositions = JsonConvert.DeserializeObject<List<OrderPosition>>(order.Positions);
             
            order.NumberOfArticles = orderPositions.Count.ToString();
            order.OrderCreationDate = DateTime.Now.Date.ToShortDateString();
            
            db.SaveChanges();
            
            return new AppActionResult
            {
                IsError = false,
                Message = $"Заказ {order.Id} создан"
            };
        }

        public bool IsAvailable(Role role, Order order)
        {
            //var order = orders.ElementAt(0);
            return (order.Status == OrderState.Draft) && (role.Name == "Administrator" || role.Name == "TransportCoordinator");
        }
    }

    public class OrderPosition
    {
        public string Nart { get; set; }
        public string Count { get; set; }
    }
}