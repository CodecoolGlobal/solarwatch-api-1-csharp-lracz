using System.Net;

namespace WeatherApi.Services;

public class OpenWeatherMapApi: IWeatherDataProvider
{
    private readonly ILogger<OpenWeatherMapApi> _logger;
    
    public OpenWeatherMapApi(ILogger<OpenWeatherMapApi> logger)
    {
        _logger = logger;
    }

    public string GetCurrent(double lat, double lon)
    {
        var apiKey = "2929327ff6b0d883f136039297eb4843";
        
        
        var url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={apiKey}&units=metric";

        var client = new WebClient();

        _logger.LogInformation("Calling OpenWeather API with url: {url}", url);
        return client.DownloadString(url);
    }
    public async Task<string> GetCurrentAsync(double lat, double lon)
    {
        var apiKey = "2929327ff6b0d883f136039297eb4843";
        var url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={apiKey}&units=metric";

        using var client = new HttpClient();
        _logger.LogInformation("Calling OpenWeather API with url: {url}", url);

        var response = await client.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }
}