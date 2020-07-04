using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Common
{
    /// <summary>
    /// Interface provides abstraction for data caching. 
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Add item to cache
        /// </summary>
        /// <param name="key">Item Key</param>
        /// <param name="expirationType">Absolute or Relative</param>
        /// <param name="expiration">expiration value</param>
        /// <param name="keepValueOnRetrieveFail">keep current cache value if cache retrieval fails</param>
        /// <param name="retrieveFunction"></param>
        void AddItem(string key, EnumCacheExpirationType expirationType, TimeSpan expiration, bool keepValueOnRetrieveFail, Func<object> retrieveFunction);

        /// <summary>
        /// Get Item from cache
        /// </summary>
        /// <typeparam name="T">type of item returned</typeparam>
        /// <param name="key">Item key</param>
        /// <returns>cache item if found, null otherwise</returns>
        T GetItem<T>(string key);

        /// <summary>
        /// Get Item from cache
        /// </summary>
        /// <param name="key">Item key</param>
        /// <returns>cache item if found, null otherwise</returns>
        object GetItem(string key);

        /// <summary>
        /// Remove item from cache. Call method to force cache value refresh
        /// </summary>
        /// <param name="key">Item Key</param>
        /// <returns>true if item was removed, false if item does not exist in cache</returns>
        bool RemoveItem(string key);
    }

    public enum EnumCacheExpirationType
    {
        Absolute,
        Relative
    }
}
