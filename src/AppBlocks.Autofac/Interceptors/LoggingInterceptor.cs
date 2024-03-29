﻿using AppBlocks.Autofac.Common;
using AppBlocks.Autofac.Support;
using Autofac.Features.Indexed;
using Castle.DynamicProxy;
using log4net;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<LoggingInterceptor> logger;
        private readonly IIndex<string, IServiceLogger> serviceLoggers;
        private readonly ILoggingConfiguration loggingConfiguration;
        private readonly HashSet<string> disabledServiceLoggers = new HashSet<string>();

        public LoggingInterceptor(
            ILogger<LoggingInterceptor> logger,
            IIndex<string, IServiceLogger> serviceLoggers,
            ILoggingConfiguration loggingConfiguration)
        {
            // Save reference to logger
            this.logger = logger;

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
                    if (logger.IsEnabled(LogLevel.Error))
                        logger.LogError(e, $"Service logger { serviceLogger.GetType().FullName} threw an exception " + $"during PreMethodInvocationLog method call. Logger will be disabled");

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
                    // If type is elevated to warn log as warn
                    if(loggingConfiguration.IsTypeElevatedToWarn(invocation.TargetType.FullName))
                    {
                        if (logger.IsEnabled(LogLevel.Warning))
                            logger.LogWarning(
                                $"Logging Interceptor: Calling {invocation.TargetType.FullName}.{invocation.Method.Name} " +
                                    $"with parameters: {(invocation.Arguments.Length == 0 ? "None" : string.Join(", ", invocation.Arguments))}",
                                invocation.Arguments);
                    }
                    // if type is elevated to info log as info
                    else if (loggingConfiguration.IsTypeElevatedToInfo(invocation.TargetType.FullName))
                    {
                        if (logger.IsEnabled(LogLevel.Information))
                            logger.LogInformation(
                                $"Logging Interceptor: Calling {invocation.TargetType.FullName}.{invocation.Method.Name} " +
                                    $"with parameters: {(invocation.Arguments.Length == 0 ? "None" : string.Join(", ", invocation.Arguments))}",
                                invocation.Arguments);
                    }
                    else if (logger.IsEnabled(LogLevel.Debug))
                        logger.LogDebug(
                            $"Logging Interceptor: Finished {invocation.TargetType.FullName}.{invocation.Method.Name}. " + 
                                $"Returned {invocation.ReturnValue}", 
                            invocation.ReturnValue);
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
                    if (logger.IsEnabled(LogLevel.Error))
                        logger.LogError(e, 
                            $"Service logger { serviceLogger.GetType().FullName} threw an exception during PostmethodInvocationLog method call. " + 
                            $"Logger will be disabled");

                    // Disable custom logger on exception. 
                    disabledServiceLoggers.Add(serviceLogger.GetType().FullName);
                }
            }
            else
            {
                // Use default logger if not excluded from logging
                if (!loggingConfiguration.IsTypeExcluded(invocation.TargetType.FullName))
                {
                    // Check if this is an asynchronous method calle
                    var resultMethod = invocation?
                        .ReturnValue?
                        .GetType()
                        .GetMethods()
                        .FirstOrDefault(n => n.Name == "get_Result");

                    // Not an async call
                    if (resultMethod == null)
                    {
                        // If type is elevated to warn log as warn
                        if (loggingConfiguration.IsTypeElevatedToWarn(invocation.TargetType.FullName))
                        {
                            if (logger.IsEnabled(LogLevel.Warning))
                                logger.LogWarning(
                                    $"Logging Interceptor: Finished {invocation.TargetType.FullName}.{invocation.Method.Name}. " +
                                        $"Returned {invocation.ReturnValue}",
                                    invocation.ReturnValue);
                        }
                        // if type is elevated to info log as info
                        else if (loggingConfiguration.IsTypeElevatedToInfo(invocation.TargetType.FullName))
                        {
                            if (logger.IsEnabled(LogLevel.Information))
                                logger.LogInformation(
                                    $"Logging Interceptor: Finished {invocation.TargetType.FullName}.{invocation.Method.Name}. " +
                                        $"Returned {invocation.ReturnValue}",
                                    invocation.ReturnValue);
                        }
                        else if (logger.IsEnabled(LogLevel.Debug))
                            logger.LogDebug(
                                $"Logging Interceptor: Finished {invocation.TargetType.FullName}.{invocation.Method.Name}. " +
                                    $"Returned {invocation.ReturnValue}",
                                invocation.ReturnValue);                        
                    }
                    // Log result for async call
                    else
                    {
                        try
                        {
                            var returnValue = resultMethod?.Invoke(invocation?.ReturnValue, null);

                            // If type is elevated to warn log as warn
                            if (loggingConfiguration.IsTypeElevatedToWarn(invocation.TargetType.FullName))
                            {
                                if (logger.IsEnabled(LogLevel.Warning))
                                    logger.LogWarning(
                                        $"Logging Interceptor: Finished {invocation.TargetType.FullName}.{invocation.Method.Name}. " +
                                            $"Returned {returnValue}",
                                        returnValue);
                            }
                            // if type is elevated to info log as info
                            else if (loggingConfiguration.IsTypeElevatedToInfo(invocation.TargetType.FullName))
                            {
                                if (logger.IsEnabled(LogLevel.Information))
                                    logger.LogInformation(
                                        $"Logging Interceptor: Finished {invocation.TargetType.FullName}.{invocation.Method.Name}. " +
                                            $"Returned {returnValue}",
                                        returnValue);
                            }
                            else if (logger.IsEnabled(LogLevel.Debug))
                                logger.LogDebug(
                                    $"Logging Interceptor: Finished {invocation.TargetType.FullName}.{invocation.Method.Name}. " +
                                        $"Returned {returnValue}",
                                    returnValue);
                        }
#pragma warning disable CA1031 // Do not catch general exception types
                        catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
                        {
                            // Log any exceptions
                            if (logger.IsEnabled(LogLevel.Error))
                                logger.LogError(
                                    e,
                                    $"Logging Interceptor: {invocation.TargetType.FullName}.{invocation.Method.Name} threw an exception");
                        }
                    }
                }
            }
        }
    }
}
