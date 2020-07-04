using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Common
{
    public interface IWorkflowWriter
    {
        void PreMethodInvocationOutput(IInvocation invocation);
        void PostMethodInvocationOutput(IInvocation invocation);
    }
}
