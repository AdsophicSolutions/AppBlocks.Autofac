using Castle.DynamicProxy;

namespace AppBlocks.Autofac.Common
{
    /// <summary>
    /// Defines methods for use in service validators. 
    /// </summary>
    public interface IServiceValidator
    {
        /// <summary>
        /// Called before service method is invoked. Implementors should use 
        /// this method to write validations against method input parameters
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/> details</param>
        void ValidateInputParameters(IInvocation invocation);

        /// <summary>
        /// Called after service method returns. Implementors should use this 
        /// method to write validations for method return value
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/> details</param>
        void ValidateResult(IInvocation invocation);
    }
}
