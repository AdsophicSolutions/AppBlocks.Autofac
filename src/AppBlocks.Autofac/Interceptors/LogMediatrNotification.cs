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
        public Task Handle(TNotification notification, 
            CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                if (Logger.IsInfoEnabled)
                    Logger.Info($"Logging notification from {notification.GetType().FullName}");
            }
           );
        }

        public ILog Logger { get; set; }
    }
}
