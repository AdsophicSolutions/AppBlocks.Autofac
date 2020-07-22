using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Interceptors
{
    /// <summary>
    /// Defines method for workflow interceptors
    /// </summary>
    public interface IWorkflowInterceptor
    {
        /// <summary>
        /// Write to workflow based on input parameters to service method 
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/> instance</param>
        void PreMethodInvoke(IInvocation invocation);

        /// <summary>
        /// Write to workflow based on return value for service method
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/> instance</param>
        void PostMethodInvoke(IInvocation invocation);
    }
}
