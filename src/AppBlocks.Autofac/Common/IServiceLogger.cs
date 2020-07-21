using Castle.DynamicProxy;

namespace AppBlocks.Autofac.Common
{
    /// <summary>
    /// Defines methods to use in custom service loggers
    /// </summary>
    public interface IServiceLogger
    {
        /// <summary>
        /// Called before service method is invoked. Implementors should log 
        /// service method input parameters here
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/> details</param>
        void PreMethodInvocationLog(IInvocation invocation);

        /// <summary>
        /// Called after service method returns. Implements should log
        /// servie method output parameters here
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/> details</param>
        void PostMethodInvocationLog(IInvocation invocation);
    }
}
