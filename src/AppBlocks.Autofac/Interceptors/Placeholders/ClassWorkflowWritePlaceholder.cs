using AppBlocks.Autofac.Common;
using AppBlocks.Autofac.Support;
using Castle.DynamicProxy;
using System;

namespace AppBlocks.Autofac.Interceptors.Placeholders
{
    [AppBlocksWorkflowWriterService(WorkflowName: "**")]
    public class ClassWorkflowWritePlaceholder : IWorkflowWriter
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
