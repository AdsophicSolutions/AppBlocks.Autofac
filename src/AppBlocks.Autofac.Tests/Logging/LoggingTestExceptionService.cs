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
    public class LoggingTestExceptionService : ILoggingTestExceptionService
    {
        private static int callCount = 0;
        public static int GetCallCount() => callCount;

        private readonly ILogger<LoggingTestExceptionService> logger;

        public LoggingTestExceptionService(ILogger<LoggingTestExceptionService> logger)
        {
            this.logger = logger;
        }

        internal static void ResetCount()
        {   callCount = 0;
        }

        public void Method1()
        {
            callCount++;

            logger.LogInformation($"{nameof(LoggingTestExceptionService)}.{nameof(Method1)} called successfully");
        }
    }
}
