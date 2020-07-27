using AppBlocks.Autofac.Common;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace AppBlocks.Autofac.Services
{
    /// <summary>
    /// Default implementation for <see cref="IContext"/> based on application configuration 
    /// file
    /// </summary>
    internal sealed class ApplicationContextService : IContext
    {
        private readonly Dictionary<string, object> context =
            new Dictionary<string, object>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"><see cref="ApplicationConfiguration"/> instance</param>
        public ApplicationContextService(ApplicationConfiguration configuration)
        {
            Initialize(configuration);
        }

        /// <summary>
        /// Initialize application configuration
        /// </summary>
        /// <param name="configuration"><see cref="ApplicationConfiguration"/> instance</param>
        private void Initialize(ApplicationConfiguration configuration)
        {
            // Iterate through configuration file paths
            foreach (string configurationFilePath in 
                configuration.ConfigurationFilePaths.Value)
            {
                // Create Configuration builder for parsing Json
                var builder = new ConfigurationBuilder()
                    .AddJsonFile(configurationFilePath);

                // Build configuration
                var jsonConfiguration = builder.Build();

                // Look for appContext section and initialize 
                // context dictionary
                var appContext = jsonConfiguration
                        .GetSection("appContext")
                        ?.GetChildren()
                        ?.ToDictionary(x => x.Key, x => x.Value);

                // Initialize context if appContext is found
                if(appContext != null)
                {
                    // Initialize using key/value pair
                    foreach(var kvp in appContext)
                    {
                        // Add key/value pair to context
                        if (!context.ContainsKey(kvp.Key)) 
                            context.Add(kvp.Key, kvp.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or Sets value associated with the specified key from <see cref="IContext"/>
        /// </summary>
        /// <param name="key">Key to search</param>
        /// <returns>The key value associated with the specified key. If the specified key is not found, a get operation throws a 
        /// <see cref="System.Collections.Generic.KeyNotFoundException"/>. Set operation creates a new element with the specified key</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"/>
        public object this[string key] 
        { 
            get => context[key]; 
            set => context[key] = value; 
        }

        /// <summary>
        /// Determines if <see cref="IContext"/> contains the specified key
        /// </summary>
        /// <param name="key">Key to search</param>
        /// <returns>
        /// <c>true</c> if <see cref="IContext"/> contains the specified key, <c>false</c> otherwise
        /// </returns>
        public bool ContainsKey(string key) => context.ContainsKey(key);

        /// <summary>
        /// Get the value for a specified key from <see cref="IContext"/>
        /// </summary>
        /// <param name="key">Key to search</param>
        /// <param name="value"><c>true</c> if <see cref="IContext"/> contains an element with the specified key; otherwise, <c>false</c>.</returns>
        public bool TryGetValue(string key, out object value) => 
            context.TryGetValue(key, out value);
    }
}
