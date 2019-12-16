using Domain.Persistables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.Companies
{
    public interface ICompaniesService : IDictonaryService<Company, CompanyDto>
    {
    }
}
