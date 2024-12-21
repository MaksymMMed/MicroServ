using ReceiverAPI.Dto;
using ReceiverAPI.Services;
using System.Diagnostics;
using TransitService.TransitServices.Receiver;

namespace ReceiverAPI.Subscribers
{
    public class RabbitSubscriber : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public RabbitSubscriber(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await AddForecast(stoppingToken);
        }

        private async Task AddForecast(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var rabbitReceiverService = scope.ServiceProvider.GetRequiredService<IRabbitReceiverService>();
                await rabbitReceiverService.ReceiveMessage<CreateForecastDto>(
                    "forecast",
                    async (dto) =>
                    {
                        using (var processingScope = _serviceProvider.CreateScope())
                        {
                            var weatherService = processingScope.ServiceProvider.GetRequiredService<IWeatherService>();
                            await weatherService.CreateForecast(dto);
                        }
                    }
                );
            }
        }
    }
}
