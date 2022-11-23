using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Services
{
    public interface IDeliTemplates
    {
        IPublishedContent AssignJsonTemplate(IPublishedContent content);
        ITemplate JsonTemplate();
        bool IsJsonTemplate(IPublishedContent content);
    }
}