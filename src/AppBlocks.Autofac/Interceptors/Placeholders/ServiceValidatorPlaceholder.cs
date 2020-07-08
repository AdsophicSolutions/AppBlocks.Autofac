using AppBlocks.Autofac.Common;
using AppBlocks.Autofac.Support;
using Castle.DynamicProxy;
using System;

namespace AppBlocks.Autofac.Interceptors.Placeholders
{
    /// <summary>
    /// Place holder class validator to satisfy DI
    /// </summary>
    [AppBlocksValidatorService(ClassFullName: "**")]
    public class ServiceValidatorPlaceholder : IServiceValidator
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
