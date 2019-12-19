using Domain.Services;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace API.Controllers
{
    public class StateController<T> :  Controller
    {
        private readonly IStateService _stateService;

        public StateController(IStateService stateService)
        {
            _stateService = stateService;
        }

        /// <summary>
        /// Все доступные статусы с цветами
        /// </summary>
        [HttpPost("search")]
        public IEnumerable<StateDto> GetAll()
        {
            var result = _stateService.GetAll<T>();
            return result;
        }

        /// <summary>
        /// Все доступные статусы
        /// </summary>
        [HttpGet("forSelect")]
        public IEnumerable<LookUpDto> ForSelect()
        {
            var result = _stateService.ForSelect<T>();
            return result;
        }
    }
}