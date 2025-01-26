using Microsoft.AspNetCore.Mvc;
using ReceiverAPI.Dto;
using ReceiverAPI.Services;

namespace ReceiverAPI.Controllers
{
    [Route("receiver/[controller]")]
    [ApiController]
    public class ForecastController : ControllerBase
    {
        private readonly IWeatherService _service;

        public ForecastController(IWeatherService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetForecastDto>>> GetForecasts()
        {
            try
            {
                var items = await _service.GetForecasts();
                return Ok(items);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
