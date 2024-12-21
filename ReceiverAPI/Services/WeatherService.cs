using Microsoft.EntityFrameworkCore;
using ReceiverAPI.Context;
using ReceiverAPI.Dto;
using ReceiverAPI.Entity;

namespace ReceiverAPI.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly AppDbContext db;
        private readonly DbSet<WeatherForecast> table;

        public WeatherService(AppDbContext db)
        {
            this.db = db;
            table = this.db.Set<WeatherForecast>();
        }

        public async Task CreateForecast(CreateForecastDto dto)
        {
            try
            {
                var item = new WeatherForecast
                {
                    Date = dto.Date,
                    Summary = dto.Summary,
                    TemperatureC = dto.TemperatureC,
                    TemperatureF = dto.TemperatureF
                };

                await table.AddAsync(item);
                await db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<GetForecastDto>> GetForecasts()
        {
            try
            {
                var items = await table.ToListAsync();
                List<GetForecastDto> mappedItems = new List<GetForecastDto>();
                foreach (WeatherForecast item in items)
                {
                    mappedItems.Add(new GetForecastDto()
                    {
                        Id = item.Id,
                        Date = item.Date,
                        Summary = item.Summary,
                        TemperatureC = item.TemperatureC,
                        TemperatureF = item.TemperatureF,
                    });
                }
                return mappedItems;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
