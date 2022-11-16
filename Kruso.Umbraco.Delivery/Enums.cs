﻿using System;
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
        PreviewPane,
        Failed
    }

    public enum TemplateType
    {
        Page,
        Block,
        Route,
        Search,
        Ref
    }

    public enum ContentWrapperType
    {
        PublishedContent,
        Content,
        Media,
        PublishedElement
    }
}