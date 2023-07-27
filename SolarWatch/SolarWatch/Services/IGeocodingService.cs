namespace WeatherApi.Services;

public interface IGeocodingService
{
    Task<(double latitude, double longitude)> GetCoordinatesForCityAsync(string city);
}