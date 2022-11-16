using System;
using System.Collections.Generic;
using System.Linq;

namespace Kruso.Umbraco.Delivery
{
    public class DeliveryConfig : DeliveryConfigValues
    {
        public DeliveryConfigBase[] Sites { get; set; }

        public DeliveryConfigValues GetConfigValues(string frontendHost)
        {
            var siteValues = Sites?.FirstOrDefault(s => s.FrontendHost == frontendHost);

            var values = new DeliveryConfigValues
            {
                FrontendHost = string.IsNullOrEmpty(siteValues?.FrontendHost) ? FrontendHost : siteValues.FrontendHost,
                NotFoundType = string.IsNullOrEmpty(siteValues?.NotFoundType) ? NotFoundType : siteValues.NotFoundType,
                SettingsType = string.IsNullOrEmpty(siteValues?.SettingsType) ? SettingsType : siteValues.SettingsType,
                RobotsTxtProperty = string.IsNullOrEmpty(siteValues?.RobotsTxtProperty) ? RobotsTxtProperty : siteValues.RobotsTxtProperty,
                MediaCdnUrl = string.IsNullOrEmpty(siteValues?.MediaCdnUrl) ? MediaCdnUrl : siteValues.MediaCdnUrl,
                ForwardedHeader = ForwardedHeader,
                CertificateThumbprint = CertificateThumbprint,
            };

            return values;
        }
    }

    public class DeliveryConfigValues : DeliveryConfigBase
    {
        //Config values that are common for all sites.
        public string ForwardedHeader { get; set; }
        public string CertificateThumbprint { get; set; }
    }

    public class DeliveryConfigBase
    {
        /// These values can be different per site
        public string NotFoundType { get; set; }
        public string SettingsType { get; set; }
        public string FrontendHost { get; set; }
        public string RobotsTxtProperty { get; set; }
        public string MediaCdnUrl { get; set; }
    }
}