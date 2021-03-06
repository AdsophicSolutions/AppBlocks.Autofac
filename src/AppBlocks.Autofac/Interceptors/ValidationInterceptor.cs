﻿using AppBlocks.Autofac.Common;
using AppBlocks.Autofac.Support;
using Autofac.Features.Indexed;
using Castle.DynamicProxy;
using log4net;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace AppBlocks.Autofac.Interceptors
{
    /// <summary>
    /// ValidationInterceptor calls registered type validators before and after 
    /// </summary>
    [AppBlocksService(
        Name: "",
        ServiceType: null,
        ServiceScope: AppBlocksInstanceLifetime.SingleInstance,
        Interceptors: new string[0],
        Workflows: null,
        IsKeyed: false)]
    public class ValidationInterceptor : IValidationInterceptor
    {
        private readonly ILogger<ValidationInterceptor> logger;
        private readonly IIndex<string, IServiceValidator> serviceValidators;
        private readonly HashSet<string> disabledServiceValidators = new HashSet<string>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Referenc to logger instance</param>
        /// <param name="serviceValidators">Keyed <see cref="IServiceValidator"/> instances</param>
        public ValidationInterceptor(ILogger<ValidationInterceptor> logger,
            IIndex<string, IServiceValidator> serviceValidators)
        {
            this.logger = logger;
            this.serviceValidators = serviceValidators;
        }

        /// <summary>
        /// Call validation interceptor before service method invocation
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/> instance</param>
        public void PreMethodInvoke(IInvocation invocation)
        {
            // Look for service validator for service
            if (serviceValidators.TryGetValue(invocation?.TargetType.FullName, out IServiceValidator serviceValidator) &&
                !disabledServiceValidators.Contains(serviceValidator.GetType().FullName))
            {
                try
                {
                    // Call validator method to validate input parameters
                    serviceValidator.ValidateInputParameters(invocation);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    // Log errors
                    if (logger.IsEnabled(LogLevel.Error))
                        logger.LogError(e, 
                            $"Service Validator { serviceValidator.GetType().FullName} threw an exception during PreMethodInvoke method call" + 
                            $"Validator will be disabled");

                    // Disable validator. Validators are disabled if they throw 
                    // an exception
                    disabledServiceValidators.Add(serviceValidator.GetType().FullName);
                }
            }
        }

        /// <summary>
        /// Call validation interceptor after service method returns
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/> instance</param>
        public void PostMethodInvoke(IInvocation invocation)
        {
            // Look for service validator for service 
            // Ignore if service validator is disabled
            if (serviceValidators.TryGetValue(invocation?.TargetType.FullName, out IServiceValidator serviceValidator) &&
                !disabledServiceValidators.Contains(serviceValidator.GetType().FullName))
            {
                try
                {
                    // Call service validator with method return value
                    serviceValidator.ValidateResult(invocation);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    // Log error
                    if (logger.IsEnabled(LogLevel.Error))
                        logger.LogError(e, 
                            $"Service Validator { serviceValidator.GetType().FullName} threw an exception during PostMethodInvoke method call. " + 
                            $"Validator will be disabled");


                    // Disable validator if validator throws exception
                    disabledServiceValidators.Add(serviceValidator.GetType().FullName);
                }
            }
        }
    }
}
