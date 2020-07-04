using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Common
{
    internal class LoggingConfiguration : ILoggingConfiguration
    {
        private readonly ApplicationConfiguration configuration;
        private readonly Lazy<HashSet<string>> useDefaultLogTypes = new Lazy<HashSet<string>>(() => new HashSet<string>());

        public LoggingConfiguration(ApplicationConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public bool ShouldUseDefaultLogger(string fullTypeName)
        {
            if (!useDefaultLogTypes.IsValueCreated)
            {
                foreach (string useDefaultLoggerType in configuration.UseDefaultLoggerTypes)
                {
                    useDefaultLogTypes.Value.Add(useDefaultLoggerType);
                }
            }

            return useDefaultLogTypes.Value.Contains(fullTypeName);
        }
    }
}
