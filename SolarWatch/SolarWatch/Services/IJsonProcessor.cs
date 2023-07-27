using SolarWatch;

namespace WeatherApi.Services;

public interface IJsonProcessor
{
    WeatherForecast Process(string data);
}