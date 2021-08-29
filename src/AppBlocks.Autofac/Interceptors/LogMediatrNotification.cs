using AppBlocks.Autofac.Common;
using log4net;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppBlocks.Autofac.Interceptors
{
    /// <summary>
    /// Logs MediatR service notifications 
    /// </summary>
    /// <typeparam name="TNotification">Notification Type</typeparam>
    internal class LogMediatrNotification<TNotification> : 
        INotificationHandler<TNotification> where TNotification : INotification
    {
        private readonly ILogger<LogMediatrNotification<TNotification>> logger;
        private readonly ILoggingConfiguration loggingConfiguration;

        public LogMediatrNotification(
            ILogger<LogMediatrNotification<TNotification>> logger,
            ILoggingConfiguration loggingConfiguration)
        {
            this.logger = logger;
            this.loggingConfiguration = loggingConfiguration;
        }

        /// <summary>
        /// Handle notification
        /// </summary>
        /// <param name="notification">Notification to process</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> instance</param>
        /// <returns>Handling <see cref="Task"/></returns>
        public Task Handle(TNotification notification, 
            CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var typeName = notification?.GetType().FullName;

                if (loggingConfiguration.IsTypeElevatedToWarn(typeName))
                {
                    if (logger.IsEnabled(LogLevel.Warning))
                        logger.LogWarning(
                            $"Logging notification from {typeName}. Notification details {notification}");
                }
                // if type is elevated to info log as info
                else if (loggingConfiguration.IsTypeElevatedToInfo(typeName))
                {
                    if (logger.IsEnabled(LogLevel.Information))
                        logger.LogInformation(
                            $"Logging notification from {typeName}. Notification details {notification}");
                }
                else if (logger.IsEnabled(LogLevel.Debug))
                    logger.LogDebug($"Logging notification from {typeName}. Notification details {notification}");
            }, CancellationToken.None);
        }
    }
}
