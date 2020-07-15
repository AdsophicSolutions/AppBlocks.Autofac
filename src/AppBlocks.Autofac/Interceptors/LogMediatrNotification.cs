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
    internal class LogMediatrNotification<TNotification> : 
        INotificationHandler<TNotification> where TNotification : INotification
    {
        private static readonly ILog logger =
            LogManager.GetLogger(typeof(LogMediatrNotification<>).Assembly, "AppBlocks.Autofac.Interceptors.LogMediatrNotification");

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
