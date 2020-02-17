using Domain.Shared;

namespace Domain.Services.Tariffs
{
    public class TariffDtoForProvider : TariffDto
    {
        public LookUpDto ProviderId { get; set; }
    }
}