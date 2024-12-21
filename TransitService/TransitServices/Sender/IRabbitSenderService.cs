namespace TransitService.TransitServices.Sender
{
    public interface IRabbitSenderService
    {
        void SendMessage(string message);
    }
}