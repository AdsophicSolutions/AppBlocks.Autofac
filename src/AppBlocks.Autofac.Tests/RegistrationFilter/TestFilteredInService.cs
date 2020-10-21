using AppBlocks.Autofac.Support;
using log4net;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Permissions;
using System.Text;

namespace AppBlocks.Autofac.Tests.RegistrationFilter
{
    [AppBlocksService]
    public class TestFilteredInService : ITestFilterService
    {
        private static int callCount;

        public static int GetCallCount() => callCount;
        public static void ResetCount() => callCount = 0;

        private readonly ILogger<TestFilteredInService> logger;

        public TestFilteredInService(ILogger<TestFilteredInService> logger)
        {
            this.logger = logger;
        }

        public void RunService()
        {
            callCount++;

            logger.LogInformation($"{nameof(TestFilteredInService)}.{nameof(RunService)} called successfully");
        }
    }
}
