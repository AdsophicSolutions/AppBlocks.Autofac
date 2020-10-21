using AppBlocks.Autofac.Support;
using log4net;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AppBlocks.Autofac.Tests.Logging
{
    [AppBlocksService]
    public class LoggingTestService : ILoggingTestService
    {
        private readonly ILogger<LoggingTestService> logger;

        public LoggingTestService(ILogger<LoggingTestService> logger)
        {
            this.logger = logger;
        }

        public void Method1() => 
            logger.LogInformation($"{nameof(LoggingTestService)}.{nameof(Method1)} called successfully");
    }
}
