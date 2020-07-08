using Castle.DynamicProxy;

namespace AppBlocks.Autofac.Common
{
    /// <summary>
    /// Implementors of this interface are responsible for validating 
    /// parameters to method calls in a class and validate 
    /// result values. 
    /// </summary>
    public interface IServiceValidator
    {
        void ValidateInputParameters(IInvocation invocation);
        void ValidateResult(IInvocation invocation);
    }
}
