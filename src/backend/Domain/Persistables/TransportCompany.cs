using System;
using Domain.Enums;

namespace Domain.Persistables
{
    public class TransportCompany : IPersistable
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ContractNumber { get; set; }
        public string DateOfPowerOfAttorney { get; set; }
        /*end of fields*/
    }
}