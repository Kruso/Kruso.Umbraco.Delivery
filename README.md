# Umbraco Delivery API

* [Umbraco Delivery API](#umbraco-delivery-api)
    * [Introduction to Umbraco.Delivery](#introduction-to-umbracodelivery)
        * [Main features](#main-features)
        * [Demo site](#demo-site)
        * [History](#history)
    * [Installation & setup](#installation--setup)
        * [Nuget](#nuget)
        * [Application and service configuration](#application-and-service-configuration)
            * [Configuration tip](#configuration-tip)
        * [Add template](#add-template)
    * [Using Umbraco.Delivery](#using-umbracodelivery)
        * [Json Template](#json-template)
        * [Get content](#get-content)
            * [Get children \[TODO\]](#get-children-todo)
        * [Manifest](#manifest)
        * [Sitemap](#sitemap)
            * [Exclude items from Sitemap](#exclude-items-from-sitemap)
        * [Translations](#translations)
        * [Robots.txt](#robotstxt)
        * [Search](#search)
    * [Configuration](#configuration)
    - [**Site Settings Type**](#site-settings-type)
    - [Not Found Type \[TODO\]](#not-found-type-todo)
    - [Robots Txt](#robots-txt)
    - [Keep Empty Properties TODO](#keep-empty-properties-todo)
    * [Search](#search-1)
        * [Implementing ISearchQuery Classes \[TODO\]](#implementing-isearchquery-classes-todo)
    * [Page and property converters - TODO Needs to be verified](#page-and-property-converters---todo-needs-to-be-verified)
        * [6.1 Pages and Blocks](#61-pages-and-blocks)
        * [6.2 Converters](#62-converters)
            * [6.2.1 Page Converters](#621-page-converters)
            * [6.2.2 Block Converters](#622-block-converters)
            * [6.2.3 Navigation Converter TODO](#623-navigation-converter-todo)
        * [6.3 Property Value Factories](#63-property-value-factories)
        * [6.4 Templates](#64-templates)
        * [6.5 The ModelNode Class](#65-the-modelnode-class)
        * [6.6 Dependency Injection](#66-dependency-injection)
        * [6.7 Manually Converting IPublishedContent to JSON](#67-manually-converting-ipublishedcontent-to-json)
            * [6.7.1 The ModelFactory Class](#671-the-modelfactory-class)
            * [6.7.2 The ModelConverter Class](#672-the-modelconverter-class)
        * [6.8 The IModelFactoryContext Object](#68-the-imodelfactorycontext-object)

## Introduction to Umbraco.Delivery

This package will add a REST-like content delivery API to Umbraco CMS. It provides a number of API endpoints to access content, search, and other functions.

### Main features

* Access [content](#content) in various ways. There are several ways to tweak the outputted json and how deep content structure that will be returned.
* Get an [XML sitemap](#sitemap).
* Return robots.txt content
* Return all Translations set up in Umbraco. [TODO: is this still valid?]
* Search among content, and the possibility to tweak how your content gets indexed.
* Supports multiple sites (in multiple languages) without the need for addtional host names for your Umbraco instance.
* Supports "hybrid mode". A page can be returned as a traditional page, or as Json.
* Get information about sites and languages, and get all available routes for all documents.

As far as possible Umbraco.Delivery will work out of the box without further configuration, even for a multi-site and multi-language setup.

[TODO redirects]

### Demo site

[TODO link to demo sitge]
<br>
### History

This package is mainly the work of @EdSoanes. It has been used in several solutions during several years, from Umbraco v8 to the latest v10. The version for Umbraco v10 has several additons and changes, but the core functionality is well battle tested.
It was made public as Kruso.Umbraco.Delivery because we at [Kruso](https://kruso.dk/en/) love open source, and want to contribute to the community. It also keeps us on our toes when we know others will look at our code 😉.
<br>
## Installation & setup

### Nuget

Install Kruso.Umbraco.Delivery [TODO link]

### Application and service configuration

1\. Add ```AddUmbracoDeliveryServices``` and ```AddUmbracoDelivery``` In the ```ConfigureServices``` method in Startup.cs. It should look something like this:

```
services.AddUmbracoDeliveryServices(_config);

services.AddUmbraco(_env, _config)
    .AddBackOffice()
    .AddWebsite()
    .AddComposers()
    .AddUmbracoDelivery()
    .Build();
```

2\. Add ```app.UseUmbracoDelivery();``` in the ```Configure``` method in the same file.
Note that it must be added before ```app.UseUmbraco()```.

#### Configuration tip

You may want to add compression to the returned Json. It will reduce size significantly.
Read more [here](https://learn.microsoft.com/en-us/aspnet/core/performance/response-compression?view=aspnetcore-6.0), and see how it can be set up in the complete example.
[TODO link to gist with complete example]
<br>
### Add template

Pages that should be returned as Json must have a Template called "Json" assigned.

Now you can get page content as Json! Like this [LINK to demo].

## Using Umbraco.Delivery

Build your document types, data types, and site structure as normal in Umbraco. Umbraco.Delivery respects how Umbraco is designed to work.
This package makes a clear distinction between Pages and Blocks, and depending on whether content in the content tree is considered a Page or Block, different rendering options are available.

* **Pages**. Documents within the content tree that have a Template assigned are considered Pages. This means that the document will have an external route (e.g. `/products/my-product`).
* **Blocks**. Documents that do not have a template, or are of type Element in Umbraco are considered blocks. They are a part of a page, do not have an external route and cannot be requested directly. Note that a document that meets the criteria to be a Page can also be considered a Block if it is referenced on a Page as part of its content.

### Json Template

Normally when creating a Document Type in Umbraco, you can choose to create one with or without a Template. Umbraco.Delivery requires a Template to be assigned to any Document Type that will be considered a Page (rather than a Block). It does not, however, require a different Template for each Document Type. You can create a single Template called "Json", and assign it as template to any Document Type that is intended to be a Page.
<br>
### Get content

[TODO] Depth parameter
[TODO] Forwarded header
[TODO] Include/exclude

If you know a Page's guid and culture then you can fetch the JSON for that page using the following endpoint:

```
/api/{culture}/content/{page id}
```

Example: `/api/en/content/a36277ff-a60b-413a-a3a8-08d0ebe0fb60`

[TODO: this doesn't seem to work] You can also retrieve a Page from the same endpoint by it's route/path:

```
/api/content?path={route}
```

Note that you don't need to specify a culture in this case. The route is always unique across pages and cultures.

You can also retrieve a page's Json by calling the route directly, just as if it were a traditional Umbraco CMS page:

```
https://my.site.com/{route}
```

##### Get children [TODO]
<br>
### Manifest

```
/api/manifest
```

This endpoint is responsible for providing Site configuration data, providing a complete description of every site. The following information is included:

* Culture Information
* Start Page Information
* All page Routes per site

You can optionally select parts of the entire manifest by using a features query string. For example, `/api/manifest?features=startpage` would return only startpage information. `/api/manifest?features=startpage,routes,cultureinfo`, or leaving out the features param returns all info.

### Sitemap

The sitemap for the entire site can be fetched as an XML document on the endpoint `/api/sitemap` or `/api/{culture}/sitemap`
If culture is not included in the path the sitemap will contain the site structure for all languages.
Note that the "Forwarded header" needs to be set in the request to generate a correct sitemap.

##### Exclude items from Sitemap

If a property called `excludeFromSitemap` is created on a Document type as a true/false field, then this document will be excluded from the sitemap.

### Translations

All translations (dictionary items) entered into the CMS can be accessed as a single Jsob array on the endpoint `/umbraco/api/data/translations`. All translations in all languages are returned.

### Robots.txt

If a robots.txt property is configured it can be retrieved in plain text on the endpoint `/api/{culture}/robots/`

### Search
<br>
[TODO]
<br>
### Preview

[TODO]

## Configuration

In most cases Umbraco.Delivery should work out of the box without further configuration. You may need to configure certain Document Types in the `appsettings.json` file so certain functionality will work. For example, identifying which Document Type represents the 404 page.
<br>
```
"UmbracoDelivery": {
    "SettingsType": "settings",
    "NotFoundType": "notFoundPage",
    "RobotsTxtProperty": "settings.robotsTxt",
    "ForwardedHeader": "X-Headless-Origin",
    "FrontEndHost": "www.myhostname.com",
    "CertificateThumbprint": ""
  }
```
<br>
##### **Site Settings Type**

It is common for sites to have settings such as contact email addresses, footer text, menu items et.c., which are stored as content in the content tree but not considered part of the site itself. You can specify a Document Type to be considered a Settings document. This will never be accessible as a page but will instead be accessible through a fixed API endpoint. Create a settings Document Type as normal and assign it a Template.

##### Not Found Type [TODO]

You can create a Document Type which is intended to be used as a 404 page. Create a Document Type as normal and assign it a Template.
You can then configure this Document Type in the `appsettings.json` file. Once this is done, any route that doesn't respond with a Document will return this document's JSON instead.

##### Robots Txt

This setting determines which Document Type and Property the robots.txt text can be found.

##### Keep Empty Properties TODO

If this is set to true then when serializing content to json, properties with null values or empty lists will be kept, otherwise they are removed.

[TODO]
Describe multi-site and preview configuration.

<br>
## Search

Searches using Umbraco CMS's Examine indexes is also possible to do with Umbraco.Delivery.

* Each 'canned search' can be implemented with it's own endpoint
* Searches can be executed by calling this endpoint and query string parameters can be used to control the search
* Paging parameters are available to all searches
* `ModelNodeConverters` can be written to operate on search results, giving developers the option to customize the search output

### Implementing ISearchQuery Classes [TODO]

As a developer you can build your own Examine search queries

* Implement a class with the `ISearchQuery` interface
* Apply the SearchQuery attribute to the class. E.g.; `[SearchQuery("ExternalIndex", "All")]`
* You can write your own Examine search queries in the `Execute` method

Example ISearchQuery class

```
using Examine;
using Kruso.Umbraco.Headless;
using Kruso.Umbraco.Headless.Extensions;
using Kruso.Umbraco.Headless.Models;
using Kruso.Umbraco.Headless.Search;

namespace Majworld.Website.Code.Search
{
    [SearchQuery("ExternalIndex", "All")]
    public class AllSearchQuery : ISearchQuery
    {
        public ISearchResults Execute(ISearcher searcher, SearchRequest searchRequest)
        {
            var freeText = searchRequest.Params.Val<string>("freeText");
            if (!string.IsNullOrEmpty(freeText))
            {
                var results = searcher
                    .CreateQuery("content")
                        .ManagedQuery(freeText)
                    .And()
                        .FieldNot("deleted", "y")
                    .Execute();

                return results;
            }

            return null;
        }
    }
}
```

The above example query provides a simple Examine free text search in the ExternalIndex index. The freeText parameter is passed to the SearchQuery. The results are returned and the `Kruso.Umbraco.Headless` will convert the result to JSON and execute any ModelNodeConverters on the results.

* The examine index the query will execute on. In this case the `ExternalIndex` index
* The endpoint the query can be executed on. In this case it will be `/api/v1/{culture}/search/all`
* The SearchRequest input parameter provides information about the query including any query string params specified when the above endpoint is called.

## Page and property converters - TODO Needs to be verified

Although Umbraco.Delivery will generate Json without further input, you may want to change the returned Json depending on your needs:

1. ~~**Brevity**. The generated JSON may be too long, with a lot of unnecessary properties. You can remove or alter the structure of the JSON.~~
2. **Additional Content**. You might wish to append additional properties to your page's JSON, for example, from a different data source or from a more complex query to find other Umbraco nodes.
3. **Additional standardised fields**. You might want all Document Types to contain additional standard fields. These can be added programmatically.
4. **Custom Properties**. You might make custom Properties in Umbraco. You will need to provide a Property Resolver for it to appear in your page's JSON.

### 6.1 Pages and Blocks

`Kruso.Umbraco.Headless` treats Pages as top level content that is returned by a page request. Blocks are considered to be embedded content within a page, either by existing only within the page (Nested Content) or by content linked directly into the page (Multi Node Tree Picker, Media Picker). Some content items can therefore be considered both Blocks and Pages if, for example, a Page is added as a Block to another page with the Multi Node Tree Picker property.

### 6.2 Converters

A custon converter allow for the returned Json for Pages and Blocks to be altered as desired before they are returned. You can create a Converter anywhere in your Umbraco CMS project with two requirements:

1. Your converter class requires an attribute specifying its type and the Alias for the Document Type is will act on
2. Your converter class must implement the IModelConverter interface.

There are four types of Converters:

1. **Page Converters**. These operate only on Page items
2. **Block Converters**. These operate only on Block content, that is, content embedded within a Page.
3. **Navigation Converters**. This special converter type operates on route structure nodes returned on the `/umbraco/api/v1/manifest` endpoint. You can use it to add custom properties if required.
4. **Search Converters**. This type of converter will convert the results of searches using Examine indexes. See more on Search functionality later.

#### 6.2.1 Page Converters

The following example demonstrates the basic features of a page converter:

1. It converts the `StandardArticlePage` DocumentType
2. It is a Page converter. A `StandardArticlePage` is a complete document returned in response to a content request.
3. The `Convert()` method has two params:
4. the `source` param contains a complete default representation of a page
5. The `target` param will contain the updated representation of a page. You are responsible for building the new representation
6. The `ModelNode` class has a number of methods to help you convert your content into the desired format

In this example we are altering an ArticlePage JSON to add a new SEO property that contains a bunch of new fields and values. Additionally, we are moving a couple of properties (`articleName` and `articleDescription`) into this new SEO property.

* Notice that `ModelNode` methods can be chained together for brevity
* You can fetch values from the source using the `Val<T>("[property name]")` generic method.
* The `CopyAllExceptProps()` method can be used to copy all properties from source to target with exceptions

```
using Kruso.Umbraco.Headless;
using Kruso.Umbraco.Headless.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Majworld.Website.Code.Converters
{
    [ModelNodeConverter(TemplateType.Page, "StandardArticlePage")]
    public class StandardArticlePageConverter : IModelNodeConverter
    {
        private readonly StandardArticlePageConverterHelper _helper;

        public StandardArticlePageConverter(StandardArticlePageConverterHelper helper)
        {
            _helper = helper;
        }

        public void Convert(ModelNode target, ModelNode source)
        {
            var id = source.Val<int>("cmscode");
            var culture = source.Val<string>("culture");
            var category = _helper.GetCategory(id, culture);

            target
                .CopyAllExceptProps(source, "isRoot", "cmscodepage", "sortOrder")
                .CopyProp("categoryId", category, "cmscode")
                .CopyProp("categoryName", category, "title");
        }
    }
}
```

#### 6.2.2 Block Converters

A block converter works identically to the above page converter but with a `TemplateType.Block` property set on the Converter attribute. Blocks are not pages, but are nested content on a page as created with Umbraco fields:

* `Umbraco.NestedContent`
* `Umbraco.ContentPicker`
* `Umbraco.MultiNodeTreePicker`
* `Umbraco.BlockList`

A Block converter will work on any 'node' of the JSON structure as long as that node has an `id` and `type` property, so you can create converters on Image properties for example.

#### 6.2.3 Navigation Converter TODO

You can also make a navigation converter if required. THis allows you alter the standard output of the `/umbraco/api/v1/manifest` endpoint (routes).

In the following example, if the `target ModelNode` is for a document of type `productCategoryPage` then it will fetch the page, find the color property of the page, and add this property to the navigation JSON for this page.

```
using Kruso.Umbraco.Headless;
using Kruso.Umbraco.Headless.Models;
using Umbraco.Web.Composing;

namespace Kruso.Cms.Code.Converters
{
    [ModelNodeConverter(ConverterType.Navigation)]
    public class NavigationConverter : IModelNodeConverter
    {
        public void Convert(ModelNode target, ModelNode source)
        {
            target.CopyAllProps(source);

            if (target.Val<string>("type") == "productCategoryPage")
            {
                var id = target.Val<int>("cmscode");
                var culture = target.Val<string>("culture");

                var content = Current.UmbracoHelper.Content(id);
                var factory = new ModelFactory(content, culture);
                var page = factory.CreatePage();
                target.AddProp("color", page.Val<string>("color"));
            }
        }
    }
}
```

### 6.3 Property Value Factories

By default you probably won't need to create your own property value factories. `Kruso.Umbraco.Headless` has a property value factory for each default property type already. However, you may want to alter the default JSON for a given property type, or you might create your own custom property types. In these cases you can create your own. The following example is the default `PropertyValueFactory` for the `Umbraco.ColorPicker` property type.

NOTE: If you create your own in your Umbraco project in the same manor, this default property resolver will be overridden for **ALL** properties of type Umbraco.ColorPicker

```
using Kruso.Umbraco.Headless.Extensions;
using Newtonsoft.Json.Linq;
using Umbraco.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Headless.ModelGeneration.PropertyValueFactories
{
    [PropertyValueFactory("Umbraco.ColorPicker")]
    public class ColorPickerPropertyValueFactory : IModelPropertyValueFactory
    {
        public virtual object Create(IModelFactoryContext context, IPublishedProperty property)
        {
            var value = context.GetPropertyValue(property)?.ToString();
            var json = value.ToJson();
            var color = json != null
                ? context.GetPropertyValue(json.ToObject<JObject>(), "value")
                : value;

            return !string.IsNullOrEmpty(color) ? $"#{color}" : null;
        }
    }
}
```

### 6.4 Templates

`Kruso.Umbraco.Headless` contains 3 Templates by default for Page, Block and Navigation objects. These are responsible for creating the initial 'template' `ModelNode` with some standardised properties for these objects. If you wish to ensure that all pages, regardless of type, contains some standardised properties (maybe some SEO properties or navigation info like breadcrumbs) then you have the option of replacing the default factory classes with your own.

In the following example, the default Block factory is replaced. Just create your own class as follows in you Umbraco project. This will ensure that every block is created with the seven base properties of `id, cmscode, cmscodePage, culture, name, type, sortOrder`:

```
using Kruso.Umbraco.Headless.Extensions;
using Kruso.Umbraco.Headless.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Headless.ModelGeneration.Templates
{
    [ModelTemplate(TemplateType.Block)]
    public class BlockModelTemplate : IModelTemplate
    {
        public virtual ModelNode Create(IModelFactoryContext context, ModelNode props, IPublishedContent content)
        {
            return Create(context, props, content, null);
        }

        public virtual ModelNode Create(IModelFactoryContext context, ModelNode props, IPublishedContent content, params string[] excludeProps)
        {
            var block = new ModelNode()
                .AddProp("id", content.HeadlessId(context.Culture))
                .AddProp("cmscode", content?.Id)
                .AddProp("cmsname", content.Name)
                .AddProp("cmscodePage", context.Page?.Id)
                .AddProp("culture", context.Culture)
                .AddProp("type", content.ContentType.Alias.Capitalize())
                .AddProp("sortOrder", content.SortOrder)
                .CopyAllProps(props);

            return block;
        }
    }
}
```

### 6.5 The ModelNode Class

The `ModelNode` class is a dynamic object that is used throughout `Kruso.Umbraco.Headless` to represent the JSON that will be returned. It also has a number of extension methods that help you to manipulate the properties and values that the final JSON structure will contain. The methods are pretty self-explanatory in their operation.

You can chain them together for brevity. Each method (with exceptions) returns the `ModelNode` object itself.
Most methods will even work on a NULL `ModelNode` object. We want as few exceptions as possible.

### 6.6 Dependency Injection

Umbraco CMS uses `LightInject` as a dependency container by default and all the classes in `Kruso.Umbraco.Headless` are added to the container by default. You can therefore use contructor injection for any converters or factories you implement, for example:

```
    [ModelNodeConverter(TemplateType.Page)]
    public class DefaultPageConverter : IModelConverter
    {
        protected readonly IDataService DataService;
        protected readonly ILanguageService LanguageService;

        public DefaultPageConverter(ILanguageService languageService, IDataService dataService)
        {
            DataService = dataService;
            LanguageService = languageService;
        }
```

### 6.7 Manually Converting IPublishedContent to JSON

By default any requests made to Umbraco CMS with `Kruso.Umbraco.Headless` installed will automatically be converted to JSON without intervention but it can be useful to be able to convert Umbraco content manually. For example, you might need to fetch unrelated content from the CMS and add it to a page's default JSON output (using a `ModelNodeConverter`). In this case you can load the Umbraco content manually and use `ModelFactory` and `ModelConverter` objects to perform this transformation:

In the following example:

1. The site settings page (assuming id 1022) it loaded from the Umbraco content cache.
2. A `ModelFactory` object is created that will convert the `IPublishedContent` (siteSettings) object into a `ModelNode` object (`CreatePage()` is run on the `ModelFactory` so the content will be converted as a Page, not a Block) containing all the properties and values from the siteSettings content.
3. A `ModelConverter` object is create that will run any `ModelNodeConverter` objects defined for the siteSettings `DocumentType` on the `ModelNode`.
4. The final `ModelNode` object is returned.

```
public ModelNode GetSiteSettings(string culture)
{
    var siteSettings = Current.UmbracoHelper.Content(1022);
    var factory = new ModelFactory(siteSettings, culture);
    var converter = new ModelConverter();

    return converter.Convert(factory.CreatePage());
}
```

#### 6.7.1 The ModelFactory Class

The `ModelFactory` class contains the fundamental code used to convert Umbraco CMS content (`IPublishedContent`) into a DataNode object which is then serialized to JSON.

1. **Recursion protection**. It will ensure that the conversion process exits any infinite loop. Sometimes a page can refer to blocks which in turn refer back to the same page. Without recursion protection an infinite loop (and a stack overflow exception) would occur.
2. **Depth control**. You can manually specify how deeply the factory is to traverse the content tree. For example, a page can refer to blocks which can refer to other blocks and so on. In many cases you might not need the entire structure of blocks, only the highest level. you can specify a maximum depth if needed.
3. **Customizable behaviour** with `PropertyValueFactories` and `PageFactories`. You can create custom Page/Block Factories and PropertyValueFactories to modify how pages/blocks and property values are created as desired. The default factories are replaced with your custom ones.
4. Choice to convert content to Blocks or Pages.

#### 6.7.2 The ModelConverter Class

The `ModelConverter` class will execute any Page and Block converters on any `ModelNode` object. It will discover which Blocks and Page object the DataNode represents by checking the `type` property on any `ModelNode` object.

1. **No default behaviour**. By default the `ModelConverter` will not do anything. It will only execute `ModelNodeConverters` if you implement some in your project, there are no default `ModelNodeConverters` in `Kruso.Umbraco.Headless`.
2. **Bottom Up**. In a `ModelNode` containing a number of sub-nodes representing `Blocks`, it will iterate down to the lowest leaves of the `ModelNode` tree and convert the lowest items first, recursing up the tree from there.

### 6.8 The IModelFactoryContext Object

The ModelFactoryContext is an important internal object within the ModelFactory. This object is passed to each Factory when it executes and contains information about the Factories culture, depth and recursion protection, and contains methods that can be used when generating DataNode models.

##