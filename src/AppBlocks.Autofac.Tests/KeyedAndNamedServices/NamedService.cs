using AppBlocks.Autofac.Support;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AppBlocks.Autofac.Tests.KeyedAndNamedServices
{
    [AppBlocksNamedService("AppBlocks.Autofac.Tests.KeyedAndNamedServices.NamedService")]
    public class NamedService : INamedService
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
                logger.Info($"{nameof(NamedService)}.{nameof(RunService)} called successfully");
        }
    }
}
