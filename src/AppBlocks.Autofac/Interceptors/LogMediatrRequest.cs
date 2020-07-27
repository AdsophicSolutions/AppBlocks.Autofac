using log4net;
using MediatR.Pipeline;
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
        private static readonly ILog logger =
            LogManager.GetLogger(typeof(LogMediatrRequest<>).Assembly, "AppBlocks.Autofac.Interceptors.LogMediatrRequest");

        /// <summary>
        /// Process request
        /// </summary>
        /// <param name="request">Request Type</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> instance</param>
        /// <returns>Processing <see cref="Task"/></returns>
        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                if (logger.IsInfoEnabled)
                    logger.Info($"Logging request from {request?.GetType().FullName}. Request Details {request}");
            }
            );
        }

        //public ILog Logger { get; set; }
    }
}
