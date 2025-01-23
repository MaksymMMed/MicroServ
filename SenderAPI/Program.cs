using RabbitMQ.Client;
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
    builder.Services.AddSingleton(sg => new ConnectionFactory
    {
        Uri = new Uri(Environment.GetEnvironmentVariable("RABBITMQ_URI")!)
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
