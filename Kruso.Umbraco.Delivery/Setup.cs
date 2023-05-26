using Kruso.Umbraco.Delivery.Controllers;
using Kruso.Umbraco.Delivery.Controllers.Renderers;
using Kruso.Umbraco.Delivery.Helper;
using Kruso.Umbraco.Delivery.ModelConversion;
using Kruso.Umbraco.Delivery.ModelGeneration;
using Kruso.Umbraco.Delivery.ModelGeneration.Templates;
using Kruso.Umbraco.Delivery.Publishing;
using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Routing.Implementation;
using Kruso.Umbraco.Delivery.Search;
using Kruso.Umbraco.Delivery.Security;
using Kruso.Umbraco.Delivery.Services;
using Kruso.Umbraco.Delivery.Services.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Web.Website.Controllers;
using Umbraco.Extensions;

namespace Kruso.Umbraco.Delivery
{
    public class UmbracoDeliveryOptions
    {
        internal Dictionary<string, Type> ModelPropertyValueFactories = new Dictionary<string, Type>();
        internal Type PropertyModelTemplate = typeof(PropertyModelTemplate);
        internal Dictionary<string, Type> ModelTemplates = new Dictionary<string, Type>();
        internal Dictionary<string, Type> ModelNodeConverters = new Dictionary<string, Type>();
        internal Dictionary<string, Type> ModelNodeListConverters = new Dictionary<string, Type>();
        internal List<Type> SearchQueries = new List<Type>();
        internal List<Type> SearchIndexers = new List<Type>();
        internal List<Type> EventHandlers = new List<Type>();

        public string ConfigSection = "UmbracoDelivery";

        public void AddComponentsFrom<T>()
        {
            AddComponentsFrom(typeof(T).Assembly);
        }

        public void AddComponentsFrom(Assembly assembly)
        {
            GetAllWithInterface<IModelPropertyValueFactory, ModelPropertyValueFactoryAttribute>(assembly, ModelPropertyValueFactories);
            GetAllWithInterface<IModelTemplate, ModelTemplateAttribute>(assembly, ModelTemplates);
            GetAllWithInterface<IModelNodeConverter, ModelNodeConverterAttribute>(assembly, ModelNodeConverters);
            GetAllWithInterface<IModelNodeListConverter, ModelNodeListConverterAttribute>(assembly, ModelNodeListConverters);

            SearchQueries.AddRange(GetAllWithInterface<ISearchQuery>(assembly));
            SearchIndexers.AddRange(GetAllWithInterface<ISearchIndexer>(assembly));
            EventHandlers.AddRange(GetAllWithInterface<IDeliEventHandler>(assembly));
        }

        private void GetAllWithInterface<T, TA>(Assembly assembly, Dictionary<string, Type> componentMap)
            where T : class
            where TA : IdentifiableAttribute
        {
            foreach (var componentType in GetAllWithInterface<T>(assembly))
            {
                var identifiableAttrs = componentType.GetCustomAttributes(typeof(TA), true).Cast<TA>();
                foreach (var identifiableAttr in identifiableAttrs)
                {
                    foreach (var c in identifiableAttr.Components)
                    {
                        if (!componentMap.ContainsKey(c))
                        {
                            componentMap.Add(c, componentType);
                        }
                        else
                        {
                            componentMap[c] = componentType;
                        }
                    }
                }
            }
        }

        private Type[] GetAllWithInterface<T>(Assembly assembly)
            where T : class
        {
            return assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(typeof(T)))
                .ToArray();
        }
    }

    public static class Setup
    {
        public static IApplicationBuilder UseUmbracoDelivery(this IApplicationBuilder app)
        {
            app.UseMiddleware<DeliRequestMiddleware>();

            return app;
        }

        public static IUmbracoBuilder AddUmbracoDelivery(this IUmbracoBuilder builder)
        {
            builder.SetContentLastChanceFinder<DeliNotFoundContentFinder>();

            builder.ContentFinders()
                .InsertBefore<ContentFinderByUrl, DeliContentFinderByUrl>()
                .Remove<ContentFinderByUrl>()
                .InsertBefore<ContentFinderByIdPath, DeliContentFinderByPreviewUrl>()
                .Remove<ContentFinderByIdPath>();

            builder.Components().Append<SearchIndexerComponent>();

            builder
                .AddNotificationHandler<ContentPublishedNotification, DeliPublishedNotificationHandler>()
                .AddNotificationHandler<ContentDeletedNotification, DeliDeletedNotificationHandler>()
                .AddNotificationHandler<ContentMovedNotification, DeliMovedNotificationHandler>()
                .AddNotificationHandler<ContentMovedToRecycleBinNotification, DeliMovedToTrashNotificationHandler>()
                .AddNotificationHandler<ContentSavedNotification, DeliSavedNotificationHandler>();

            return builder;
        }

        public static IServiceCollection AddUmbracoDeliveryServices(this IServiceCollection services, IConfiguration configuration, Action<UmbracoDeliveryOptions> optionsAction = null)
        {
            var options = new UmbracoDeliveryOptions();
            options.AddComponentsFrom(Assembly.GetExecutingAssembly());

            optionsAction?.Invoke(options);

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
                .AddSingleton<ICertificateHandler, CertificateHandler>()
                .AddSingleton<IAuthTokenHandler, AuthTokenHandler>()
                .AddSingleton<IDeliContentTypes, DeliContentTypes>()
                .AddSingleton<IDeliMedia, DeliMedia>()
                .AddSingleton<IDeliProperties, DeliProperties>()
                .AddSingleton<IDeliUrl, DeliUrl>();

            services
                .AddTransient<DeliRequestMiddleware>()
                .AddSingleton<IDeliRequestAccessor, DeliRequestAccessor>()
                .AddScoped<IUserIdentity, DefaultIdentity>()
                .AddTransient<PageRenderer>()
                .AddTransient<BlockRenderer>()
                .AddTransient<ChildPageRenderer>()
                .AddTransient<ManifestRenderer>()
                .AddTransient<SitemapRenderer>()
                .AddTransient<RobotsRenderer>();


            //services
            //    .AddSingleton<IModelPropertyValueFactory, SliderPropertyValueFactory>()
            //    .AddSingleton<IModelPropertyValueFactory, CheckboxListPropertyValueFactory>()
            //    .AddSingleton<IModelPropertyValueFactory, RadioButtonListPropertyValueFactory>()
            //    .AddSingleton<IModelPropertyValueFactory, ColorPickerPropertyValueFactory>()
            //    .AddSingleton<IModelPropertyValueFactory, ContentPickerPropertyValueFactory>()
            //    .AddSingleton<IModelPropertyValueFactory, DefaultPropertyValueFactory>()
            //    .AddSingleton<IModelPropertyValueFactory, DropDownPropertyValueFactory>()
            //    .AddSingleton<IModelPropertyValueFactory, ImageCropperPropertyValueFactory>()
            //    .AddSingleton<IModelPropertyValueFactory, MediaPickerPropertyValueFactory>()
            //    .AddSingleton<IModelPropertyValueFactory, MultiNodeTreePickerPropertyValueFactory>()
            //    .AddSingleton<IModelPropertyValueFactory, MultipleTextstringPropertyValueFactory>()
            //    .AddSingleton<IModelPropertyValueFactory, MultiUrlPickerPropertyValueFactory>()
            //    .AddSingleton<IModelPropertyValueFactory, NestedContentPropertyValueFactory>()
            //    .AddSingleton<IModelPropertyValueFactory, RelatedLinksPropertyValueFactory>()
            //    .AddSingleton<IModelPropertyValueFactory, TinyMCEPropertyValueFactory>()
            //    .AddSingleton<IModelPropertyValueFactory, TrueFalsePropertyValueFactory>()
            //    .AddSingleton<IModelPropertyValueFactory, BlockListPropertyValueFactory>()
            //    .AddSingleton<IModelPropertyValueFactory, BlockGridPropertyValueFactory>()
            //    .AddSingleton<IModelFactoryComponentSource, ModelFactoryComponentSource>();

            //services
            //    .AddSingleton<IModelTemplate, PageModelTemplate>()
            //    .AddSingleton<IModelTemplate, BlockModelTemplate>()
            //    .AddSingleton<IModelTemplate, MediaModelTemplate>()
            //    .AddSingleton<IModelTemplate, RouteModelTemplate>()
            //    .AddSingleton<IModelTemplate, RefModelTemplate>()
            //    .AddSingleton<IPropertyModelTemplate, PropertyModelTemplate>();

            services
                .AddSingleton<IDeliEventHandlerSource, DeliEventHandlerSource>();

            services
                .AddSingleton<IModelConverterComponentSource, ModelConverterComponentSource>()
                .AddSingleton<IModelFactoryComponentSource, ModelFactoryComponentSource>()
                .AddSingleton<ISearchQueryExecutorComponentSource, SearchQueryExecutorComponentSource>()
                .AddSingleton<IModelConverter, ModelConverter>()
                .AddSingleton<IModelFactory, ModelFactory>()
                .AddSingleton<ISearchQueryExecutor, SearchQueryExecutor>()
                .AddTransient<IModelFactoryContext, ModelFactoryContext>();

            services.Configure<UmbracoRenderingDefaultsOptions>(c =>
            {
                c.DefaultControllerType = typeof(DeliRenderController);
            });

            services.AddConfig<DeliveryConfig>(configuration, options.ConfigSection);

            services.AddSingleton(typeof(IPropertyModelTemplate), options.PropertyModelTemplate);

            foreach (var propertyValueFactory in options.ModelPropertyValueFactories.Values)
                services.AddSingleton(typeof(IModelPropertyValueFactory), propertyValueFactory);

            foreach (var modelTemplate in options.ModelTemplates.Values)
                services.AddSingleton(typeof(IModelTemplate), modelTemplate);

            foreach (var modelNodeConverter in options.ModelNodeConverters.Values)
                services.AddSingleton(typeof(IModelNodeConverter), modelNodeConverter);

            foreach (var modelNodeListConverter in options.ModelNodeListConverters.Values)
                services.AddSingleton(typeof(IModelNodeListConverter), modelNodeListConverter);

            foreach (var searchQuery in options.SearchQueries)
                services.AddSingleton(typeof(ISearchQuery), searchQuery);

            foreach (var searchIndexer in options.SearchIndexers)
                services.AddSingleton(typeof(ISearchIndexer), searchIndexer);

            foreach (var searchIndexer in options.EventHandlers)
                services.AddSingleton(typeof(IDeliEventHandler), searchIndexer);

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
