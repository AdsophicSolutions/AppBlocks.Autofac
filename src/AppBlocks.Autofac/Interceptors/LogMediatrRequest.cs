using log4net;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppBlocks.Autofac.Interceptors
{
    /// <summary>
    /// Logs MediatR service requests
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    internal class LogMediatrRequest<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly ILogger<LogMediatrRequest<TRequest>> logger;

        public LogMediatrRequest(ILogger<LogMediatrRequest<TRequest>> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Process request
        /// </summary>
        /// <param name="request">Request Type</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> instance</param>
        /// <returns>Processing <see cref="Task"/></returns>
        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                if (logger.IsEnabled(LogLevel.Information))
                    logger.LogInformation($"Logging request from {request?.GetType().FullName}. Request Details {request}");

            }, CancellationToken.None);
        }
    }
}
