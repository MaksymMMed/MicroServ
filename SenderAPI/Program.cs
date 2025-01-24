using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using RabbitMQ.Client;
using System.Security.Cryptography.X509Certificates;
using TransitService.TransitServices.Sender;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IRabbitSenderService, RabbitSenderService>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton(sg => new ConnectionFactory
    {
        HostName = "rabbitmq"
    });
}
else
{
    string keyVaultName = Environment.GetEnvironmentVariable("KEY_VAULT_NAME")!;
    var certificateClient = new CertificateClient(new Uri($"https://{keyVaultName}.vault.azure.net/"), new DefaultAzureCredential());
    var certificate = certificateClient.GetCertificate("BaseCert").Value;

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ConfigureHttpsDefaults(httpsOptions =>
        {
            httpsOptions.ServerCertificate = new X509Certificate2(certificate.Cer);
        });
    });

    builder.Services.AddSingleton(sg => new ConnectionFactory
    {
        Uri = new Uri(Environment.GetEnvironmentVariable("SERVICE_BUS_STRING")!)
    });
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
