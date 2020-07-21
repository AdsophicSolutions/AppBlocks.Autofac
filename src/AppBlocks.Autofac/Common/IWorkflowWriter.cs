using Castle.DynamicProxy;

namespace AppBlocks.Autofac.Common
{
    /// <summary>
    /// Defined methods for use in workflow writers. 
    /// </summary>
    public interface IWorkflowWriter
    {
        /// <summary>
        /// Called before service method is invoked. Override this method to write workflow 
        /// details based on method input parameters
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/> details</param>
        void PreMethodInvocationOutput(IInvocation invocation);

        /// <summary>
        /// Called after service method returns. Override this method to write workflow 
        /// details based on method return value
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/> details</param>
        void PostMethodInvocationOutput(IInvocation invocation);
    }
}
