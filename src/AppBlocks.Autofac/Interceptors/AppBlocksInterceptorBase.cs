using Castle.DynamicProxy;

namespace AppBlocks.Autofac.Interceptors
{
    public abstract class AutofacInterceptorBase : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            PreMethodInvoke(invocation);
            MethodInvoke(invocation);
            PostMethodInvoke(invocation);
        }

        protected abstract void PreMethodInvoke(IInvocation invocation);
        protected virtual void MethodInvoke(IInvocation invocation)
        {
            invocation.Proceed();
        }
        protected abstract void PostMethodInvoke(IInvocation invocation);
    }
}
