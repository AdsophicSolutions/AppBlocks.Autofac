using AppBlocks.Autofac.Support;
using log4net;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppBlocks.Autofac.Tests.MediatR
{
    [AppBlocksMediatrNotificationService]
    public class NotificationService : INotificationHandler<Notification>
    {   

        private static int callCount;

        public static int GetCallCount() => callCount;
        public static void ResetCount() => callCount = 0;

        private readonly ILogger<NotificationService> logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            this.logger = logger;
        }

        public Task Handle(Notification notification, 
            CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                if (notification.Message == "0")
                {
                    callCount++;

                    if (logger.IsEnabled(LogLevel.Information))
                        logger.LogInformation($"{nameof(NotificationService)}.{nameof(Handle)} Received message {notification.Message}");
                }
            });
        }
    }
}
