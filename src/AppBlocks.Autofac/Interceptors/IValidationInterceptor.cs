using Castle.DynamicProxy;

namespace AppBlocks.Autofac.Interceptors
{
    /// <summary>
    /// Defines methods for validation interceptor
    /// </summary>
    public interface IValidationInterceptor
    {
        /// <summary>
        /// Write validations against input parameters for service method
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/> instance</param>
        void PreMethodInvoke(IInvocation invocation);

        /// <summary>
        /// Write validations against return value for service method
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/> instance</param>
        void PostMethodInvoke(IInvocation invocation);
    }
}
