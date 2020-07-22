using AppBlocks.Autofac.Common;
using AppBlocks.Autofac.Support;
using Castle.DynamicProxy;
using System;

namespace AppBlocks.Autofac.Interceptors.Placeholders
{
    /// <summary>
    /// Default implmentation of <see cref="IServiceLogger"/> to satisfy
    /// Autofac keyed services requirements
    /// </summary>
    [AppBlocksLoggerService(Name: "**")]
    internal class ServiceLoggerPlaceholder : IServiceLogger
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
