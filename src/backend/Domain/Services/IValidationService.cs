using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services
{
    public interface IValidationService
    {
        ValidatedRecord<TDto> Validate<TDto>(TDto dto);
    }
}
