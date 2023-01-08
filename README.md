# Umbraco Delivery API


## Introduction to Umbraco.Delivery

This package will add a REST-like content delivery API to Umbraco CMS. It provides a number of API endpoints to access content, search, and other functions.

### Main features

* [Get content](https://github.com/Kruso/Kruso.Umbraco.Delivery/wiki/2.-Using-Kruso.Umbraco.Delivery#get-content) in various ways. There are several ways to tweak the outputted JSON and how deep content structure that will be returned.
* [Search](https://github.com/Kruso/Kruso.Umbraco.Delivery/wiki/9.-Search-Queries-and-Indexing) using Umbraco's Examine API and customize both the response and how content is indexed.
* [Provide SEO support](https://github.com/Kruso/Kruso.Umbraco.Delivery/wiki/4.-SEO-Features). Get an XML sitemap and robots.txt. Also supports Umbraco's handling of alternative urls
* [Return all Translations](https://github.com/Kruso/Kruso.Umbraco.Delivery/wiki/2.-Using-Umbraco.Delivery#the-manifest) set up in Umbraco.
* Supports multiple sites (in multiple languages) without the need for addtional host names for your Umbraco instance.
* Supports "hybrid mode". [A page can be returned as a traditional page, or as Json](https://github.com/Kruso/Kruso.Umbraco.Delivery/wiki/1.-Get-Started#the-umbraco-json-template).
* [Get information about sites and languages, and get all available routes for all documents](https://github.com/Kruso/Kruso.Umbraco.Delivery/wiki/2.-Using-Umbraco.Delivery#the-manifest).

As far as possible Umbraco.Delivery will work out of the box without further configuration, even for a multi-site and multi-language setup.


### Demo site

[TODO link to demo site]
<br>
### History

This package is mainly the work of [@EdSoanes](https://github.com/EdSoanes). It has been used in several solutions during several years, from Umbraco v8 to the latest v11. This library have lately got several new features, but the core functionality is well battle tested.
It was made public as Kruso.Umbraco.Delivery because we at [Kruso](https://kruso.dk/en/) love open source, and want to contribute to the community. It also keeps us on our toes when we know others will look at our code 😉.
<br>
## How to use

Please read the [wiki](https://github.com/Kruso/Kruso.Umbraco.Delivery/wiki). <br>
There are still some features that are not documented. You are welcome to open an [issue](https://github.com/Kruso/Kruso.Umbraco.Delivery/issues) if you have any questions. <br>



[TODO]
Describe multi-site setup, and preview configuration.
