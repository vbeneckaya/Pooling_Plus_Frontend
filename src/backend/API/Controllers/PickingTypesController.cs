using Domain.Services.PickingTypes;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/pickingTypes")]
    public class PickingTypesController
    {
        /// <summary>
        /// Все доступные статусы
        /// </summary>
        [HttpGet("forSelect")]
        public IEnumerable<LookUpDto> ForSelect()
        {
            return _pickingTypesService.ForSelect();
        }

        public PickingTypesController(IPickingTypesService pickingTypesService)
        {
            _pickingTypesService = pickingTypesService;
        }

        private readonly IPickingTypesService _pickingTypesService;
    }
}