
using ReceiverAPI.Dto;

namespace ReceiverAPI.Services
{
    public interface IWeatherService
    {
        Task CreateForecast(CreateForecastDto dto);
        Task<List<GetForecastDto>> GetForecasts();
    }
}