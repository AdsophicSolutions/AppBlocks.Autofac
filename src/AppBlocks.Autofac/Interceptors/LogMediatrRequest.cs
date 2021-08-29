using AppBlocks.Autofac.Common;
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
        private readonly ILoggingConfiguration loggingConfiguration;

        public LogMediatrRequest(ILogger<LogMediatrRequest<TRequest>> logger,
            ILoggingConfiguration loggingConfiguration)
        {
            this.logger = logger;
            this.loggingConfiguration = loggingConfiguration;
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
                var typeName = request?.GetType().FullName;

                if (loggingConfiguration.IsTypeElevatedToWarn(typeName))
                {
                    if (logger.IsEnabled(LogLevel.Warning))
                        logger.LogWarning(
                            $"Logging request from {typeName}. Request details {request}");
                }
                // if type is elevated to info log as info
                else if (loggingConfiguration.IsTypeElevatedToInfo(typeName))
                {
                    if (logger.IsEnabled(LogLevel.Information))
                        logger.LogInformation(
                            $"Logging request from {typeName}. Request details {request}");
                }
                else if (logger.IsEnabled(LogLevel.Debug))
                    logger.LogDebug($"Logging request from {typeName}. Request details {request}");

            }, CancellationToken.None);
        }
    }
}
