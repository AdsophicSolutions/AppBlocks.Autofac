using AppBlocks.Autofac.Common;
using AppBlocks.Autofac.Support;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Interceptors.Placeholders
{
    /// <summary>
    /// Place holder class validator to satisfy DI
    /// </summary>
    [AppBlocksClassValidatorWriterService(ClassFullName: "**")]
    public class ClassValidatorPlaceholder : IClassValidator
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
