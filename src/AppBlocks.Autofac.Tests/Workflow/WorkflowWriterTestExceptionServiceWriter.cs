using AppBlocks.Autofac.Common;
using AppBlocks.Autofac.Support;
using Castle.DynamicProxy;
using System;

namespace AppBlocks.Autofac.Tests.Workflow
{
    [AppBlocksWorkflowWriterService("TestExceptionWorkflowWriter")]
    public class WorkflowWriterTestExceptionServiceWriter : IWorkflowWriter
    {
        private static int postInvocationCallCount = 0;
        public static int GetPostInvocationCallCount() => postInvocationCallCount;

        private static int preInvocationCallCount = 0;

        internal static void ResetCount()
        {
            preInvocationCallCount = 0;
            postInvocationCallCount = 0;
        }

        public static int GetPreInvocationCallCount() => preInvocationCallCount;

        public void PreMethodInvocationOutput(IInvocation invocation)
        {
            preInvocationCallCount++;
        }

        public void PostMethodInvocationOutput(IInvocation invocation)
        {
            postInvocationCallCount++;
            throw new Exception("Workflow Writer failed");
        }
    }
}