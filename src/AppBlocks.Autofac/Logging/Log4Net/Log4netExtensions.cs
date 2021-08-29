using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Logging.Log4Net
{
    /// <summary>
    /// Extensions for <see cref="ILoggerFactory"/> to support 
    /// adding log4net to Microsoft logging framework
    /// </summary>
    public static class Log4netExtensions
    {
        /// <summary>
        /// Add log4net logging to Microsoft logging framework
        /// </summary>
        /// <param name="factory">Reference to <see cref="ILoggerFactory"/></param>
        /// <param name="log4NetConfigFile">Path to log4net configuration file</param>
        /// <returns><see cref="ILoggerFactory"/> reference</returns>
        public static ILoggerFactory AddLog4Net(this ILoggerFactory factory, string log4NetConfigFile)
        {
            if (factory == null) 
                throw new ArgumentNullException(nameof(factory), "Factory argument cannot be null");

            using (var provider = new Log4NetProvider(log4NetConfigFile))
            {
                factory.AddProvider(provider);
            }
            return factory;
        }

        /// <summary>
        /// Add log4net logging to Microsoft logging framework. log4net configuration 
        /// file is defaulted to log4net.config in the output directory
        /// </summary>
        /// <param name="factory">Reference to <see cref="ILoggerFactory"/></param>
        /// <returns><see cref="ILoggerFactory"/> reference</returns>
        public static ILoggerFactory AddLog4Net(this ILoggerFactory factory)
            => AddLog4Net(factory, "log4net.config");

        /// <summary>
        /// Add log4net logging to Microsoft logging framework
        /// </summary>
        /// <param name="builder">Reference to <see cref="ILoggingBuilder"/></param>
        /// <param name="log4NetConfigFile">Path to log4net configuration file</param>
        /// <returns>Reference to <see cref="ILoggingBuilder"/></returns>
        public static ILoggingBuilder AddLog4Net(this ILoggingBuilder builder, string log4NetConfigFile)
        {
            using (var provider = new Log4NetProvider(log4NetConfigFile))
            {
                builder.AddProvider(provider);
            }
            return builder;
        }

        /// <summary>
        /// Add log4net logging to Microsoft logging framework. log4net configuration 
        /// file is defaulted to log4net.config in the output directory
        /// </summary>
        /// <param name="builder">Reference to <see cref="ILoggingBuilder"/></param>        
        /// <returns>Reference to <see cref="ILoggingBuilder"/></returns>
        public static ILoggingBuilder AddLog4Net(this ILoggingBuilder builder)
            => AddLog4Net(builder, "log4net.config");
    }
}
