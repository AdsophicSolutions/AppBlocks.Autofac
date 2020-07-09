using Castle.DynamicProxy;
using log4net;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Interceptors
{
    public class AppBlocksServiceInterceptor : AutofacInterceptorBase
    {
        private readonly ILoggingInterceptor loggingInterceptor;
        private readonly IValidationInterceptor validationInterceptor;
        private readonly IWorkflowInterceptor workflowInterceptor;

        public AppBlocksServiceInterceptor(
            ILoggingInterceptor loggingInterceptor,
            IValidationInterceptor validationInterceptor,
            IWorkflowInterceptor workflowInterceptor)
        {
            this.loggingInterceptor = loggingInterceptor;
            this.validationInterceptor = validationInterceptor;
            this.workflowInterceptor = workflowInterceptor;
        }

        protected override void PostMethodInvoke(IInvocation invocation)
        {
            try
            {                
                loggingInterceptor.PostMethodInvoke(invocation);
                validationInterceptor.PostMethodInvoke(invocation);
                workflowInterceptor.PostMethodInvoke(invocation);
            }
            catch(Exception e)
            {
                if (Logger.IsErrorEnabled)
                    Logger.Error($"Error executing {nameof(PostMethodInvoke)}", e);
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

        protected override void PreMethodInvoke(IInvocation invocation)
        {
            try
            {
                loggingInterceptor.PreMethodInvoke(invocation);
                validationInterceptor.PreMethodInvoke(invocation);
                workflowInterceptor.PreMethodInvoke(invocation);
            }
            catch (Exception e)
            {
                if (Logger.IsErrorEnabled)
                    Logger.Error($"Error executing {nameof(PreMethodInvoke)}", e);
            }
        }

        public ILog Logger { get; set; }
    }
}
