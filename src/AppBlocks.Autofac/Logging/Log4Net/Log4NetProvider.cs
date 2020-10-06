using log4net;
using log4net.Config;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace AppBlocks.Autofac.Logging.Log4Net
{
    /// <summary>
    /// Log4Net provider
    /// </summary>
    internal class Log4NetProvider : ILoggerProvider
    {   
        private readonly ConcurrentDictionary<string, Log4NetLogger> loggers =
            new ConcurrentDictionary<string, Log4NetLogger>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="log4NetConfigFile">Path to log4net configuration file</param>
        internal Log4NetProvider(string log4NetConfigFile)
        {
            //var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            var logRepository = LogManager.GetRepository();
            XmlConfigurator.Configure(logRepository, new FileInfo(log4NetConfigFile));
        }

        /// <summary>
        /// Implementation for <see cref="ILoggerProvider.CreateLogger(string)"/>
        /// </summary>
        /// <param name="loggerName">Logger Name</param>
        /// <returns><see cref="ILogger"/> instance</returns>
        public ILogger CreateLogger(string loggerName) => 
            loggers.GetOrAdd(loggerName, CreateLoggerImplementation);

        public void Dispose() => loggers.Clear();

        private Log4NetLogger CreateLoggerImplementation(string name) => new Log4NetLogger(name); 
    }
}
