using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SolarWatch;
using SolarWatch.Context;
using SolarWatch.Model;
using WeatherApi.Services;

namespace WeatherApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IWeatherDataProvider _weatherDataProvider;
    private readonly IJsonProcessor _jsonProcessor;
    private readonly ISunriseSunsetService _sunriseSunsetService;
    private readonly IGeocodingService _geocodingService;
    private readonly ICityRepository _cityRepository;
    private readonly WeatherApiContext _dbContext;
    private readonly ISunriseSunsetRepository _sunriseSunsetRepository;

    public WeatherForecastController(ILogger<WeatherForecastController> logger,
        IWeatherDataProvider weatherDataProvider, IJsonProcessor jsonProcessor,
        IGeocodingService geocodingService, ISunriseSunsetService sunriseSunsetService,
        ICityRepository cityRepository, 
        ISunriseSunsetRepository sunriseSunsetRepository)
    {
        _logger = logger;
        _weatherDataProvider = weatherDataProvider;
        _jsonProcessor = jsonProcessor;
        _sunriseSunsetService = sunriseSunsetService;
        _geocodingService = geocodingService;
        _cityRepository = cityRepository;
   
        _sunriseSunsetRepository = sunriseSunsetRepository;
    }

    [HttpGet("GetWeatherForecast")]
    public ActionResult<IEnumerable<WeatherForecast>> Get(DateTime date)
    {
        if (date.Year < 2023)
        {
            return NotFound("Invalid date. Please provide a date before 2023.");
        }

        var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = date.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

        return Ok(forecasts);
    }

    // [HttpGet("Get")]
    // public async Task<ActionResult<WeatherForecast>> Get(DateTime date, [Required]string city)
    // {
    //
    //     var (lat, lon) = await _geocodingService.GetCoordinatesForCityAsync(city);
    //     try
    //     {
    //         var weatherData = _weatherDataProvider.GetCurrent(lat, lon);
    //         return Ok(_jsonProcessor.Process(weatherData));
    //     }
    //     catch (Exception e)
    //     {
    //         _logger.LogError(e, "Error getting weather data");
    //         return NotFound("Error getting weather data");
    //     }
    // }
    [HttpGet("Get")]
    public IEnumerable<WeatherForecast> Get()
    {
        _logger.LogInformation("Long running process started");
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
            })
            .ToArray();
    }

    [HttpGet("GetAsync")]
    public async Task<IEnumerable<WeatherForecast>> GetAsync()
    {
        _logger.LogInformation("Long running process started");
        await Task.Delay(60000);
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
            })
            .ToArray();
    }

    // [HttpGet("GetCurrent")]
    // public async Task<ActionResult<WeatherForecast>> GetCurrent()
    // {
    //     var lat = 47.497913;
    //     var lon = 19.040236;
    //
    //     try
    //     {
    //         var weatherData = await _weatherDataProvider.GetCurrentAsync(lat, lon);
    //         return Ok(_jsonProcessor.Process(weatherData));
    //     }
    //     catch (Exception e)
    //     {
    //         _logger.LogError(e, "Error getting weather data");
    //         return NotFound("Error getting weather data");
    //     }
    // }
    //

    [HttpGet("sunrise-sunset")]
    public async Task<ActionResult> GetSunriseSunset(string city, DateTime date)
    {
        try
        {
            var (lat, lon) = await _geocodingService.GetCoordinatesForCityAsync(city);


            var (sunriseTime, sunsetTime) = await _sunriseSunsetService.GetSunriseSunsetAsync(lat, lon, date);


            return Ok(new { Sunrise = sunriseTime, Sunset = sunsetTime });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting weather data");
            return StatusCode(500, "An unexpected error occurred");
        }
    }

    // [HttpGet("GetCurrent")]
    // public async Task<ActionResult<WeatherForecast>> GetCurrent(string cityName)
    // {
    //     var city = await _cityRepository.GetByNameAsync(cityName);
    //     if (city == null)
    //     {
    //         return NotFound($"City {cityName} not found");
    //     }
    //
    //     try
    //     {
    //         var weatherData = await _weatherDataProvider.GetCurrentAsync(city.Longitude,city.Longitude);
    //         return Ok(_jsonProcessor.Process(weatherData));
    //     }
    //     catch (Exception e)
    //     {
    //         _logger.LogError(e, "Error getting weather data");
    //         return NotFound("Error getting weather data");
    //     }
    // }
    [HttpGet("GetCurrent")]
    public async Task<ActionResult<WeatherForecast>> GetCurrent(string cityName)
    {
        var city = await _cityRepository.GetByNameAsync(cityName);

        if (city == null)
        {
            // City not found in the database, fetch its coordinates from the Geocoding API
            var (latitude, longitude) = await _geocodingService.GetCoordinatesForCityAsync(cityName);

            // Save the new city information in the database
            city = new City
            {
                Name = cityName,
                Latitude = latitude,
                Longitude = longitude
            };

            await _cityRepository.AddAsync(city);
        }

        try
        {
          
           
            var weatherData = await _weatherDataProvider.GetCurrentAsync(city.Latitude, city.Longitude);
            var processedWeatherData = _jsonProcessor.Process(weatherData);

            // Fetch or add SunriseSunset data for the city
            var sunriseSunset = await _sunriseSunsetRepository.GetByCityNameAsync(city.Name);
            if (sunriseSunset == null)
            {
                
                sunriseSunset = new SunriseSunset
                {
                    Sunrise = processedWeatherData.SunriseTime,
                    Sunset = processedWeatherData.SunsetTime,
                    CityId = city.Id
                };
                await _sunriseSunsetRepository.AddAsync(sunriseSunset);
            }
            return Ok(new { WeatherData = processedWeatherData, SunriseSunset = sunriseSunset });
        
            
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting weather data");
            return NotFound("Error getting weather data");
        }
    }
}