using Newtonsoft.Json;

namespace Application.Services.Addresses.DaData
{
    public class DaDataCleanAddressAnswer : DaDataAddressData
    {
        [JsonProperty("result")]
        public string Result { get; set; }
    }
}
