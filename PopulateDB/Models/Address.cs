using System.Text.Json.Serialization;

namespace PopulateDB.Models
{
    public class Address
    {
        [JsonPropertyName("street")]
        public string Street { get; set; } = string.Empty;

        [JsonPropertyName("suite")]
        public string Suite { get; set; } = string.Empty;

        [JsonPropertyName("city")]
        public string City { get; set; } = string.Empty;

        [JsonPropertyName("zipcode")]
        public string ZipCode { get; set; } = string.Empty;

        [JsonPropertyName("geo")]
        public Geo Geo { get; set; } = default!;
    }
}
