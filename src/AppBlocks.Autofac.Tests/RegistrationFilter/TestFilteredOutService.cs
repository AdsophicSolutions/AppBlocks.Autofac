using AppBlocks.Autofac.Support;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AppBlocks.Autofac.Tests.RegistrationFilter
{
    [AppBlocksService]
    public class TestFilteredOutService : ITestFilterService
    {
        private static int callCount;

        public static int GetCallCount() => callCount;
        public static void ResetCount() => callCount = 0;

        public void RunService() => callCount++;
    }
}
