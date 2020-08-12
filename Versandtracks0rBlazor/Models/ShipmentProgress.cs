using System;
using System.Text.Json.Serialization;

namespace Versandtracks0rBlazor.Models
{
    public class ShipmentProgress
    {
        public string Location { get; set; }
        public double Long { get; set; }
        public double Lat { get; set; }
        public int Id { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Message { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ShipmentStatus Status { get; set; }
    }
}
