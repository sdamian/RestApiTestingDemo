using WebApplication.Models;

namespace WebApplication.Services
{
    public interface IForecastService
    {
        WeatherForecast[] GetForecast();
        WeatherForecast GetCurrent();
    }
}
