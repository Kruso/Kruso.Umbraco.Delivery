using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Webhooks
{
    internal interface IDeliWebhookService
    {
        Task SendAsync(List<DeliWebhookNotification> notifications);
    }
}