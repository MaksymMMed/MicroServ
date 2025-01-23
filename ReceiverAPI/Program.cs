using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using ReceiverAPI.Context;
using ReceiverAPI.Entity;
using ReceiverAPI.Services;
using ReceiverAPI.Subscribers;
using TransitService.TransitServices.Receiver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton(sg => new ConnectionFactory
    {
        HostName = "rabbitmq",

    });

    builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}
else
{
    builder.Services.AddSingleton(sg => new ConnectionFactory
    {
        Uri = new Uri(Environment.GetEnvironmentVariable("SERVICE_BUS_STRING")!)

    });

    builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")));
}

builder.Services.AddScoped<IRabbitReceiverService, RabbitReceiverService>();
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddHostedService<RabbitSubscriber>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var databaseExists = await dbContext.Database.CanConnectAsync();

        if (!databaseExists)
        {
            await dbContext.Database.MigrateAsync();
            dbContext.Forecast.Add(new WeatherForecast { Id = Guid.NewGuid(),TemperatureC = 32,TemperatureF = 64,Summary ="Summer" });
            dbContext.Forecast.Add(new WeatherForecast { Id = Guid.NewGuid(),TemperatureC = -20,TemperatureF = 0,Summary ="Winter" });
            dbContext.Forecast.Add(new WeatherForecast { Id = Guid.NewGuid(),TemperatureC = 15,TemperatureF = 32,Summary ="Spring" });
            dbContext.SaveChanges();
        }
    }
}
else
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var databaseExists = await dbContext.Database.CanConnectAsync();

        if (!databaseExists)
        {
            await dbContext.Database.MigrateAsync();
        }
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
