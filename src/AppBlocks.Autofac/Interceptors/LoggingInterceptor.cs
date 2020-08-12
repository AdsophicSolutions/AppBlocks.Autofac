using AppBlocks.Autofac.Common;
using AppBlocks.Autofac.Support;
using Autofac.Features.Indexed;
using Castle.DynamicProxy;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBlocks.Autofac.Interceptors
{
    /// <summary>
    /// LoggingInterceptor performs automatic logging before and after service methods are invoked. 
    /// </summary>
    [AppBlocksService(
        Name:"", 
        ServiceType:null,
        ServiceScope:AppBlocksInstanceLifetime.SingleInstance, 
        Interceptors: new string[0], 
        Workflows:null,
        IsKeyed: false)]
    internal class LoggingInterceptor : ILoggingInterceptor
    {
        private readonly IIndex<string, IServiceLogger> serviceLoggers;
        private readonly ILoggingConfiguration loggingConfiguration;
        private readonly HashSet<string> disabledServiceLoggers = new HashSet<string>();

        public LoggingInterceptor(
            IIndex<string, IServiceLogger> serviceLoggers,
            ILoggingConfiguration loggingConfiguration)
        {
            // keyed service loggers, keyed using type full names
            this.serviceLoggers = serviceLoggers;

            // logging configuration, used to exclude types
            this.loggingConfiguration = loggingConfiguration;
        }

        /// <summary>
        /// Call service loggers before service method invocation
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/> instance</param>
        public void PreMethodInvoke(IInvocation invocation)
        {
            // search for custom service logger
            if (serviceLoggers.TryGetValue(invocation.TargetType.FullName, out IServiceLogger serviceLogger)
                && !disabledServiceLoggers.Contains(serviceLogger.GetType().FullName))
            {
                try
                {
                    // Call logger 
                    serviceLogger.PreMethodInvocationLog(invocation);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    // Log any errors
                    if(Logger.IsErrorEnabled)
                        Logger.Error($"Service logger {serviceLogger.GetType().FullName} threw an exception " +
                            $"during PreMethodInvocationLog method call. Logger will be disabled", e);

                    // Disable logger if any exceptions occur. 
                    disabledServiceLoggers.Add(serviceLogger.GetType().FullName);
                }
            }
            else
            {
                // Use default logger if no custom logger is setup
                // Do not log if type is excluded from logging
                if (!loggingConfiguration.IsTypeExcluded(invocation.TargetType.FullName))
                {
                    if (Logger.IsInfoEnabled)
                        Logger.Info($"Logging Interceptor: Calling {invocation.TargetType.FullName}.{invocation.Method.Name} " +
                            $"with parameters: {(invocation.Arguments.Length == 0 ? "None" : string.Join(", ", invocation.Arguments))}");
                }
            }
        }

        /// <summary>
        /// Call service loggers after service method returns
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/> instance</param>
        public void PostMethodInvoke(IInvocation invocation)
        {
            // Call custom logger is one is configured
            if (serviceLoggers.TryGetValue(invocation.TargetType.FullName, out IServiceLogger serviceLogger)
                && !disabledServiceLoggers.Contains(serviceLogger.GetType().FullName))
            {
                try
                {
                    // Call log method
                    serviceLogger.PostMethodInvocationLog(invocation);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    // Log any exceptions
                    if (Logger.IsErrorEnabled)
                    {
                        Logger.Error($"Service logger {serviceLogger.GetType().FullName} threw an exception during PostmethodInvocationLog method call. " +
                            $"Logger will be disabled", e);
                    }

                    // Disable custom logger on exception. 
                    disabledServiceLoggers.Add(serviceLogger.GetType().FullName);
                }
            }
            else
            {
                // Use default logger if not excluded from logging
                if (!loggingConfiguration.IsTypeExcluded(invocation.TargetType.FullName))
                {
                    if (Logger.IsInfoEnabled)
                        Logger.Info($"Logging Interceptor: Finished {invocation.TargetType.FullName}.{invocation.Method.Name}. " +
                            $"Returned {invocation.ReturnValue}");
                }
            }
        }

        public ILog Logger { get; set; }
    }
}
