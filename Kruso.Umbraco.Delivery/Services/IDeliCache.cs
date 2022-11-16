﻿namespace Kruso.Umbraco.Delivery.Services
{
    public interface IDeliCache
    {
        void AddToMemory(string cacheKey, object val);
        void AddToRequest(string cacheKey, object val);
        bool ExistsInMemory(string cacheKey);
        bool ExistsOnRequest(string cacheKey);
        T GetFromMemory<T>(string cacheKey, T def = default);
        T GetFromRequest<T>(string cacheKey, T def = default);
        bool RemoveFromMemory(string cacheKey);
    }
}