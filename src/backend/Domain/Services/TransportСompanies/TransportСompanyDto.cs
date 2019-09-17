namespace Domain.Services.TransportСompanies
{
    public class TransportСompanyDto : IDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ContractNumber { get; set; }
        public string DateOfPowerOfAttorney { get; set; }
        /*end of fields*/
    }
}