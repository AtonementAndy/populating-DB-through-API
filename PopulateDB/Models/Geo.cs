using System.Text.Json.Serialization;

namespace PopulateDB.Models
{
    public class Geo
    {
        [JsonPropertyName("lat")]
        public string Lat { get; set; } = string.Empty;

        [JsonPropertyName("lng")]
        public string Lng { get; set; } = string.Empty;
    }
}
