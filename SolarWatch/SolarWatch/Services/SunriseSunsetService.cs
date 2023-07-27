using System.Net;
using System.Text.Json;

namespace WeatherApi.Services
{
    public class SunriseSunsetService : ISunriseSunsetService
    {
        private readonly HttpClient _httpClient;

        public SunriseSunsetService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(TimeSpan sunrise, TimeSpan sunset)> GetSunriseSunsetAsync(double latitude, double longitude, DateTime date)
        {
            try
            {
                var url = $"https://api.sunrise-sunset.org/json?lat={latitude}&lng={longitude}&date={date:yyyy-MM-dd}";
                var client = new WebClient();
                var data = client.DownloadString(url);
                JsonDocument json = JsonDocument.Parse(data);

                // Extract sunrise and sunset times from the JSON response
                string sunriseTimeString = json.RootElement.GetProperty("results").GetProperty("sunrise").GetString();
                string sunsetTimeString = json.RootElement.GetProperty("results").GetProperty("sunset").GetString();

                TimeSpan sunriseTime = DateTime.Parse(sunriseTimeString).TimeOfDay;
                TimeSpan sunsetTime = DateTime.Parse(sunsetTimeString).TimeOfDay;
                
                return (sunriseTime, sunsetTime);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occurred during the API call
                // For example, log the error or throw a custom exception
                // You can define a custom exception class to wrap the exception for more meaningful error handling
                throw new Exception("Error while fetching sunrise/sunset data from the API.", ex);
            }
        }
      
    }
}