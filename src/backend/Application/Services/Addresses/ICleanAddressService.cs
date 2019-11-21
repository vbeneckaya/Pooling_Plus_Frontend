namespace Application.Services.Addresses
{
    public interface ICleanAddressService
    {
        CleanAddressDto CleanAddress(string address);
    }
}