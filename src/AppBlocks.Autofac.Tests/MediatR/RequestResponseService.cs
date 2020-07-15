using AppBlocks.Autofac.Support;
using log4net;
using MediatR;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppBlocks.Autofac.Tests.MediatR
{
    [AppBlocksMediatrRequestService]
    public class RequestResponseService : IRequestHandler<Request, Response>
    {
        private static readonly ILog logger =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static int callCount;

        public static int GetCallCount() => callCount;
        public static void ResetCount() => callCount = 0;

        public Task<Response> Handle(Request request, 
            CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                if(request.Input == "0")
                {
                    callCount++;

                    if (logger.IsInfoEnabled)
                        logger.Info($"{nameof(RequestResponseService)}.{nameof(Handle)} Received input {request.Input}");

                    return new Response { Output = "1" };
                }

                return new Response();
            });
        }
    }
}
