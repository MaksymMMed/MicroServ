using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using ReceiverAPI;
using ReceiverAPI.Context;
using ReceiverAPI.Entity;
using ReceiverAPI.Services;
using ReceiverAPI.Subscribers;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
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
    string keyVaultName = Environment.GetEnvironmentVariable("KEY_VAULT_NAME")!;
    string tenantId = Environment.GetEnvironmentVariable("TENANT_ID")!;
    string clientId = Environment.GetEnvironmentVariable("CLIENT_ID")!;
    string clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET")!;


    var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);

    var certificateClient = new SecretClient(new Uri($"https://{keyVaultName}.vault.azure.net/"), credential:credential);
    var certificate = certificateClient.GetSecret("BaseCert").Value;

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ConfigureHttpsDefaults(httpsOptions =>
        {
            var privateKeyBytes = Convert.FromBase64String(certificate.Value);
            httpsOptions.ServerCertificate = new X509Certificate2(privateKeyBytes);
        });
    });

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
