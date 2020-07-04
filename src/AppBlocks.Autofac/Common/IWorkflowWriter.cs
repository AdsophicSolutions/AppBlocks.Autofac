using Castle.DynamicProxy;

namespace AppBlocks.Autofac.Common
{
    public interface IWorkflowWriter
    {
        void PreMethodInvocationOutput(IInvocation invocation);
        void PostMethodInvocationOutput(IInvocation invocation);
    }
}
