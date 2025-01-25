using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
using RabbitMQ.Client;
using SenderAPI;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using TransitService.TransitServices.Sender;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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
    string tenantId = Environment.GetEnvironmentVariable("TENANT_ID")!;
    string clientId = Environment.GetEnvironmentVariable("CLIENT_ID")!;
    string clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET")!;


    var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);

    var certificateClient = new SecretClient(new Uri($"https://{keyVaultName}.vault.azure.net/"), credential: credential);
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
}

builder.Services.AddSingleton<IRabbitSenderService, RabbitSenderService>();

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
