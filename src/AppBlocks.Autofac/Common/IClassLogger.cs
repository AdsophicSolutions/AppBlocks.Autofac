using Castle.DynamicProxy;

namespace AppBlocks.Autofac.Common
{
    public interface IClassLogger
    {
        void PreMethodInvocationLog(IInvocation invocation);
        void PostMethodInvocationLog(IInvocation invocation);
    }
}
