using AppBlocks.Autofac.Support;
using log4net;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AppBlocks.Autofac.Tests
{
    [AppBlocksService]
    public class Service : IService
    {
        private static int callCount;
        public static int GetCallCount() => callCount;
        public static void ResetCount() => callCount = 0;

        private readonly ILogger<Service> logger;

        public Service(ILogger<Service> logger)
        {
            this.logger = logger;
        }

        public int RunService()
        {
            callCount++;

            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation($"{nameof(Service)}.{nameof(RunService)} called successfully");

            return 0;
        }
    }
}
