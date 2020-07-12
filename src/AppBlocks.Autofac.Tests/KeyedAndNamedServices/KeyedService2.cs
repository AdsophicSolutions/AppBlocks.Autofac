using AppBlocks.Autofac.Support;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AppBlocks.Autofac.Tests.KeyedAndNamedServices
{
    [AppBlocksKeyedService("KeyedService2", typeof(IKeyedService))]
    public class KeyedService2 : IKeyedService
    {
        private static readonly ILog logger =
           LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static int callCount;
        public static int GetCallCount() => callCount;
        public static void ResetCount() => callCount = 0;

        public void RunKeyedService()
        {
            callCount++;

            if (logger.IsInfoEnabled)
                logger.Info($"{nameof(KeyedService2)}.{nameof(RunKeyedService)} called successfully");
        }
    }
}
