using AppBlocks.Autofac.Common;
using Autofac.Features.Indexed;
using Castle.DynamicProxy;
using log4net;
using System;
using System.Linq;

namespace AppBlocks.Autofac.Interceptors
{
    public class LoggingInterceptor : AutofacInterceptorBase
    {
        private readonly IIndex<string, IClassLogger> classLoggers;
        private readonly ILoggingConfiguration loggingConfiguration;

        public LoggingInterceptor(
            IIndex<string, IClassLogger> classLoggers,
            ILoggingConfiguration loggingConfiguration)
        {
            this.classLoggers = classLoggers;
            this.loggingConfiguration = loggingConfiguration;
        }

        protected override void PreMethodInvoke(IInvocation invocation)
        {
            if (classLoggers.TryGetValue(invocation.TargetType.FullName, out IClassLogger classLogger))
            {
                classLogger.PreMethodInvocationLog(invocation);
            }
            else
            {                
                if (!loggingConfiguration.IsTypeExcluded(invocation.TargetType.FullName))
                {
                    Logger.Info($"Logging Interceptor: Calling {invocation.TargetType.FullName}.{invocation.Method.Name}" +
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
                classLogger.PostMethodInvocationLog(invocation);
            }
            else
            {   
                if (!loggingConfiguration.IsTypeExcluded(invocation.TargetType.FullName))
                {
                    Logger.Info($"Logging Interceptor: Finished {invocation.TargetType.FullName}.{invocation.Method.Name}. " +
                        $"Returned {invocation.ReturnValue}");
                }
            }
        }

        public ILog Logger { get; set; }
    }
}
