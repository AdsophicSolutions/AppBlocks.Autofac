using AppBlocks.Autofac.Common;
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
    public class LoggingInterceptor : AutofacInterceptorBase
    {
        private readonly IIndex<string, IClassLogger> classLoggers;
        private readonly ILoggingConfiguration loggingConfiguration;
        private readonly HashSet<string> disabledLoggers = new HashSet<string>();

        public LoggingInterceptor(
            IIndex<string, IClassLogger> classLoggers,
            ILoggingConfiguration loggingConfiguration)
        {
            this.classLoggers = classLoggers;
            this.loggingConfiguration = loggingConfiguration;
        }

        protected override void PreMethodInvoke(IInvocation invocation)
        {
            if (classLoggers.TryGetValue(invocation.TargetType.FullName, out IClassLogger classLogger)
                && !disabledLoggers.Contains(classLogger.GetType().FullName))
            {
                try
                {
                    classLogger.PreMethodInvocationLog(invocation);
                }
                catch(Exception e)
                {
                    if(Logger.IsErrorEnabled)
                    {
                        Logger.Error($"Class logger {classLogger.GetType().FullName} threw an exception during PreMethodInvocationLog method call. Logger will be disabled", e);
                    }

                    disabledLoggers.Add(classLogger.GetType().FullName);
                }
            }
            else
            {
                if (!loggingConfiguration.IsTypeExcluded(invocation.TargetType.FullName))
                {
                    if (Logger.IsInfoEnabled)
                        Logger.Info($"Logging Interceptor: Calling {invocation.TargetType.FullName}.{invocation.Method.Name} " +
                            $"with parameters {string.Join(", ", invocation.Arguments.Select(a => a ?? string.Empty).ToString()).ToArray()}");
                }
            }
        }

        protected override void MethodInvoke(IInvocation invocation)
        {
            try
            {
                base.MethodInvoke(invocation);
            }
            catch (Exception e)
            {
                Logger.Error($"Exception thrown running {invocation.TargetType.FullName}.{invocation.Method.Name}", e);
            }
        }

        protected override void PostMethodInvoke(IInvocation invocation)
        {
            if (classLoggers.TryGetValue(invocation.TargetType.FullName, out IClassLogger classLogger))
            {
                try
                {
                    classLogger.PostMethodInvocationLog(invocation);
                }
                catch (Exception e)
                {
                    if (Logger.IsErrorEnabled)
                    {
                        Logger.Error($"Class logger {classLogger.GetType().FullName} threw an exception during PostmethodInvocationLog method call. " +
                            $"Logger will be disabled", e);
                    }

                    disabledLoggers.Add(classLogger.GetType().FullName);
                }
            }
            else
            {
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
