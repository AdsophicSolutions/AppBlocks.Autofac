using AppBlocks.Autofac.Support;
using Autofac.Features.Indexed;
using log4net;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AppBlocks.Autofac.Tests.KeyedAndNamedServices
{
    [AppBlocksService]
    public class KeyedServiceReceiver : IKeyedServiceReceiver
    {
        private readonly ILogger<KeyedServiceReceiver> logger;

        private static int callCount;
        public static int GetCallCount() => callCount;
        public static void ResetCount() => callCount = 0;

        private readonly IIndex<string, IKeyedService> keyedServices;

        public KeyedServiceReceiver(ILogger<KeyedServiceReceiver> logger,
            IIndex<string, IKeyedService> keyedServices)
        {
            this.logger = logger;
            this.keyedServices = keyedServices;
        }

        public void RunKeyedServices()
        {
            callCount++;

            logger.LogInformation($"{nameof(KeyedServiceReceiver)}.{nameof(RunKeyedServices)} called successfully");

            if (keyedServices.TryGetValue("KeyedService1", out IKeyedService keyedService))
                keyedService.RunKeyedService();

            if (keyedServices.TryGetValue("KeyedService2", out keyedService))
                keyedService.RunKeyedService();
        }
    }
}
