using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace AppBlocks.Autofac.Common
{
    public class ApplicationConfiguration
    {
        private static readonly log4net.ILog logger =
                log4net.LogManager.GetLogger(typeof(ApplicationConfiguration));

        internal Lazy<IList<string>> ConfigurationFilePaths { get; } = new Lazy<IList<string>>(() => new List<string>());
        internal Lazy<IList<string>> AutofacDirectories { get; } = new Lazy<IList<string>>(() => new List<string>());
        internal Lazy<IList<string>> ExcludeFromLogTypes { get; } = new Lazy<IList<string>>(() => new List<string>());

        public ApplicationConfiguration(string configurationFilePath) 
            : this(new [] { configurationFilePath})
        {
            
        }

        /// <summary>
        /// Constructur to create ApplicationConfiguration object
        /// </summary>
        /// <param name="configurationFilePaths">IEnumerable list of configuration directories</param>
        public ApplicationConfiguration(IEnumerable<string> configurationFilePaths)
        {
            //List of directories cannot be null or empty
            if ((configurationFilePaths?.Count() ?? 0) == 0)
            {
                throw new ArgumentNullException("Argument configuration file paths cannot be null or empty");
            }

            //Add file path to list of file paths to process
            foreach (var configurationFilePath in configurationFilePaths)
            {
                ValidateAndAddConfigurationPath(configurationFilePath);
            }
        }

        /// <summary>
        /// Parameterless constructor to create Application Configuration
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
            if (string.IsNullOrEmpty(configurationFilePath)) 
                throw new ArgumentNullException("Configuration file path cannot be null or empty");

            //Add file path to list of file paths to process
            ValidateAndAddConfigurationPath(configurationFilePath);

        }

        public void GenerateConfiguration()
        {
            foreach (string configurationFilePath in ConfigurationFilePaths.Value)
            {
                var builder = new ConfigurationBuilder()
                    .AddJsonFile(configurationFilePath);

                var configuration = builder.Build();
                SetupAutofacSourceDirectories(configuration);
                SetupExcludeLogTypes(configuration);
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

            if (logger.IsDebugEnabled) logger.Debug($"Adding configuration file path {configurationFilePath}");

            //Add to list of directories to be processed
            ConfigurationFilePaths.Value.Add(configurationFilePath);
        }

        private void SetupAutofacSourceDirectories(IConfigurationRoot configurationRoot)
        {
            var autofacSourceDirectoriesConfiguration = configurationRoot
                .GetSection("autofacSourceDirectories")
                ?.GetChildren()
                .Select(d => d.Value)
                .ToArray();

            if (autofacSourceDirectoriesConfiguration == null) return;

            foreach (var autofacSourceDirectory in autofacSourceDirectoriesConfiguration)
            {
                if (!Directory.Exists(autofacSourceDirectory))
                {
                    throw new Exception($"Autofac source directory does not exist: {autofacSourceDirectory}");
                }

                if (logger.IsDebugEnabled) 
                    logger.Debug($"Adding Autofac source directory {autofacSourceDirectory}");

                AutofacDirectories.Value.Add(autofacSourceDirectory);
            }
        }

        private void SetupExcludeLogTypes(IConfigurationRoot configurationRoot)
        {
            var excludeFromLogTypes = configurationRoot
                .GetSection("excludeFromLog")
                ?.GetChildren()
                .Select(t => t.Value)
                .ToArray();

            if (excludeFromLogTypes == null) return;

            foreach (var excludeFromLogType in excludeFromLogTypes)
            {
                ExcludeFromLogTypes.Value.Add(excludeFromLogType);
            }
        }
    }
}
