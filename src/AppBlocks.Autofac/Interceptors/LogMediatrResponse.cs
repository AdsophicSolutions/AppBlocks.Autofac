using log4net;
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
    /// Log MediatR response
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    internal class LogMediatrResponse<TRequest, TResponse> : 
        IRequestPostProcessor<TRequest, TResponse>
    {
        private readonly ILogger<LogMediatrResponse<TRequest, TResponse>> logger;

        public LogMediatrResponse(ILogger<LogMediatrResponse<TRequest, TResponse>> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Process response
        /// </summary>
        /// <param name="request">Request instance</param>
        /// <param name="response">Response instance</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> instance</param>
        /// <returns>Processing <see cref="Task"/></returns>
        public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                if (logger.IsEnabled(LogLevel.Information))
                    logger.LogInformation($"Logging response from {response?.GetType().FullName}. Response details {response}");
            }, CancellationToken.None);
        }
    }
}
