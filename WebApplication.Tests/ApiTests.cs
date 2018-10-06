using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Newtonsoft.Json.Linq;
using Shouldly;
using WebApplication.Models;
using WebApplication.Services;
using WebApplication.Tests.Authentication;
using Xunit;

namespace WebApplication.Tests
{
    public class ApiTests
    {
        private readonly HttpClient _client;
        private readonly Mock<IForecastService> _forecastMock = new Mock<IForecastService>();
        private readonly Mock<IPreferencesService> _preferencesMock = new Mock<IPreferencesService>();
        private bool _isAuthenticated;

        public ApiTests()
        {
            var hostBuilder = new WebHostBuilder()
                .UseSetting("AzureAd:Instance", "https://login.microsoftonline.com/")
                .UseSetting("AzureAd:Domain", "qualified.domain.name")
                .UseSetting("AzureAd:TenantId", "22222222-2222-2222-2222-222222222222")
                .UseSetting("AzureAd:ClientId", "11111111-1111-1111-11111111111111111")
                .ConfigureTestServices(services =>
                {
                    services
                        .AddTransient(_ => _forecastMock.Object)
                        .AddTransient(_ => _preferencesMock.Object);
                    services
                        .AddAuthentication("Test")
                        .AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                            "Test",
                            o => o.ShouldBeAuthenticated = _isAuthenticated);
                })
                .UseStartup<Startup>();

            var server = new TestServer(hostBuilder);
            _client = server.CreateClient();
            _forecastMock.Setup(x => x.GetCurrent()).Returns(new WeatherForecast
            {
                TemperatureC = 10
            });
        }

        [Fact]
        public async Task Get_WeatherForecast_ShouldReturnOk()
        {
            HttpResponseMessage response = await _client.GetAsync("api/SampleData/WeatherForecasts");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get_TodayForecast_ShouldReturnOk()
        {
            HttpResponseMessage response = await _client.GetAsync("api/Weather/Today");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get_TodayForecastResponse_ShouldHaveForecast()
        {
            HttpResponseMessage response = await _client.GetAsync("api/Weather/Today");
            var result = await response.Content.ReadAsAsync<WeatherForecast>();
            result.ShouldNotBeNull();
        }

        [Fact]
        public async Task Get_TodayForecastResponse_ShouldIncludeTemperature()
        {
            HttpResponseMessage response = await _client.GetAsync("api/Weather/Today");
            var result = await response.Content.ReadAsAsync<JObject>();
            result["temperatureC"].ShouldNotBeNull();
        }

        [Fact]
        public async Task Get_TodayForecastResponse_ReturnsCorrectForecast()
        {
            _forecastMock.Setup(x => x.GetCurrent()).Returns(new WeatherForecast
            {
                TemperatureC = 42
            });
            HttpResponseMessage response = await _client.GetAsync("api/Weather/Today");
            var result = await response.Content.ReadAsAsync<JObject>();
            result["temperatureC"].ShouldBe(42);
            result["temperatureF"].ShouldBe(107);
        }

        [Fact]
        public async Task PutPreferences_WhenNotAuthenticated_ReturnsUnauthorized()
        {
            _isAuthenticated = false;
            HttpResponseMessage response = await _client.PutAsJsonAsync("api/Weather/Preferences", "location");
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task PutPreferences_WhenAuthenticated_ReturnsNoContent()
        {
            _isAuthenticated = true;
            HttpResponseMessage response = await _client.PutAsJsonAsync("api/Weather/Preferences", "location");
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task PutPreferences_WhenAuthenticated_DoesSaveThePreferences()
        {
            _isAuthenticated = true;
            await _client.PutAsJsonAsync("api/Weather/Preferences", "new location");
            _preferencesMock.Verify(x => x.UpdateLocation("new location"));
        }
    }
}
