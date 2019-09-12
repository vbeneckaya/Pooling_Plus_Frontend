using System;
using Application.Shared;
using DAL;
using Domain.Persistables;
using Domain.Services.Orders;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Orders
{
    public class OrdersService : GridServiceBase<Order, OrderDto>, IOrdersService
    {
        public OrdersService(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public override DbSet<Order> UseDbSet(AppDbContext dbContext)
        {
            return dbContext.Orders;
        }

        public override void MapFromDtoToEntity(Order entity, OrderDto dto)
        {
            if(!string.IsNullOrEmpty(dto.Id))
                entity.Id = Guid.Parse(dto.Id);
            /*end of fields*/
        }

        public override OrderDto MapFromEntityToDto(Order entity)
        {
            return new OrderDto
            {
                Id = entity.Id.ToString(),
                /*end of fields*/
            };
        }
    }
}