using Kruso.Umbraco.Delivery.Models;
using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Security;
using Kruso.Umbraco.Delivery.Services;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Controllers.Renderers
{
    public class StringWriterUtf8 : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }

    public class SitemapRenderer
    {
        private readonly IDeliCulture _deliCulture;
        private readonly IUserIdentity _identity;
        private readonly IDeliUrl _deliUrl;
        private readonly IDeliRequest _deliRequest;
        private readonly IDeliContent _deliContent;
        private readonly IDeliPages _deliPages;
        private readonly IDeliProperties _deliProperties;

        public SitemapRenderer(IDeliCulture deliCulture, IUserIdentity identity, IDeliUrl deliUrl, IDeliRequestAccessor deliRequestAccessor, IDeliContent deliContent, IDeliPages deliPages, IDeliProperties deliProperties)
        {
            _deliCulture = deliCulture;
            _identity = identity;
            _deliUrl = deliUrl;
            _deliRequest = deliRequestAccessor.Current;
            _deliContent = deliContent;
            _deliPages = deliPages;   
            _deliProperties = deliProperties;
        }

        public string Create(string culture)
        {
            var sitemap = new Sitemap();
            culture ??= _deliRequest.Culture ?? _deliCulture.DefaultCulture;
            _deliCulture.WithCultureContext(culture, () =>
            {
                var startPage = _deliPages.StartPage(culture);
                foreach (var content in _deliContent.PublishedDescendants(startPage))
                {
                    AddContentToSitemap(sitemap, content);
                }
            });

            return SerializeSitemap(sitemap);
        }

        private void AddContentToSitemap(Sitemap sitemap, IPublishedContent content)
        {
            if (_identity.HasAccess(content))
            {
                var allowedCultures = _deliCulture.GetCultures(content);

                var urlsByCulture = allowedCultures
                      .Where(c => _deliCulture.IsPublishedInCulture(content, c) && !ExcludeFromSitemap(content, c))
                      .Select(c => new SitemapAlternateUrl
                      {
                          href = _deliUrl.GetAbsoluteDeliveryUrl(content, c),
                          hreflang = c
                      });

                foreach (var altUrl in urlsByCulture.Where(x => !string.IsNullOrEmpty(x.href)))
                {
                    var url = new SitemapUrl
                    {
                        loc = altUrl.href,
                        lastmod = content.UpdateDate,
                        links = urlsByCulture.Where(x => x.hreflang != altUrl.hreflang).ToList()
                    };

                    sitemap.urlset.Add(url);
                }
            }
        }

        private string SerializeSitemap(Sitemap sitemap)
        {
            var sitemapXml = string.Empty;

            var xmlSerializer = new XmlSerializer(sitemap.GetType());
            using (StringWriter textWriter = new StringWriterUtf8())
            {
                xmlSerializer.Serialize(textWriter, sitemap);
                sitemapXml = textWriter.ToString();
            }

            return sitemapXml;
        }

        private bool ExcludeFromSitemap(IPublishedContent content, string culture)
        {
            if (content == null)
                return true;

            var val = _deliProperties.Value(content, "excludeFromSitemap", culture);
            return val is bool && (bool)val;
        }

    }
}
