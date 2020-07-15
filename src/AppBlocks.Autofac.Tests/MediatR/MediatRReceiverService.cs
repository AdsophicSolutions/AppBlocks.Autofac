using AppBlocks.Autofac.Support;
using log4net;
using MediatR;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AppBlocks.Autofac.Tests.MediatR
{
    [AppBlocksService]
    public class MediatRReceiverService : IMediatRReceiverService
    {
        private static readonly ILog logger =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static int callCount;

        public static int GetCallCount() => callCount;
        public static void ResetCount() => callCount = 0;

        private IMediator Mediator { get; }

        public MediatRReceiverService(IMediator mediator)
        {
            Mediator = mediator;
        }

        public void RunRequest()
        {
            var request = new Request { Input = "0" };
            var response = Mediator
                    .Send(request)
                    .GetAwaiter()
                    .GetResult();

            if(response.Output == "1")
            {
                callCount++;
                if (logger.IsInfoEnabled)
                    logger.Info($"Received response in " +
                        $"{nameof(MediatRReceiverService)}.{nameof(RunRequest)}");
            }
        }

        public void RunNotification()
        {
            var notification = new Notification { Message = "0" };

            callCount++;
            if (logger.IsInfoEnabled)
                logger.Info($"Publishing notification in " +
                    $"{nameof(MediatRReceiverService)}.{nameof(RunNotification)}");

            Mediator.Publish(notification).GetAwaiter().GetResult();
        }
    }
}
