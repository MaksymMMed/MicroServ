using RabbitMQ.Client;
using System.Text;

namespace TransitService.TransitServices.Sender
{
    public class RabbitSenderService : IRabbitSenderService
    {
        private readonly IConnection _connection;
        private readonly ConnectionFactory _factory;

        public RabbitSenderService(ConnectionFactory factory)
        {
            _factory = factory;
            _connection = _factory.CreateConnection();
            InitializeQueue();
        }

        private void InitializeQueue()
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queue: "forecast",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            }
        }

        public void SendMessage(string message)
        {
            using (var channel = _connection.CreateModel())
            {
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "forecast",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine($"Send message {message}");
            }
        }
    }
}
