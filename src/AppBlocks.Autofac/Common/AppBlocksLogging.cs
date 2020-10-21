using AppBlocks.Autofac.Logging.Log4Net;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Common
{
    /// <summary>
    /// Configure logging for application
    /// </summary>
    public class AppBlocksLogging
    {
        private static readonly Lazy<AppBlocksLogging> instance =
            new Lazy<AppBlocksLogging>(() => new AppBlocksLogging());
        private ILoggerFactory loggerFactory;

        /// <summary>
        /// Constructor Private
        /// </summary>
        private AppBlocksLogging()
        {
        }

        /// <summary>
        /// Returns singleton instance
        /// </summary>
        public static AppBlocksLogging Instance => instance.Value;

        /// <summary>
        /// Initialize logging to use log4net
        /// </summary>
        /// <param name="log4NetConfigFile">Path to log4net configuration file</param>
        public void UseLog4Net(string log4NetConfigFile)
        {
            // Currently no support for multiple logging end points. 
            if (loggerFactory != null)
                throw new Exception("Logger factory is already initialized. AppBlocks does not currently support multiple logging destinations");

            // Intialize log4net logging
            loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddLog4Net(log4NetConfigFile);
            }
           );
        }

        /// <summary>
        /// Initialize logging to log to console. Not recommended for production environments
        /// </summary>
        public void UseDefault()
        {
            // Currently no support for multiple logging end points
            if (loggerFactory != null)
                throw new Exception("Logger factory is already initialized. AppBlocks does not currently support multiple logging destinations");

            // Initialize default logging to console
            loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            }
           );
        }

        /// <summary>
        /// Gets an intialized logger factory implementation
        /// </summary>
        /// <returns>Reference to <see cref="ILoggerFactory"/></returns>
        public ILoggerFactory GetLoggerFactory()
        {
            if (loggerFactory == null) UseDefault();

            return loggerFactory;
        }

        /// <summary>
        /// Sets logger factory implementation
        /// </summary>
        /// <param name="loggerFactory">Logger factory to use</param>
        public void SetLoggerFactory(ILoggerFactory loggerFactory)
        {
            if(this.loggerFactory != null)
                throw new Exception("Logger factory is already initialized. AppBlocks does not currently support multiple logging destinations");

            this.loggerFactory = loggerFactory;
        }
    }
}
