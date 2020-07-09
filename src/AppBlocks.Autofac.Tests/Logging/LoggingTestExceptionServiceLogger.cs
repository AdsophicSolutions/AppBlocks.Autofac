using AppBlocks.Autofac.Common;
using AppBlocks.Autofac.Support;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Tests.Logging
{
    [AppBlocksLoggerService("AppBlocks.Autofac.Tests.Logging.LoggingTestExceptionService")]
    public class LoggingTestExceptionServiceLogger : IServiceLogger
    {
        private static int postInvocationCallCount = 0;
        public static int GetPostInvocationCallCount() => postInvocationCallCount;

        private static int preInvocationCallCount = 0;

        internal static void ResetCount()
        {
            preInvocationCallCount = 0;
            postInvocationCallCount = 0;
        }

        public static int GetPreInvocationCallCount() => preInvocationCallCount;

        public void PostMethodInvocationLog(IInvocation invocation)
        {
            postInvocationCallCount++;
            throw new Exception("Failed during logging");
        }

        public void PreMethodInvocationLog(IInvocation invocation)
        {
            preInvocationCallCount++;            
        }
    }
}
