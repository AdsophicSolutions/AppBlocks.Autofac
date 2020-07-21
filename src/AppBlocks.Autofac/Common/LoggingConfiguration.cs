using System;
using System.Collections.Generic;

namespace AppBlocks.Autofac.Common
{
    /// <summary>
    /// Default implementation for <see cref="ILoggingConfiguration"/>
    /// </summary>
    internal class LoggingConfiguration : ILoggingConfiguration
    {
        private readonly ApplicationConfiguration configuration;
        private readonly Lazy<HashSet<string>> excludeFromLogTypes 
            = new Lazy<HashSet<string>>(() => new HashSet<string>());

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Instance of <see cref="ApplicationConfiguration"/>
        /// for this AppBlocks assembly</param>
        public LoggingConfiguration(ApplicationConfiguration configuration)
        {
            this.configuration = configuration;
        }


        /// <summary>
        /// Evaluates if logging should be performed
        /// </summary>
        /// <param name="fullTypeName">Full name of type to check</param>
        /// <returns><c>true</c> if type should be excluded; otherwise <c>false</c>;</returns>
        public bool IsTypeExcluded(string fullTypeName)
        {
            // Initialize log type dictionary if not already created
            if (!excludeFromLogTypes.IsValueCreated)
            {
                // Iterate through exclude log types configuration from ApplicationConfiguration
                foreach (string excludeFromLogType in configuration?.ExcludeFromLogTypes.Value)
                {
                    // Add to exclusion set
                    excludeFromLogTypes.Value.Add(excludeFromLogType);
                }
            }

            // Chek if type name should be excluded
            return excludeFromLogTypes.Value.Contains(fullTypeName);
        }
    }
}
