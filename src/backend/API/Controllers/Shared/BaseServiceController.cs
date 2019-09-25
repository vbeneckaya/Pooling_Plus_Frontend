using Domain;
using Domain.Persistables;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Shared
{
    public abstract class BaseServiceController<TService, TEntity, TDto> : Controller where TService : IService where TEntity : IPersistable where TDto :IDto
    {
        protected readonly TService service;

        public BaseServiceController(TService service)
        {
            this.service = service;
        }
    }
}