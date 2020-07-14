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
        public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                if (Logger.IsInfoEnabled)
                    Logger.Info($"Logging response from {response.GetType().FullName}");
            }
            );
        }

        public ILog Logger { get; set; }
    }
}
