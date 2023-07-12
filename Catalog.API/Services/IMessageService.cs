namespace Catalog.API.Services
{
    public interface IMessageService
    {
        void PublishMessage(string routingKey, object message);
        string ConsumeMessage();
    }
}
