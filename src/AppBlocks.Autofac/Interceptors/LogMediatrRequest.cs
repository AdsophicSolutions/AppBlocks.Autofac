using log4net;
using MediatR.Pipeline;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppBlocks.Autofac.Interceptors
{
    internal class LogMediatrRequest<TRequest> : IRequestPreProcessor<TRequest>
    {
        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                if (Logger.IsInfoEnabled)
                    Logger.Info($"Logging request from {request.GetType().FullName}");
            }
            );
        }

        public ILog Logger { get; set; }
    }
}
