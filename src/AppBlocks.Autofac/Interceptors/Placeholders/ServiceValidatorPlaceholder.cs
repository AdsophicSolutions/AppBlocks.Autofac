using AppBlocks.Autofac.Common;
using AppBlocks.Autofac.Support;
using Castle.DynamicProxy;
using System;

namespace AppBlocks.Autofac.Interceptors.Placeholders
{
    /// <summary>
    /// Default implementation for <see cref="IServiceValidator"/> to satisfy
    /// Autofac keyed services requirements
    /// </summary>
    [AppBlocksValidatorService(ClassFullName: "**")]
    internal class ServiceValidatorPlaceholder : IServiceValidator
    {
        public void ValidateInputParameters(IInvocation invocation)
        {
            throw new NotImplementedException();
        }

        public void ValidateResult(IInvocation invocation)
        {
            throw new NotImplementedException();
        }
    }
}
