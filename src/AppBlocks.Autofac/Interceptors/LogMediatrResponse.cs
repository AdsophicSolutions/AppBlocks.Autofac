using log4net;
using MediatR.Pipeline;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppBlocks.Autofac.Interceptors
{
    /// <summary>
    /// Log MediatR response
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    internal class LogMediatrResponse<TRequest, TResponse> : 
        IRequestPostProcessor<TRequest, TResponse>
    {
        private static readonly ILog logger =
            LogManager.GetLogger(typeof(LogMediatrResponse<,>).Assembly, "AppBlocks.Autofac.Interceptors.LogMediatrResponse");

        /// <summary>
        /// Process response
        /// </summary>
        /// <param name="request">Request instance</param>
        /// <param name="response">Response instance</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> instance</param>
        /// <returns>Processing <see cref="Task"/></returns>
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
