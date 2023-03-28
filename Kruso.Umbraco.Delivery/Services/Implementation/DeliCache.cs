using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using Umbraco.Extensions;

namespace Kruso.Umbraco.Delivery.Services.Implementation
{
    public class DeliCache : IDeliCache
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<DeliCache> _logger;

        public DeliCache(IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, ILogger<DeliCache> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public T GetFromRequest<T>(string cacheKey, Func<T> create)
        {
            var context = _httpContextAccessor.HttpContext;
            if (ValidForRequest(cacheKey) && context.Items.TryGetValue(cacheKey, out var val) && val is T)
                return (T)val;

            var def = create != null
                ? create()
                : default(T);

            AddToRequest(cacheKey, def);
                
            return def;
        }

        public T GetFromRequest<T>(string cacheKey, T def = default(T))
        {
            var context = _httpContextAccessor.HttpContext;
            if (ValidForRequest(cacheKey) && context.Items.TryGetValue(cacheKey, out var val) && val is T)
                return (T)val;

            if (def != null)
                AddToRequest(cacheKey, def);

            return def;
        }

        public bool ExistsOnRequest(string cacheKey)
        {
            var context = _httpContextAccessor.HttpContext;
            return ValidForRequest(cacheKey) && context.Items.ContainsKey(cacheKey);
        }

        public void AddToRequest(string cacheKey, object val)
        {
            if (ValidForRequest(cacheKey))
            {
                var context = _httpContextAccessor.HttpContext;
                context.Items.Add(cacheKey, val);
            }
        }

        public void ReplaceOnRequest(string cacheKey, object val)
        {
            if (ExistsOnRequest(cacheKey))
            {
                var context = _httpContextAccessor.HttpContext;
                context.Items.Remove(cacheKey);
            }

            AddToRequest(cacheKey, val);
        }

        public T GetFromMemory<T>(string cacheKey, T def = default(T))
        {
            if (ValidForMemory(cacheKey) && _memoryCache.TryGetValue(cacheKey, out T val))
            {
                _memoryCache.Remove(cacheKey);
                return val;
            }

            return def;
        }

        public bool RemoveFromMemory(string cacheKey)
        {
            if (ExistsInMemory(cacheKey))
            {
                _memoryCache.Remove(cacheKey);
                return true;
            }

            return false;
        }

        public bool ExistsInMemory(string cacheKey)
        {
            return ValidForMemory(cacheKey) && _memoryCache.TryGetValue(cacheKey, out object val);
        }

        public void AddToMemory(string cacheKey, object val)
        {
            if (ValidForMemory(cacheKey) && val != null)
                _memoryCache.Set(cacheKey, val);
        }

        private bool ValidForMemory(string cacheKey)
        {
            var res = !string.IsNullOrEmpty(cacheKey);

            if (!res)
                _logger.LogDebug($"Invalid for memory cache. Cache key {cacheKey}");

            return res;
        }

        private bool ValidForRequest(string cacheKey)
        {
            var context = _httpContextAccessor.HttpContext;
            var res = !string.IsNullOrEmpty(cacheKey) && context != null;

            if (!res)
                _logger.LogDebug($"Invalid for request cache. Cache key {cacheKey}, HttpContext: {context != null}");

            return res;
        }
    }
}
