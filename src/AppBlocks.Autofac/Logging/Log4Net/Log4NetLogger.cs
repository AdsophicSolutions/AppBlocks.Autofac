using log4net;
using log4net.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace AppBlocks.Autofac.Logging.Log4Net
{
    
    /// <summary>
    /// Implementation for <see cref="Microsoft.Extensions.Logging.ILogger"/>
    /// </summary>
    internal class Log4NetLogger : Microsoft.Extensions.Logging.ILogger
    {   
        private readonly ILog log;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the logger to be created</param>
        internal Log4NetLogger(string name)
        {
            if (string.IsNullOrEmpty(name)) return;
            log = LogManager.GetLogger(name);
        }

        /// <summary>
        /// Implementation for <see cref="ILogger.BeginScope{TState}(TState)"/>
        /// </summary>
        /// <typeparam name="TState">State type</typeparam>
        /// <param name="state">Reference to state</param>
        /// <returns>null</returns>
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        /// <summary>
        /// Implementation for <see cref="ILogger.IsEnabled(LogLevel)"/>
        /// </summary>
        /// <param name="logLevel">LogLevel to check</param>
        /// <returns><c>true</c> if <see cref="LogLevel"/> is enabled; otherwise <c>false</c></returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            if (log == null) return false;

            switch (logLevel)
            {
                case LogLevel.Critical:
                    return log.IsFatalEnabled;
                case LogLevel.Debug:
                case LogLevel.Trace:
                    return log.IsDebugEnabled;
                case LogLevel.Error:
                    return log.IsErrorEnabled;
                case LogLevel.Information:
                    return log.IsInfoEnabled;
                case LogLevel.Warning:
                    return log.IsWarnEnabled;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        /// <summary>
        /// Log statement to log to log4net
        /// </summary>
        /// <typeparam name="TState">State type</typeparam>
        /// <param name="logLevel">Logging <see cref="LogLevel"/></param>
        /// <param name="eventId"><see cref="EventId"/> for this statement</param>
        /// <param name="state">Reference to state</param>
        /// <param name="exception"><see cref="Exception"/> if relevant. Does not apply to all log levels</param>
        /// <param name="formatter">Format function</param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            // Format message
            if (formatter == null) 
                throw new ArgumentNullException(nameof(formatter), "Formatter cannot be null");
            string message = formatter(state, null);

            // If event id name is not null. Add event id information to log
            if (eventId.Name != null) 
                message = $"{message} EventId: {eventId.Id}, Event Name: {eventId.Name}";
            else if(eventId.Id != 0)
                message = $"{message} EventId: {eventId.Id}";

            switch (logLevel)
            {
                case LogLevel.Critical:
                    log.Fatal(message);
                    break;
                case LogLevel.Debug:
                case LogLevel.Trace:
                    log.Debug(message);
                    break;
                case LogLevel.Error:
                    if (exception == null) log.Error(message); 
                    else log.Error(exception);
                    break;
                case LogLevel.Information:
                    log.Info(message);
                    break;
                case LogLevel.Warning:
                    log.Warn(message);
                    break;
                default:
                    log.Warn($"Encountered unknown log level {logLevel}, writing out as Info.");
                    if (exception == null) log.Info(message); else log.Info(message, exception);
                    break;
            }
        }
    }
}
