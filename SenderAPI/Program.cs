using RabbitMQ.Client;
using TransitService.TransitServices.Sender;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(sg => new ConnectionFactory
{
    HostName = "rabbitmq" 
});
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
