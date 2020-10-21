using AppBlocks.Autofac.Support;
using log4net;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AppBlocks.Autofac.Tests.KeyedAndNamedServices
{
    [AppBlocksNamedService("AppBlocks.Autofac.Tests.KeyedAndNamedServices.NamedService")]
    public class NamedService : INamedService
    {
        private static int callCount;
        public static int GetCallCount() => callCount;
        public static void ResetCount() => callCount = 0;

        private readonly ILogger<NamedService> logger;

        public NamedService(ILogger<NamedService> logger)
        {
            this.logger = logger;
        }

        public void RunService()
        {
            callCount++;

            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation($"{nameof(NamedService)}.{nameof(RunService)} called successfully");
        }
    }
}
