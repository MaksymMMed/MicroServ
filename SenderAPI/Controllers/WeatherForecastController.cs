using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using SenderAPI.Dto;
using TransitService.TransitServices.Sender;

namespace SenderAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IRabbitSenderService _sender;

        public WeatherForecastController(IRabbitSenderService service)
        {
            _sender = service;
        }

        [HttpGet]
        public WeatherForecastDto Get()
        {
            int number = new Random().Next(-20, 55);
            var forecast = new WeatherForecastDto
            {
                Date = DateTime.Now.AddDays(1),
                TemperatureC = number,
                TemperatureF = 32 + (int)(number / 0.5556),
                Summary = Summaries[new Random().Next(Summaries.Length)]
            };

            _sender.SendMessage(JsonSerializer.Serialize(forecast));

            return forecast;
        }
    }
}
