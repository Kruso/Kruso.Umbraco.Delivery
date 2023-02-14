using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery
{
    public enum RequestType
    {
        Initialized,
        Content,
        PreviewContent,
        Failed
    }

    public enum RequestOrigin
    {
        Backend,
        Frontend,
        Indexer
    }

    public enum TemplateType
    {
        Page,
        Block,
        Route,
        Search,
        Ref,
        Property
    }

    public enum ContentWrapperType
    {
        PublishedContent,
        Content,
        Media,
        PublishedElement
    }

    public enum EventType
    {
        Published,
        Saved,
        Deleted
    }
}
