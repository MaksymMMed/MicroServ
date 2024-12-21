
namespace TransitService.TransitServices.Receiver
{
    public interface IRabbitReceiverService
    {
        Task ReceiveMessage<T>(string queueName, Func<T, Task> handleMessage);
    }
}