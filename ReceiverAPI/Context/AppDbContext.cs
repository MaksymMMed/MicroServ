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
        }
    }
}
