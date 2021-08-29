using AppBlocks.Autofac.Common;
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
        private readonly ILoggingConfiguration loggingConfiguration;

        public LogMediatrResponse(ILogger<LogMediatrResponse<TRequest, TResponse>> logger,
            ILoggingConfiguration loggingConfiguration)
        {
            this.logger = logger;
            this.loggingConfiguration = loggingConfiguration;
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
                var typeName = response?.GetType().FullName;

                if (loggingConfiguration.IsTypeElevatedToWarn(typeName))
                {
                    if (logger.IsEnabled(LogLevel.Warning))
                        logger.LogWarning(
                            $"Logging response from {typeName}. Response details {response}");
                }
                // if type is elevated to info log as info
                else if (loggingConfiguration.IsTypeElevatedToInfo(typeName))
                {
                    if (logger.IsEnabled(LogLevel.Information))
                        logger.LogInformation(
                            $"Logging response from {typeName}. Response details {response}");
                }
                else if (logger.IsEnabled(LogLevel.Debug))
                    logger.LogDebug($"Logging response from {typeName}. Response details {response}");
                
            }, CancellationToken.None);
        }
    }
}
