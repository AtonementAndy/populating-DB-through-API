﻿using System.Text.Json.Serialization;

namespace PopulateDB.Models
{
    public class Geo
    {
        [JsonPropertyName("lat")]
        public string Lat { get; set; } = default!;

        [JsonPropertyName("lng")]
        public string Lng { get; set; } = default!;
    }
}