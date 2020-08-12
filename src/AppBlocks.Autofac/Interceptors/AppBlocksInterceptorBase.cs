using Castle.DynamicProxy;

namespace AppBlocks.Autofac.Interceptors
{
    /// <summary>
    /// Abstract template class that implements <see cref="IInterceptor"/>
    /// </summary>
    public abstract class AutofacInterceptorBase : IInterceptor
    {
        /// <summary>
        /// Intercepts method calls to Autofac services
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/> instance</param>
        public void Intercept(IInvocation invocation)
        {
            // Before service method is invoked
            PreMethodInvoke(invocation);

            // Invoke service method
            MethodInvoke(invocation);

            // After service method returns
            PostMethodInvoke(invocation);
        }

        /// <summary>
        /// Called before service method is invoked. 
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/> instance</param>
        protected abstract void PreMethodInvoke(IInvocation invocation);

        /// <summary>
        /// Invokes service method
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/> instance</param>
        protected virtual void MethodInvoke(IInvocation invocation)
        {
            // Invoke service method
            invocation?.Proceed();
        }

        /// <summary>
        /// Called after service method returns
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/></param>
        protected abstract void PostMethodInvoke(IInvocation invocation);
    }
}
