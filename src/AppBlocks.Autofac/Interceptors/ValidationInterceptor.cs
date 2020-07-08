using AppBlocks.Autofac.Common;
using Autofac.Features.Indexed;
using Castle.DynamicProxy;
using log4net;
using System;
using System.Collections.Generic;

namespace AppBlocks.Autofac.Interceptors
{
    public class ValidationInterceptor : AutofacInterceptorBase
    {
        private readonly IIndex<string, IServiceValidator> serviceValidators;
        private readonly HashSet<string> disabledServiceValidators = new HashSet<string>();

        public ValidationInterceptor(IIndex<string, IServiceValidator> serviceValidators)
        {
            this.serviceValidators = serviceValidators;
        }

        protected override void PreMethodInvoke(IInvocation invocation)
        {
            if (serviceValidators.TryGetValue(invocation.TargetType.FullName, out IServiceValidator serviceValidator) &&
                !disabledServiceValidators.Contains(invocation.TargetType.FullName))
            {
                try
                {
                    serviceValidator.ValidateResult(invocation);
                }
                catch(Exception e)
                {
                    if (Logger.IsErrorEnabled)
                    {
                        Logger.Error($"Service Validator {serviceValidator.GetType().FullName} threw an exception during PreMethodInvoke method call" +
                            $"Validator will be disabled", e);
                    }

                    disabledServiceValidators.Add(serviceValidator.GetType().FullName);
                }
            }
        }

        protected override void PostMethodInvoke(IInvocation invocation)
        {
            if (serviceValidators.TryGetValue(invocation.TargetType.FullName, out IServiceValidator serviceValidator) &&
                !disabledServiceValidators.Contains(invocation.TargetType.FullName))
            {
                try
                {
                    serviceValidator.ValidateInputParameters(invocation);
                }
                catch (Exception e)
                {
                    if (Logger.IsErrorEnabled)
                    {
                        Logger.Error($"Service Validator {serviceValidator.GetType().FullName} threw an exception during PostMethodInvoke method call. " +
                            $"Validator will be disabled", e);
                    }

                    disabledServiceValidators.Add(serviceValidator.GetType().FullName);
                }
            }
        }

        public ILog Logger { get; set; }
    }
}
