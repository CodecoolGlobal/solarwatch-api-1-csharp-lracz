using System.Text.Json.Serialization;

namespace WeatherApi.Services;

public class GeocodingResponse
{
   
    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lon")]
    public double Lon { get; set; }
    
}
