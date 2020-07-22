using AppBlocks.Autofac.Common;
using AppBlocks.Autofac.Support;
using Castle.DynamicProxy;
using System;

namespace AppBlocks.Autofac.Interceptors.Placeholders
{
    /// <summary>
    /// Default implementation for <see cref="IWorkflowWriter"/> to satisfy 
    /// Autofac keyed services requirements
    /// </summary>
    [AppBlocksWorkflowWriterService(WorkflowName: "**")]
    internal class ClassWorkflowWritePlaceholder : IWorkflowWriter
    {
        public void PostMethodInvocationOutput(IInvocation invocation)
        {
            throw new NotImplementedException();
        }

        public void PreMethodInvocationOutput(IInvocation invocation)
        {
            throw new NotImplementedException();
        }
    }
}
