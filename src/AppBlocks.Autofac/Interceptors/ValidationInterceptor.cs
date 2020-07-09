using AppBlocks.Autofac.Common;
using AppBlocks.Autofac.Support;
using Autofac.Features.Indexed;
using Castle.DynamicProxy;
using log4net;
using System;
using System.Collections.Generic;

namespace AppBlocks.Autofac.Interceptors
{
    [AppBlocksService(
        Name: "",
        ServiceType: null,
        ServiceScope: EnumAppBlocksInstanceLifetime.SingleInstance,
        Interceptors: new string[0],
        Workflows: null,
        IsKeyed: false)]
    public class ValidationInterceptor : IValidationInterceptor
    {
        private readonly IIndex<string, IServiceValidator> serviceValidators;
        private readonly HashSet<string> disabledServiceValidators = new HashSet<string>();

        public ValidationInterceptor(IIndex<string, IServiceValidator> serviceValidators)
        {
            this.serviceValidators = serviceValidators;
        }

        public void PreMethodInvoke(IInvocation invocation)
        {
            if (serviceValidators.TryGetValue(invocation.TargetType.FullName, out IServiceValidator serviceValidator) &&
                !disabledServiceValidators.Contains(serviceValidator.GetType().FullName))
            {
                try
                {
                    serviceValidator.ValidateInputParameters(invocation);
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

        public void PostMethodInvoke(IInvocation invocation)
        {
            if (serviceValidators.TryGetValue(invocation.TargetType.FullName, out IServiceValidator serviceValidator) &&
                !disabledServiceValidators.Contains(serviceValidator.GetType().FullName))
            {
                try
                {
                    serviceValidator.ValidateResult(invocation);
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
