using System;
using System.Linq;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class RandomWeatherService : IForecastService
    {
        private readonly string[] _summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public WeatherForecast[] GetForecast()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                DateFormatted = DateTime.Now.AddDays(index).ToString("d"),
                TemperatureC = rng.Next(-20, 55),
                Summary = _summaries[rng.Next(_summaries.Length)]
            }).ToArray();
        }

        public WeatherForecast GetCurrent()
        {
            var rng = new Random();
            return new WeatherForecast
            {
                DateFormatted = DateTime.Now.ToString("d"),
                TemperatureC = rng.Next(-20, 55),
                Summary = _summaries[rng.Next(_summaries.Length)]
            };
        }
    }
}
