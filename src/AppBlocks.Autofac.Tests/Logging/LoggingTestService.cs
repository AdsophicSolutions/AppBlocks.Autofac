using AppBlocks.Autofac.Support;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AppBlocks.Autofac.Tests.Logging
{
    [AppBlocksService]
    public class LoggingTestService : ILoggingTestService
    {
        private static readonly ILog logger =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Method1()
        {
            if (logger.IsInfoEnabled)
                logger.Info($"{nameof(LoggingTestService)}.{nameof(Method1)} called successfully");
        }
    }
}
