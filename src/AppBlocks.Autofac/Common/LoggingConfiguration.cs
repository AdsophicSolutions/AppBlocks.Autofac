using System;
using System.Collections.Generic;

namespace AppBlocks.Autofac.Common
{
    internal class LoggingConfiguration : ILoggingConfiguration
    {
        private readonly ApplicationConfiguration configuration;
        private readonly Lazy<HashSet<string>> excludeFromLogTypes 
            = new Lazy<HashSet<string>>(() => new HashSet<string>());

        public LoggingConfiguration(ApplicationConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public bool IsTypeExcluded(string fullTypeName)
        {
            if (!excludeFromLogTypes.IsValueCreated)
            {
                foreach (string excludeFromLogType in configuration?.ExcludeFromLogTypes.Value)
                {
                    excludeFromLogTypes.Value.Add(excludeFromLogType);
                }
            }

            return excludeFromLogTypes.Value.Contains(fullTypeName);
        }
    }
}
