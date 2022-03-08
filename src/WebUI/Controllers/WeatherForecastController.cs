using Microsoft.AspNetCore.Mvc;
using ReceiptGenerator.Application.WeatherForecasts.Queries.GetWeatherForecasts;

namespace ReceiptGenerator.WebUI.Controllers
{
    public class WeatherForecastController : ApiControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            return await Mediator.Send(new GetWeatherForecastsQuery());
        }
    }
}