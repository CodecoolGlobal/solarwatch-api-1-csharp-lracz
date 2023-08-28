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
                
                string? sunriseTimeString = json.RootElement.GetProperty("results").GetProperty("sunrise").GetString();
                string? sunsetTimeString = json.RootElement.GetProperty("results").GetProperty("sunset").GetString();

                TimeSpan sunriseTime = DateTime.Parse(sunriseTimeString).TimeOfDay;
                TimeSpan sunsetTime = DateTime.Parse(sunsetTimeString).TimeOfDay;
                
                return (sunriseTime, sunsetTime);
            }
            catch (Exception ex)
            {
              
                throw new Exception("Error while fetching sunrise/sunset data from the API.", ex);
            }
        }
      
    }
}