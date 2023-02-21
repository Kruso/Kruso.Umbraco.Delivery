using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Routing.Implementation;
using Kruso.Umbraco.Delivery.Services;
using Kruso.Umbraco.Delivery.Services.Implementation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Web;
using static Umbraco.Cms.Core.Constants.Conventions;

namespace Kruso.Umbraco.Delivery.Publishing
{
    public abstract class DeliNotificationHandler
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly DeliRequestAccessor _deliRequestAccessor;
        private readonly IDeliEventHandlerSource _deliEventHandlerSource;
        private readonly IDeliCulture _deliCulture;
        private readonly IDeliUrl _deliUrl;

        protected DeliNotificationHandler(IServiceProvider serviceProvider)
        {
            _umbracoContextFactory = serviceProvider.GetService<IUmbracoContextFactory>();
            _deliRequestAccessor = serviceProvider.GetService<IDeliRequestAccessor>() as DeliRequestAccessor;
            _deliEventHandlerSource = serviceProvider.GetService<IDeliEventHandlerSource>();
            _deliCulture = serviceProvider.GetService<IDeliCulture>();
            _deliUrl = serviceProvider.GetService<IDeliUrl>();
        }

        protected bool Handle(IEnumerable<IContent> entities, EventType eventType, Func<IContent, string, bool> hasCultureFunc)
        {
            var cancel = false;

            using (UmbracoContextReference context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                var contentByCulture = GetContentByCulture(context, entities, _deliCulture.SupportedCultures, hasCultureFunc);
                foreach (var culture in contentByCulture.Keys)
                {
                    _deliCulture.WithCultureContext(culture, () =>
                    {
                        foreach (var content in contentByCulture[culture])
                        {
                            _deliRequestAccessor.InitializeIndexing(content, culture, _deliUrl.GetFrontendHostUri(content, culture));
                            cancel |= _deliEventHandlerSource.Get(eventType, culture)?.Handle(eventType, culture, content) ?? false;
                        }
                    });
                }
            }

            return cancel;
        }

        private static Dictionary<string, List<IPublishedContent>> GetContentByCulture(UmbracoContextReference context, IEnumerable<IContent> entities, string[] cultures, Func<IContent, string, bool> hasCultureFunc)
        {
            var res = new Dictionary<string, List<IPublishedContent>>();

            foreach (var content in entities)
            {
                var publishedCultures = cultures
                    .Where(x => hasCultureFunc(content, x))
                    .ToArray();

                foreach (var publishedCulture in publishedCultures)
                {
                    var publishedContent = context.UmbracoContext.Content.GetById(false, content.Id);
                    if (publishedContent != null)
                    {
                        if (!res.ContainsKey(publishedCulture))
                            res.Add(publishedCulture, new List<IPublishedContent>());


                        if (publishedContent != null)
                            res[publishedCulture].Add(publishedContent);
                    }
                }
            }

            return res;
        }
    }
}
