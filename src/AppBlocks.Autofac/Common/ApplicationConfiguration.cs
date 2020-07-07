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
        private static readonly log4net.ILog log =
                log4net.LogManager.GetLogger(typeof(ApplicationConfiguration));

        internal Lazy<IList<string>> ConfigurationFilePaths { get; } = new Lazy<IList<string>>(() => new List<string>());
        internal Lazy<IList<string>> AutofacDirectories { get; } = new Lazy<IList<string>>(() => new List<string>());
        internal Lazy<IList<string>> UseDefaultLoggerTypes { get; } = new Lazy<IList<string>>(() => new List<string>());

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

        public void AddLoggingConfiguration(string logFilePath)
        {
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead(logFilePath));

            var repo = log4net.LogManager.CreateRepository(
                "default",//Assembly.GetEntryAssembly(), 
                typeof(log4net.Repository.Hierarchy.Hierarchy));

            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

            log.Info("Application Configuration. Logging setup completed");
            //Lines removed for brevity
        }

        /// <summary>
        /// Add application config file path
        /// </summary>
        /// <param name="configurationFilePath">Configuration File Path to add</param>
        public void AddConfigurationFile(string configurationFilePath)
        {
            if (string.IsNullOrEmpty(configurationFilePath)) throw new ArgumentNullException("Configuration file path cannot be null or empty");

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
                SetupDefaultLoggerTypes(configuration);
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

            //Add to list of directories to be processed
            ConfigurationFilePaths.Value.Add(configurationFilePath);
        }

        private void SetupAutofacSourceDirectories(IConfigurationRoot configurationRoot)
        {
            var autofacSourceDirectoriesConfiguration = configurationRoot.GetSection("autofacSourceDirectories").GetChildren();
            foreach (var autofacSourceDirectoryConfiguration in autofacSourceDirectoriesConfiguration)
            {
                if (!Directory.Exists(autofacSourceDirectoryConfiguration["Directory"]))
                {
                    throw new Exception($"Autofac source directory does not exist: {autofacSourceDirectoryConfiguration["Directory"]}");
                }

                AutofacDirectories.Value.Add(autofacSourceDirectoryConfiguration["Directory"]);
            }
        }

        private void SetupDefaultLoggerTypes(IConfigurationRoot configurationRoot)
        {
            var useDefaultLoggerTypesConfiguration = configurationRoot.GetSection("useDefaultLogger").GetChildren();
            foreach (var defaultLoggerTypeConfiguration in useDefaultLoggerTypesConfiguration)
            {
                UseDefaultLoggerTypes.Value.Add(defaultLoggerTypeConfiguration["Type"]);
            }
        }
    }
}
