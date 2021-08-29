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
        private readonly Lazy<HashSet<string>> elevateToInfoLogTypes
            = new Lazy<HashSet<string>>(() => new HashSet<string>());
        private readonly Lazy<HashSet<string>> elevateToWarnLogTypes
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

        /// <summary>
        /// Implementation returns for types that should be elevated to Info level
        /// logging from default log leval of Debug
        /// </summary>
        /// <param name="fullTypeName">Full name for type to check</param>
        /// <returns><c>true</c> if type is elevated; otherwise <c>false</c>.</returns>
        public bool IsTypeElevatedToInfo(string fullTypeName)
        {
            // Initialize info type dictionary if not already created
            if (!elevateToInfoLogTypes.IsValueCreated)
            {
                // Iterate through elevated to info log types configuration from ApplicationConfiguration
                foreach (string elevatedToInfoLogType in configuration?.ElevateToInfoLogTypes.Value)
                {
                    // Add to exclusion set
                    elevateToInfoLogTypes.Value.Add(elevatedToInfoLogType);
                }
            }

            // Check if type name should be elevated to info
            return elevateToInfoLogTypes.Value.Contains(fullTypeName);
        }

        /// <summary>
        /// Implementation returns for types that should be elevated to Warn level
        /// logging from default log leval of Debug
        /// </summary>
        /// <param name="fullTypeName">Full name for type to check</param>
        /// <returns><c>true</c> if type is elevated; otherwise <c>false</c>.</returns>
        public bool IsTypeElevatedToWarn(string fullTypeName)
        {
            // Initialize warn type dictionary if not already created
            if (!elevateToWarnLogTypes.IsValueCreated)
            {
                // Iterate through elevated to warn log types configuration from ApplicationConfiguration
                foreach (string elevatedToWarnLogType in configuration?.ElevateToWarnLogTypes.Value)
                {
                    // Add to exclusion set
                    elevateToWarnLogTypes.Value.Add(elevatedToWarnLogType);
                }
            }

            // Check if type name should be elevated to warn
            return elevateToWarnLogTypes.Value.Contains(fullTypeName);
        }
    }
}
