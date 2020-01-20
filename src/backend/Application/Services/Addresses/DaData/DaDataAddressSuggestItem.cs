using Newtonsoft.Json;

namespace Application.Services.Addresses.DaData
{
    public class DaDataAddressSuggestItem
    {
        [JsonProperty("value")]
        public string FullName { get; set; }

        [JsonProperty("data")]
        public DaDataAddressData Data { get; set; }
    }
}
