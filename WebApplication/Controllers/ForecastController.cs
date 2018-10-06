using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    [Route("api")]
    public class ForecastController : ControllerBase
    {
        private readonly IForecastService _forecastService;

        public ForecastController(IForecastService forecastService)
        {
            _forecastService = forecastService;
        }

        [HttpGet("SampleData/WeatherForecasts")]
        public ActionResult<WeatherForecast[]> GetWeatherForecasts()
        {
            return _forecastService.GetForecast();
        }

        [HttpGet("Weather/Forecast")]
        public ActionResult<WeatherForecast[]> GetForecast()
        {
            return _forecastService.GetForecast();
        }

        [HttpGet("Weather/Today")]
        public ActionResult<WeatherForecast> GetToday()
        {
            return _forecastService.GetCurrent();
        }

        [Authorize]
        [HttpPut("Weather/Preferences")]
        public ActionResult<WeatherForecast> UpdatePreferences(
            [FromBody]string location,
            [FromServices] IPreferencesService preferences)
        {
            preferences.UpdateLocation(location);
            return NoContent();
        }
    }
}
