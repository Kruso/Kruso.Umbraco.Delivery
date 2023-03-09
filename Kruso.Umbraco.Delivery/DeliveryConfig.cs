
using Kruso.Umbraco.Delivery.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;

namespace Kruso.Umbraco.Delivery
{
    public class DeliveryConfig : DeliveryConfigValues
    {
        public DeliveryConfigBase[] Sites { get; set; }

        public DeliveryConfigValues GetSite(Uri callingUri = null)
        {
            var siteValues = !IsMultiSite()
                ? Sites?.FirstOrDefault()
                : Sites.FirstOrDefault(s => IsMatchingAuthority(FrontendHost, callingUri));

            return GetSiteValues(siteValues);
        }

        public DeliveryConfigValues[] GetAllSites()
        {
            var configValues = new List<DeliveryConfigValues>();
            if (!IsMultiSite())
                configValues.Add(GetSiteValues(Sites?.FirstOrDefault()));
            else
            {
                foreach (var site in Sites)
                {
                    var siteValues = GetSiteValues(site);
                    configValues.Add(siteValues);
                }
            }

            return configValues.ToArray();
        }

        public bool IsMultiSite() => (Sites?.Length ?? 0) > 1;

        private DeliveryConfigValues GetSiteValues(DeliveryConfigBase siteValues)
        {
            return new DeliveryConfigValues
            {
                FrontendHost = GetValue(siteValues?.FrontendHost, FrontendHost),
                NotFoundType = GetValue(siteValues?.NotFoundType, NotFoundType),
                SettingsType = GetValue(siteValues?.SettingsType, SettingsType),
                RobotsTxtProperty = GetValue(siteValues?.RobotsTxtProperty, RobotsTxtProperty),
                MediaCdnUrl = GetValue(siteValues?.MediaCdnUrl, MediaCdnUrl),
                CacheControls = GetCacheControls(siteValues?.CacheControls, CacheControls),
                Webhooks = GetWebhooks(siteValues?.Webhooks, Webhooks),
                ForwardedHeader = ForwardedHeader,
                CertificateThumbprint = CertificateThumbprint,
                CertificateFileName = CertificateFileName,
                CertificateResourceName = CertificateResourceName,
                CertificatePassword = CertificatePassword,
            };
        }

        private string GetValue(string siteValue, string defaultValue)
        {
            return string.IsNullOrEmpty(siteValue)
                ? defaultValue
                : siteValue;
        }

        private DeliveryCacheControl[] GetCacheControls(DeliveryCacheControl[] siteValue, DeliveryCacheControl[] defaultValue)
        {
            var res = new List<DeliveryCacheControl>();
            if (siteValue != null)
                res.AddRange(siteValue);
            
            if (defaultValue != null)
            {
                foreach (var cacheControl in defaultValue)
                {
                    if (!res.Any(x => x.MimeType.Equals(cacheControl.MimeType, StringComparison.InvariantCultureIgnoreCase)))
                        res.Add(cacheControl);
                }
            }

            return res.ToArray();
        }

        private DeliveryWebhook[] GetWebhooks(DeliveryWebhook[] siteValue, DeliveryWebhook[] defaultValue)
        {
            var res = new List<DeliveryWebhook>();
            if (siteValue != null)
                res.AddRange(siteValue);

            if (defaultValue != null)
            {
                foreach (var webhook in defaultValue)
                {
                    if (!res.Any(x => x.Url.Equals(webhook.Url, StringComparison.InvariantCultureIgnoreCase)))
                        res.Add(webhook);
                }
            }

            return res.ToArray();
        }

        private bool IsMatchingAuthority(string frontendHost, Uri callingUri)
        {
            var frontendUri = frontendHost.AbsoluteUri();
            if (callingUri == null || frontendUri == null)
                return false;

            return callingUri.Authority == callingUri.Authority;
        }
    }

    public class DeliveryConfigValues : DeliveryConfigBase
    {
        //Config values that are common for all sites.
        public string ForwardedHeader { get; set; }
        public string CertificateThumbprint { get; set; }
        public string CertificateFileName { get; set; }
        public string CertificateResourceName { get; set; }
        public string CertificatePassword { get; set; }
    }

    public class DeliveryConfigBase
    {
        /// These values can be different per site
        public string NotFoundType { get; set; }
        public string SettingsType { get; set; }
        public string FrontendHost { get; set; }
        public string RobotsTxtProperty { get; set; }
        public string MediaCdnUrl { get; set; }
        public DeliveryCacheControl[] CacheControls { get; set; } = new DeliveryCacheControl[0];
        public DeliveryWebhook[] Webhooks { get; set; } = new DeliveryWebhook[0];

        public string GetCacheControl(string mimeType)
        {
            return CacheControls
                .FirstOrDefault(x => x.MimeType.Equals(mimeType, StringComparison.InvariantCultureIgnoreCase))
                ?.CacheControl;
        }
    }

    public class DeliveryCacheControl
    {
        public string MimeType { get; set; }
        public string CacheControl { get; set; }
    }

    public class DeliveryWebhook
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public DeliveryWebhookHeader[] Headers { get; set; } = new DeliveryWebhookHeader[0];
    }

    public class DeliveryWebhookHeader
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}