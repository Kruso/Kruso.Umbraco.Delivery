using Kruso.Umbraco.Delivery.Controllers;
using Kruso.Umbraco.Delivery.Controllers.Renderers;
using Kruso.Umbraco.Delivery.Helper;
using Kruso.Umbraco.Delivery.ModelConversion;
using Kruso.Umbraco.Delivery.ModelGeneration;
using Kruso.Umbraco.Delivery.ModelGeneration.PropertyValueFactories;
using Kruso.Umbraco.Delivery.ModelGeneration.Templates;
using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Routing.Implementation;
using Kruso.Umbraco.Delivery.Search;
using Kruso.Umbraco.Delivery.Security;
using Kruso.Umbraco.Delivery.Services;
using Kruso.Umbraco.Delivery.Services.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Website.Controllers;
using Umbraco.Extensions;

namespace Kruso.Umbraco.Delivery
{
    public static class Setup
    {
        public static IApplicationBuilder UseUmbracoDelivery(this IApplicationBuilder app)
        {
            app.UseMiddleware<XForwardedMiddleware>();

            return app;
        }

        public static IUmbracoBuilder AddUmbracoDelivery(this IUmbracoBuilder builder)
        {
            builder.SetContentLastChanceFinder<DeliNotFoundContentFinder>();

            builder.ContentFinders()
                .InsertBefore<ContentFinderByUrl, DeliContentFinderByUrl>()
                .Remove<ContentFinderByUrl>()
                .InsertBefore<ContentFinderByIdPath, DeliContentFinderByIdPath>()
                .Remove<ContentFinderByIdPath>();

            builder.Components().Append<SearchIndexerComponent>();

            return builder;
        }

        public static IServiceCollection AddUmbracoDeliveryServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddMemoryCache()
                .AddSingleton<IDeliConfig, DeliConfig>()
                .AddSingleton<IDeliCache, DeliCache>()
                .AddSingleton<IDeliSecurity, DeliSecurity>()
                .AddSingleton<IDeliCulture, DeliCulture>()
                .AddSingleton<IDeliContent, DeliContent>()
                .AddSingleton<IDeliContentTypes, DeliContentTypes>()
                .AddSingleton<IDeliTemplates, DeliTemplates>()
                .AddSingleton<IDeliDomain, DeliDomain>()
                .AddSingleton<IDeliPages, DeliPages>()
                .AddSingleton<IDeliDataTypes, DeliDataTypes>()
                .AddSingleton<IDeliContentLoader, DeliContentLoader>()
                .AddSingleton<IDeliRequestAccessor, DeliRequestAccessor>()
                .AddSingleton<ICertificateHandler, CertificateHandler>()
                .AddSingleton<IAuthTokenHandler, AuthTokenHandler>()
                .AddSingleton<IDeliContentTypes, DeliContentTypes>()
                .AddSingleton<IDeliMedia, DeliMedia>()
                .AddSingleton<IDeliProperties, DeliProperties>()
                .AddSingleton<IDeliUrl, DeliUrl>();

            services
                .AddTransient<XForwardedMiddleware>()
                .AddScoped<IUserIdentity, DefaultIdentity>()
                .AddScoped<PageRenderer>()
                .AddScoped<ManifestRenderer>()
                .AddScoped<SitemapRenderer>()
                .AddScoped<RobotsRenderer>();

            services
                .AddSingleton<IModelPropertyValueFactory, SliderPropertyValueFactory>()
                .AddSingleton<IModelPropertyValueFactory, CheckboxListPropertyValueFactory>()
                .AddSingleton<IModelPropertyValueFactory, ColorPickerPropertyValueFactory>()
                .AddSingleton<IModelPropertyValueFactory, ContentPickerPropertyValueFactory>()
                .AddSingleton<IModelPropertyValueFactory, DefaultPropertyValueFactory>()
                .AddSingleton<IModelPropertyValueFactory, DropDownPropertyValueFactory>()
                .AddSingleton<IModelPropertyValueFactory, ImageCropperPropertyValueFactory>()
                .AddSingleton<IModelPropertyValueFactory, MediaPickerPropertyValueFactory>()
                .AddSingleton<IModelPropertyValueFactory, MultiNodeTreePickerPropertyValueFactory>()
                .AddSingleton<IModelPropertyValueFactory, MultipleTextstringPropertyValueFactory>()
                .AddSingleton<IModelPropertyValueFactory, MultiUrlPickerPropertyValueFactory>()
                .AddSingleton<IModelPropertyValueFactory, NestedContentPropertyValueFactory>()
                .AddSingleton<IModelPropertyValueFactory, RelatedLinksPropertyValueFactory>()
                .AddSingleton<IModelPropertyValueFactory, TinyMCEPropertyValueFactory>()
                .AddSingleton<IModelPropertyValueFactory, TrueFalsePropertyValueFactory>()
                .AddSingleton<IModelPropertyValueFactory, BlockListPropertyValueFactory>()
                .AddSingleton<IModelPropertyValueFactory, BlockGridPropertyValueFactory>();

            services
                .AddSingleton<IModelTemplate, PageModelTemplate>()
                .AddSingleton<IModelTemplate, BlockModelTemplate>()
                .AddSingleton<IModelTemplate, RouteModelTemplate>()
                .AddSingleton<IModelTemplate, RefModelTemplate>()
                .AddSingleton<IPropertyModelTemplate, PropertyModelTemplate>();

            services
                .AddSingleton<IModelConverterComponentSource, ModelConverterComponentSource>()
                .AddSingleton<IModelConverter, ModelConverter>()
                .AddScoped<ISearchQueryExecutor, SearchQueryExecutor>()
                .AddScoped<IModelFactoryContext, ModelFactoryContext>()
                .AddTransient<IModelFactory, ModelFactory>();

            services.Configure<UmbracoRenderingDefaultsOptions>(c =>
            {
                c.DefaultControllerType = typeof(DeliRenderController);
            });

            //services.AddConfig<DeliveryConfig>(configuration, "Umbraco.Delivery");
            services.AddConfig<DeliveryConfig>(configuration, "UmbracoDelivery");

            VersionHelper.RegisterVersion(typeof(Setup).Assembly);

            return services;
        }

        public static IServiceCollection AddConfig<T>(this IServiceCollection services, IConfiguration configuration, string sectionName)
            where T : class, new()
        {
            services.Configure<T>(configuration.GetSection(sectionName));
            services.AddSingleton(sp => sp.GetService<IOptions<T>>().Value);

            return services;
        }
    }
}
