using AppBlocks.Autofac.Support;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AppBlocks.Autofac.Tests.RegistrationFilter
{
    [AppBlocksService]
    public class TestFilteredInService : ITestFilterService
    {
        private static readonly ILog logger =            
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static int callCount;

        public static int GetCallCount() => callCount;
        public static void ResetCount() => callCount = 0;

        public void RunService()
        {
            callCount++;

            if (logger.IsInfoEnabled)
                logger.Info($"{nameof(TestFilteredInService)}.{nameof(RunService)} called successfully");
        }
    }
}
