This package will add a REST-like content delivery API to Umbraco CMS. It provides a number of API endpoints to access content, search, and other functions.

## Works on

Umbraco v11+

## Setup

Find full setup details in the Wiki _[here](https://github.com/Kruso/Kruso.Umbraco.Delivery/wiki)_

Run the following or or find the package on NuGet _[here](https://www.nuget.org/packages/Kruso.Umbraco.Delivery)_
```
dotnet add Kruso.Umbraco.Delivery
```

Configure Startup.ConfigureServices() like this:

```
    services.AddUmbracoDeliveryServices(_config);

    services.AddUmbraco(_env, _config)
        .AddBackOffice()
        .AddWebsite()
        .AddComposers()
        .AddUmbracoDelivery()
        .AddAzureBlobMediaFileSystem()
        .Build();
```

And Startup.Configure() like this:

```
    app.UseUmbracoDelivery();
    app.UseResponseCompression();

    app.UseUmbraco()
        .WithMiddleware(u =>
        {
            u.UseBackOffice();
            u.UseWebsite();
        })
        .WithEndpoints(u =>
        {
            u.UseInstallerEndpoints();
            u.UseBackOfficeEndpoints();
            u.UseWebsiteEndpoints();
        });
```

## Design Goals

* **Minimal Installation and Configuration requirements**. As far as possible, you should be able to Install Kruso.Umbraco.Delivery and start developing.
* **Respect how Umbraco works**. Use umbraco as you would expect as a content editor or developer. Kruso.Umbraco.Delivery will only return Json for a page if you want it to.
* **Performance**. Json documents should be returned fast and Umbraco should continue to scale well.
* **Support Small Sites and Large**. Many smaller sites use Umbraco as a place to implement other backend functions not related to the CMS, such as fetching data from external system. Allow developers to modify the Json data to include custom data.

## Main features

* [Get content](https://github.com/Kruso/Kruso.Umbraco.Delivery/wiki/2.-Using-Kruso.Umbraco.Delivery#get-content) in various ways. There are several ways to tweak the outputted JSON and how deep content structure that will be returned.
* [Search](https://github.com/Kruso/Kruso.Umbraco.Delivery/wiki/9.-Search-Queries-and-Indexing) using Umbraco's Examine API and customize both the response and how content is indexed.
* [Provide SEO support](https://github.com/Kruso/Kruso.Umbraco.Delivery/wiki/4.-SEO-Features). Get an XML sitemap and robots.txt. Also supports Umbraco's handling of alternative urls
* [Return all Translations](https://github.com/Kruso/Kruso.Umbraco.Delivery/wiki/2.-Using-Umbraco.Delivery#the-manifest) set up in Umbraco.
* Supports multiple sites (in multiple languages) without the need for addtional host names for your Umbraco instance.
* Supports "hybrid mode". [A page can be returned as a traditional page, or as Json](https://github.com/Kruso/Kruso.Umbraco.Delivery/wiki/1.-Get-Started#the-umbraco-json-template).
* [Get information about sites and languages, and get all available routes for all documents](https://github.com/Kruso/Kruso.Umbraco.Delivery/wiki/2.-Using-Umbraco.Delivery#the-manifest).
* Supports [secure/private preview](https://github.com/Kruso/Kruso.Umbraco.Delivery/wiki/3.-Configuration#preview-configuration).

As far as possible Umbraco.Delivery will work out of the box without further configuration, even for a multi-site and multi-language setup.

### History

This package is mainly the work of [@EdSoanes](https://github.com/EdSoanes). It has been used in several solutions during several years, from Umbraco v8 to the latest v11. This library has recently received several new features, but the core functionality is well battle tested.
It was made public as Kruso.Umbraco.Delivery because we at [Kruso](https://kruso.dk/en/) love open source, and want to contribute to the community. It also keeps us on our toes when we know others will look at our code 😉.
<br>
## How to use

Please read the [wiki](https://github.com/Kruso/Kruso.Umbraco.Delivery/wiki). <br>
Some features may not be well documented. You are welcome to open an [issue](https://github.com/Kruso/Kruso.Umbraco.Delivery/issues) if you have any questions. <br>

## Possible roadmap
* Include a basic ISearchQuery. Menaing that basic search will be enabled without any coding needed.
* Simpler preview without the need for a certificate. Will be ok for sites without very high security requirements.

