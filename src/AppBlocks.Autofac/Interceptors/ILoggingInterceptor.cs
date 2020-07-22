using Castle.DynamicProxy;

namespace AppBlocks.Autofac.Interceptors
{
    /// <summary>
    /// Defines methods for logging interceptor
    /// </summary>
    public interface ILoggingInterceptor
    {
        /// <summary>
        /// Log method input parameters
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/> instance</param>
        void PreMethodInvoke(IInvocation invocation);

        /// <summary>
        /// Log method return parameters
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/> instance</param>
        void PostMethodInvoke(IInvocation invocation);
    }
}
