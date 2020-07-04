using AppBlocks.Autofac.Common;
using Autofac.Features.Indexed;
using Castle.DynamicProxy;

namespace AppBlocks.Autofac.Interceptors
{
    public class ValidationInterceptor : AutofacInterceptorBase
    {   
        private readonly IIndex<string, IClassValidator> classValidators;

        public ValidationInterceptor( IIndex<string, IClassValidator> classValidators)
        {            
            this.classValidators = classValidators;
        }

        protected override void PostMethodInvoke(IInvocation invocation)
        {
            if (classValidators.TryGetValue(invocation.TargetType.FullName, out IClassValidator classValidator))
            {
                classValidator.ValidateInputParameters(invocation);
            }
        }

        protected override void PreMethodInvoke(IInvocation invocation)
        {
            if (classValidators.TryGetValue(invocation.TargetType.FullName, out IClassValidator classValidator))
            {
                classValidator.ValidateResult(invocation);
            }
        }
    }
}
