using Newtonsoft.Json;

namespace Kruso.Umbraco.Delivery.Models
{
    public class Settings
    {
        public string NotFoundType { get; set; }
        public string[] FolderTypes { get; set; }
        public string[] IgnoreTypes { get; set; }
        public string SiteSettingsType { get; set; }
        public bool EnableCaching { get; set; }
        public bool EnableAcceptLanguageRouting { get; set; }
        public bool KeepEmptyProperties { get; set; }
        public string DefaultLanguage { get; set; }
        [JsonProperty("frontEndSites")] 
        public DeliverySite[] Sites { get; set; }
        public string RobotsTxt { get; set; }

        public Settings()
        {
            NotFoundType = "notFoundPage";
            FolderTypes = new[] { 
                "blockFolder", 
                "contentPages",
                "articleCategories",
                "productCategories",
                "global"
            };
            IgnoreTypes = new [] {
                "footer",
                "header",
                "basketPage",
                "checkOutPage",
                "orderConfirmationPage"
            };
            SiteSettingsType = "siteSettings";
            DefaultLanguage = "en-US";
            Sites = new DeliverySite[0];
            RobotsTxt = "siteSettings.RobotsTxt";
        }
    }

    public class DeliverySite
    {
        public string Culture { get; set; }
        public string Url { get; set; }
    }
}
