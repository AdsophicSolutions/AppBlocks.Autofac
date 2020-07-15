using log4net;
using MediatR.Pipeline;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppBlocks.Autofac.Interceptors
{
    internal class LogMediatrResponse<TRequest, TResponse> : 
        IRequestPostProcessor<TRequest, TResponse>
    {
        private static readonly ILog logger =
            LogManager.GetLogger(typeof(LogMediatrResponse<,>).Assembly, "AppBlocks.Autofac.Interceptors.LogMediatrResponse");
        public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                if (logger.IsInfoEnabled)
                    logger.Info($"Logging response from {response?.GetType().FullName}. Response details {response}");
            }
            );
        }
    }
}
