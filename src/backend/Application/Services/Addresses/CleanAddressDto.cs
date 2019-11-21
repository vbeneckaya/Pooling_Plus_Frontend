namespace Application.Services.Addresses
{
    public class CleanAddressDto
    {
        public string ResultAddress { get; set; }
        public string PostalCode { get; set; }
        public string Region { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string UnparsedAddressParts { get; set; }
    }
}
