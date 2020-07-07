using Castle.DynamicProxy;

namespace AppBlocks.Autofac.Common
{
    public interface IServiceLogger
    {
        void PreMethodInvocationLog(IInvocation invocation);
        void PostMethodInvocationLog(IInvocation invocation);
    }
}
