using AppBlocks.Autofac.Common;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;

namespace AppBlocks.Autofac.Services
{
    /// <summary>
    /// InMemoryCache is a wrapper around MemoryCache to simplify creation and maintenance 
    /// of in memory cache. 
    /// </summary>
    internal sealed class InMemoryCacheService : ICacheService
    {
        private ConcurrentDictionary<string, CacheStore> cacheDictionary = new ConcurrentDictionary<string, CacheStore>();
        private MemoryCache MemoryCache { get; }

        public InMemoryCacheService()
        {
            // Initialize MemoryCache
            MemoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        /// <summary>
        /// Add cache item
        /// </summary>
        /// <param name="key">Cache item key</param>
        /// <param name="expirationType"><see cref="CacheExpirationType"/> instance</param>
        /// <param name="expiration">Expiratation <see cref="TimeSpan"/></param>
        /// <param name="keepValueOnRetrieveFail">Specifies if value should be retained if cache retrieval fails</param>
        /// <param name="retrieveFunction">Function used to retrieve cache value</param>
        public void AddItem(string key,
            CacheExpirationType expirationType,
            TimeSpan expiration,
            bool keepValueOnRetrieveFail,
            Func<object> retrieveFunction)
        {
            InitCacheItem(key, new CacheStore
            {
                CacheExpirationType = expirationType,
                Expiration = expiration,
                KeepValueOnRetrieveFail = keepValueOnRetrieveFail,
                RetrieveFunction = retrieveFunction,
                CacheValue = new Lazy<object>(() => retrieveFunction())
            });
        }

        /// <summary>
        /// Get item from cache
        /// </summary>
        /// <typeparam name="T">Item Type</typeparam>
        /// <param name="key">Cache key to retrieve</param>
        /// <returns></returns>
        public T GetItem<T>(string key)
        {
            //key is in memory cache. return value 
            if (MemoryCache.TryGetValue(key, out CacheStore cacheStore))
            {
                try
                {
                    return (T)cacheStore.CacheValue.Value;
                }
                catch
                {
                    if (cacheStore.KeepValueOnRetrieveFail) 
                        return (T)cacheStore.PreviousCacheValue;
                    throw;
                }
            }
            //cache evicted. Lets refresh cache
            else if (cacheDictionary.TryGetValue(key, out cacheStore))
            {
                try
                {
                    cacheStore.PreviousCacheValue = cacheStore.CacheValue.Value;
                    cacheStore.CacheValue = new Lazy<object>(() => cacheStore.RetrieveFunction());
                    object newCacheValue = (T)cacheStore.CacheValue.Value;
                    InitCacheItem(key, cacheStore);
                    return (T)newCacheValue;
                }
                catch
                {
                    if (cacheStore.KeepValueOnRetrieveFail) return (T)cacheStore.PreviousCacheValue;
                    throw;
                }
            }

            return default;
        }

        public object GetItem(string key)
        {
            return GetItem<object>(key);
        }

        public bool RemoveItem(string key)
        {
            if (MemoryCache.TryGetValue(key, out object cacheStore))
            {
                MemoryCache.Remove(key);
                return true;
            }

            return false;
        }

        private void InitCacheItem(
            string key,
            CacheStore cacheStore)
        {
            var memoryCacheEntryOptions = new MemoryCacheEntryOptions();
            if (cacheStore.CacheExpirationType == CacheExpirationType.Absolute)
                memoryCacheEntryOptions.SetAbsoluteExpiration(cacheStore.Expiration);
            else
                memoryCacheEntryOptions.SetSlidingExpiration(cacheStore.Expiration);

            MemoryCache.Set(key, cacheStore, memoryCacheEntryOptions);
            cacheDictionary[key] = cacheStore;
        }

        private class CacheStore
        {
            public Func<object> RetrieveFunction { get; set; }
            public Lazy<object> CacheValue { get; set; }
            public object PreviousCacheValue { get; set; }
            public bool KeepValueOnRetrieveFail { get; set; }
            public CacheExpirationType CacheExpirationType { get; set; }
            public TimeSpan Expiration { get; set; }
        }
    }
}
