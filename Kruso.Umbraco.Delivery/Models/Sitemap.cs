using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Kruso.Umbraco.Delivery.Models
{
    public enum Frequency
    {
        always,
        hourly,
        daily,
        weekly,
        monthly,
        yearly,
        never
    }


    [XmlRoot("urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class Sitemap
    {
        private XmlSerializerNamespaces xmlns;

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Xmlns
        {
            get
            {
                if (xmlns == null)
                {
                    xmlns = new XmlSerializerNamespaces();
                    xmlns.Add("", "http://www.sitemaps.org/schemas/sitemap/0.9");
                    xmlns.Add("xhtml", "http://www.w3.org/1999/xhtml");
                }
                return xmlns;
            }
            set { xmlns = value; }
        }

        [XmlElement("url")]
        public List<SitemapUrl> urlset { get; set; }

        public Sitemap()
        {
            urlset = new List<SitemapUrl>();
        }
    }

    public class SitemapUrl
    {
        public string loc { get; set; }
        public DateTime lastmod { get; set; }
        public Frequency changefreq { get; set; }
        public float priority { get; set; }

        [XmlElement(ElementName = "link", Namespace = "http://www.w3.org/1999/xhtml")]
        public List<SitemapAlternateUrl> links { get; set; }

        public SitemapUrl()
        {
            priority = 0.5f;
            changefreq = Frequency.monthly;
            links = new List<SitemapAlternateUrl>();
        }
    }
        
    public class SitemapAlternateUrl
    {
        [XmlAttribute("rel")]
        public string rel { get; set; }
        [XmlAttribute("href")]
        public string href { get; set; }
        [XmlAttribute("hreflang")]
        public string hreflang { get; set; }

        public SitemapAlternateUrl()
        {
            rel = "alternate";
        }
    }
}
