using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SolarWatch;
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

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherDataProvider weatherDataProvider, IJsonProcessor jsonProcessor,IGeocodingService geocodingService,ISunriseSunsetService sunriseSunsetService)
    {
        _logger = logger;
        _weatherDataProvider = weatherDataProvider;
        _jsonProcessor = jsonProcessor;
         _sunriseSunsetService = sunriseSunsetService;
         _geocodingService = geocodingService;
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
   
    [HttpGet("GetCurrent")]
    public async Task<ActionResult<WeatherForecast>> GetCurrent()
    {
        var lat = 47.497913;
        var lon = 19.040236;

        try
        {
            var weatherData = await _weatherDataProvider.GetCurrentAsync(lat, lon);
            return Ok(_jsonProcessor.Process(weatherData));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting weather data");
            return NotFound("Error getting weather data");
        }
    }
    
 
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




}