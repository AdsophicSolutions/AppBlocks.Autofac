using log4net;
using MediatR;
using MediatR.Pipeline;
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
        private static readonly ILog logger =
            LogManager.GetLogger(typeof(LogMediatrNotification<>).Assembly, "AppBlocks.Autofac.Interceptors.LogMediatrNotification");

        /// <summary>
        /// Handle notification
        /// </summary>
        /// <param name="notification">Notification to process</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> instance</param>
        /// <returns>Handling <see cref="Task"/></returns>
        public Task Handle(TNotification notification, 
            CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                if (logger.IsInfoEnabled)
                    logger.Info($"Logging notification from {notification?.GetType().FullName}. Notification details {notification}");
            }
           );
        }
    }
}
