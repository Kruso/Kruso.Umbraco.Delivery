using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Publishing
{
    public interface IDeliEventHandler
    {
        void Handle(IPublishedContent publishedContent);
    }
}
