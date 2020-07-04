using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Common
{
    /// <summary>
    /// Implementors of this interface are responsible for validating 
    /// parameters to method calls in a class and validate 
    /// result values. 
    /// </summary>
    public interface IClassValidator
    {
        void ValidateInputParameters(IInvocation invocation);
        void ValidateResult(IInvocation invocation);
    }
}
