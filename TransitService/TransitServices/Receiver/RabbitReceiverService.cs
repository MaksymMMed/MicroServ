using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace TransitService.TransitServices.Receiver
{
    public class RabbitReceiverService:IRabbitReceiverService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ConnectionFactory _factory;

        public RabbitReceiverService(ConnectionFactory factory)
        {
            _factory = factory;
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public Task ReceiveMessage<T>(string queueName, Func<T, Task> processMessage)
        {
            _channel.QueueDeclare(queue: queueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                try
                {
                    var deserializedMessage = JsonSerializer.Deserialize<T>(content);

                    if (deserializedMessage != null)
                    {
                        await processMessage(deserializedMessage);
                        Console.WriteLine($"Processed message: {content}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}
