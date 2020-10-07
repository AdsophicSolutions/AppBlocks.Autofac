using AppBlocks.Autofac.Support;
using log4net;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AppBlocks.Autofac.Tests.KeyedAndNamedServices
{
    [AppBlocksKeyedService("KeyedService2", typeof(IKeyedService))]
    public class KeyedService2 : IKeyedService
    {
        private readonly ILogger<KeyedService2> logger;

        public KeyedService2(ILogger<KeyedService2> logger)
        {
            this.logger = logger;
        }

        private static int callCount;
        public static int GetCallCount() => callCount;
        public static void ResetCount() => callCount = 0;

        public void RunKeyedService()
        {
            callCount++;

            logger.LogInformation($"{nameof(KeyedService2)}.{nameof(RunKeyedService)} called successfully");
        }
    }
}
