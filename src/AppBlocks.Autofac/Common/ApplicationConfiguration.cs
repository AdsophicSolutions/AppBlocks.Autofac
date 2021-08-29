using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace AppBlocks.Autofac.Common
{
    /// <summary>
    /// <see cref="ApplicationConfiguration"/> provides configuration details a 
    /// AppBlocks application
    /// </summary>
    public class ApplicationConfiguration
    {
        private readonly ILogger<ApplicationConfiguration> logger;

        internal Lazy<IList<string>> ConfigurationFilePaths { get; } = new Lazy<IList<string>>(() => new List<string>());
        internal Lazy<IList<string>> AutofacDirectories { get; } = new Lazy<IList<string>>(() => new List<string>());
        internal Lazy<IList<string>> ExcludeFromLogTypes { get; } = new Lazy<IList<string>>(() => new List<string>());
        internal Lazy<IList<string>> ElevateToInfoLogTypes { get; } = new Lazy<IList<string>>(() => new List<string>());
        internal Lazy<IList<string>> ElevateToWarnLogTypes { get; } = new Lazy<IList<string>>(() => new List<string>());

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configurationFilePath"><see cref="string"/> path to configuration json file</param>
        public ApplicationConfiguration(string configurationFilePath) 
            : this(new [] { configurationFilePath})
        {            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configurationFilePaths">IEnumerable list of configuration directories</param>
        public ApplicationConfiguration(IEnumerable<string> configurationFilePaths)
        {
            logger = new Logger<ApplicationConfiguration>(AppBlocksLogging.Instance.GetLoggerFactory());

            //List of directories cannot be null or empty
            if ((configurationFilePaths?.Count() ?? 0) == 0)
            {
                throw new ArgumentNullException(
                    paramName: nameof(configurationFilePaths),
                    message: "Argument configuration file paths cannot be null or empty");
            }

            //Add file path to list of file paths to process
            foreach (var configurationFilePath in configurationFilePaths)
            {
                ValidateAndAddConfigurationPath(configurationFilePath);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ApplicationConfiguration()
        {
        }

        /// <summary>
        /// Add application config file path
        /// </summary>
        /// <param name="configurationFilePath">Configuration File Path to add</param>
        public void AddConfigurationFile(string configurationFilePath)
        {
            // Perform a null check
            if (string.IsNullOrEmpty(configurationFilePath)) 
                throw new ArgumentNullException(
                    paramName: nameof(configurationFilePath),
                    message: "Configuration file path cannot be null or empty");

            //Add file path to list of file paths to process
            ValidateAndAddConfigurationPath(configurationFilePath);

        }

        /// <summary>
        /// Generates configuration based on configuration file paths
        /// </summary>
        public void GenerateConfiguration()
        {
            // Iterate through configuration file paths
            foreach (string configurationFilePath in ConfigurationFilePaths.Value)
            {
                // Add file to ConfigurationBuilder
                var builder = new ConfigurationBuilder()
                    .AddJsonFile(configurationFilePath);

                var configuration = builder.Build();

                // Add source directories
                AddAutofacSourceDirectories(configuration);

                // Add types to be excluded from logging
                AddExcludeLogTypes(configuration);

                // Add types to be elevate to info instead of default debug for logging
                AddElevateToLogTypes(configuration);

                // Add types to be elevate to warn instead of default debug for logging
                // Can be an effective tool in production where you can set certain services
                // to log warn level for debugging
                AddElevateToWarnTypes(configuration);
            }          
        }

        private void ValidateAndAddConfigurationPath(string configurationFilePath)
        {
            //Confirm file exists and is accessible
            if (!File.Exists(configurationFilePath))
            {
                throw new ArgumentException($"File {configurationFilePath} is not exist or not accessible. " +
                    $"All configuration file paths passed to {GetType().FullName} must exist and be accessible");
            }

            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug($"Adding configuration file path {configurationFilePath}");

            //Add to list of directories to be processed
            ConfigurationFilePaths.Value.Add(configurationFilePath);
        }

        private void AddAutofacSourceDirectories(IConfigurationRoot configurationRoot)
        {
            // Read configuration file section and create array of directories
            var autofacSourceDirectoriesConfiguration = configurationRoot
                .GetSection("autofacSourceDirectories")
                ?.GetChildren()
                .Select(d => d.Value)
                .ToArray();

            // Return if section is not found
            if (autofacSourceDirectoriesConfiguration == null) return;

            // Make sure source directories exist. 
            foreach (var autofacSourceDirectory in autofacSourceDirectoriesConfiguration)
            {
                // check if directory exists
                if (!Directory.Exists(autofacSourceDirectory))
                    throw new Exception($"Autofac source directory does not exist: {autofacSourceDirectory}");

                if (logger.IsEnabled(LogLevel.Debug))
                    logger.LogDebug($"Adding Autofac source directory {autofacSourceDirectory}");

                // Add directory to the list of source directories
                AutofacDirectories.Value.Add(autofacSourceDirectory);
            }
        }

        private void AddExcludeLogTypes(IConfigurationRoot configurationRoot)
        {
            // Read types to be excluded from logging. 
            var excludeFromLogTypes = configurationRoot
                .GetSection("excludeFromLog")
                ?.GetChildren()
                .Select(t => t.Value)
                .ToArray();

            // return if no entries are found
            if (excludeFromLogTypes == null) return;

            // Add types to the list of directories. 
            foreach (var excludeFromLogType in excludeFromLogTypes)
                ExcludeFromLogTypes.Value.Add(excludeFromLogType);
        }

        private void AddElevateToLogTypes(IConfigurationRoot configurationRoot)
        {
            // Read types to be elevated to info level logging. 
            var elevateToInfoLogTypes = configurationRoot
                .GetSection("elevateToInfoLog")
                ?.GetChildren()
                .Select(t => t.Value)
                .ToArray();

            // return if no entries are found
            if (elevateToInfoLogTypes == null) return;

            // Add types to elevate to info list
            foreach (var elevateToInfoLogType in elevateToInfoLogTypes)
                ElevateToInfoLogTypes.Value.Add(elevateToInfoLogType);
        }

        private void AddElevateToWarnTypes(IConfigurationRoot configurationRoot)
        {
            // Read types to be elevated to warn level logging. 
            var elevateToWarnLogTypes = configurationRoot
                .GetSection("elevateToWarnLog")
                ?.GetChildren()
                .Select(t => t.Value)
                .ToArray();

            // return if no entries are found
            if (elevateToWarnLogTypes == null) return;

            // Add types to elevate to warn list
            foreach (var elevateToWarnLogType in elevateToWarnLogTypes)
                ElevateToWarnLogTypes.Value.Add(elevateToWarnLogType);
        }
    }
}
