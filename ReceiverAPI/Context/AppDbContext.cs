using Microsoft.EntityFrameworkCore;
using ReceiverAPI.Entity;

namespace ReceiverAPI.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<WeatherForecast> Forecast { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<WeatherForecast>()
                .HasKey(x => x.Id);

           /*builder.Entity<WeatherForecast>()
                .HasData(
                new WeatherForecast()
                {
                    Id = Guid.NewGuid(),
                    Date = DateTime.Now.AddDays(1),
                    TemperatureC = 32,
                    TemperatureF = 64,
                    Summary = "Summer"
                },
                new WeatherForecast()
                {
                    Id = Guid.NewGuid(),
                    Date = DateTime.Now.AddDays(1),
                    TemperatureC = -20,
                    TemperatureF = 0,
                    Summary = "Winter"
                },
                new WeatherForecast()
                {
                    Id = Guid.NewGuid(),
                    Date = DateTime.Now.AddDays(1),
                    TemperatureC = 15,
                    TemperatureF = 32,
                    Summary = "Spring"
                });*/
        }
    }
}
