using AppBlocks.Autofac.Common;
using AppBlocks.Autofac.Support;
using Castle.DynamicProxy;
using System;

namespace AppBlocks.Autofac.Interceptors.Placeholders
{
    [AppBlocksLoggerService(Name: "**")]
    public class ServiceLoggerPlaceholder : IServiceLogger
    {
        public void PostMethodInvocationLog(IInvocation invocation)
        {
            throw new NotImplementedException();
        }

        public void PreMethodInvocationLog(IInvocation invocation)
        {
            throw new NotImplementedException();
        }
    }
}
