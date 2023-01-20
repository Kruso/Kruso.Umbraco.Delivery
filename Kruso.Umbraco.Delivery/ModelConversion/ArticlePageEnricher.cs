using Kruso.Umbraco.Delivery.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.ModelConversion
{
    internal class ArticlePageEnricher : Enricher
    {
        public override void Enrich()
        {
            Add("title", typeof(string), (node) => "something");
            Add("teaser", typeof(string), GetTeaser);
            Add("hero", "HeroBlock", Guid.NewGuid());
            Remove("seo");
        }

        private string GetTeaser(JsonNode node)
        {
            return "bananas";
        }

        private JsonNode CreateHeroBlock(JsonNode node, JsonNode block)
        {
            block
                .AddProp("prop1", "test")
                .AddProp("prop2", "text");

            return block;
        }
    }
}

