using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;

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
            if (ValidForRequest(cacheKey) && _httpContextAccessor.HttpContext.Items.TryGetValue(cacheKey, out var val) && val is T)
                return (T)val;

            var def = create != null
                ? create()
                : default(T);

            AddToRequest(cacheKey, def);
                
            return def;
        }

        public T GetFromRequest<T>(string cacheKey, T def = default(T))
        {
            if (ValidForRequest(cacheKey) && _httpContextAccessor.HttpContext.Items.TryGetValue(cacheKey, out var val) && val is T)
                return (T)val;

            if (def != null)
                AddToRequest(cacheKey, def);

            return def;
        }

        public bool ExistsOnRequest(string cacheKey)
        {
            return ValidForRequest(cacheKey) && _httpContextAccessor.HttpContext.Items.ContainsKey(cacheKey);
        }

        public void AddToRequest(string cacheKey, object val)
        {
            if (ValidForRequest(cacheKey))
            {
                _httpContextAccessor.HttpContext.Items.Add(cacheKey, val);
            }
        }

        public void ReplaceOnRequest(string cacheKey, object val)
        {
            if (ExistsOnRequest(cacheKey))
                _httpContextAccessor.HttpContext.Items.Remove(cacheKey);

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
                _logger.LogInformation($"Invalid for memory cache. Cache key {cacheKey}");

            return res;
        }

        private bool ValidForRequest(string cacheKey)
        {
            var res = !string.IsNullOrEmpty(cacheKey) && _httpContextAccessor.HttpContext != null;

            if (!res)
                _logger.LogInformation($"Invalid for request cache. Cache key {cacheKey}, HttpContext: {_httpContextAccessor.HttpContext != null}");

            return res;
        }
    }
}
