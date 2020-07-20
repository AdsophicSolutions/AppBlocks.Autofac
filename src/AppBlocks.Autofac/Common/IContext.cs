namespace AppBlocks.Autofac.Common
{
    /// <summary>
    /// AppBlocks application supports creation of application context that 
    /// implements the <see cref="IContext"/> interface. <see cref="IContext"/>
    /// is a global service to be used to maintain application level 
    /// shared key / value pairs
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Determines if <see cref="IContext"/> contains the specified key
        /// </summary>
        /// <param name="key">Key to search</param>
        /// <returns>
        /// <c>true</c> if <see cref="IContext"/> contains the specified key, <c>false</c> otherwise
        /// </returns>
        bool ContainsKey(string key);       

        /// <summary>
        /// Get the value for a specified key from <see cref="IContext"/>
        /// </summary>
        /// <param name="key">Key to search</param>
        /// <param name="value"><c>true</c> if <see cref="IContext"/> contains an element with the specified key; otherwise, <c>false</c>.</returns>
        bool TryGetValue(string key, out object value);

        /// <summary>
        /// Gets or Sets value associated with the specified key from <see cref="IContext"/>
        /// </summary>
        /// <param name="key">Key to search</param>
        /// <returns>The key value associated with the specified key. If the specified key is not found, a get operation throws a 
        /// <see cref="System.Collections.Generic.KeyNotFoundException"/>. Set operation creates a new element with the specified key</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"/>
        object this[string key] { get; set; }
    }
}
