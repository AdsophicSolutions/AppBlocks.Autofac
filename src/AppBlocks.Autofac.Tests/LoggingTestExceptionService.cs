using AppBlocks.Autofac.Support;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AppBlocks.Autofac.Tests
{
    [AppBlocksService]
    public class LoggingTestExceptionService : ILoggingTestExceptionService
    {
        private static readonly ILog logger =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static int callCount = 0;
        public static int GetCallCount() => callCount;

        internal static void ResetCount()
        {   callCount = 0;
        }

        public void Method1()
        {
            callCount++;

            if (logger.IsInfoEnabled)
                logger.Info($"{nameof(LoggingTestExceptionService)}.{nameof(Method1)} called successfully");
        }
    }
}
