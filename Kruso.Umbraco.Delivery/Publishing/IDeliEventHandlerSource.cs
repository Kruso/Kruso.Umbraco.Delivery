namespace Kruso.Umbraco.Delivery.Publishing
{
    public interface IDeliEventHandlerSource
    {
        IDeliEventHandler Get(EventType eventType, string[] documentTypes);
    }
}