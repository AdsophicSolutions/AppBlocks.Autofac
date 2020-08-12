using Castle.DynamicProxy;
using log4net;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Interceptors
{
    /// <summary>
    /// Concrete implementation for <see cref="IInterceptor"/>. Calls 
    /// <see cref="ILoggingInterceptor"/>, <see cref="IValidationInterceptor"/>, and 
    /// <see cref="IWorkflowInterceptor"/> implementations
    /// </summary>
    public class AppBlocksServiceInterceptor : AutofacInterceptorBase
    {
        private readonly ILoggingInterceptor loggingInterceptor;
        private readonly IValidationInterceptor validationInterceptor;
        private readonly IWorkflowInterceptor workflowInterceptor;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="loggingInterceptor"><see cref="ILoggingInterceptor"/> instance</param>
        /// <param name="validationInterceptor"><see cref="IValidationInterceptor"/> instance</param>
        /// <param name="workflowInterceptor"><see cref="IWorkflowInterceptor"/> instance</param>
        public AppBlocksServiceInterceptor(
            ILoggingInterceptor loggingInterceptor,
            IValidationInterceptor validationInterceptor,
            IWorkflowInterceptor workflowInterceptor)
        {
            this.loggingInterceptor = loggingInterceptor;
            this.validationInterceptor = validationInterceptor;
            this.workflowInterceptor = workflowInterceptor;
        }

        /// <summary>
        /// Calls logging, validation and workflow interceptor implementations
        /// after service method returns
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/> instance</param>
        protected override void PostMethodInvoke(IInvocation invocation)
        {
            try
            {                
                // Call logging interceptor
                loggingInterceptor.PostMethodInvoke(invocation);

                // Call validation interceptor
                validationInterceptor.PostMethodInvoke(invocation);

                // Call workflow interceptor
                workflowInterceptor.PostMethodInvoke(invocation);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                // Log any errors
                if (Logger.IsErrorEnabled)
                    Logger.Error($"Error executing {nameof(PostMethodInvoke)}", e);
            }
        }

        /// <summary>
        /// Invoke service method
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/> instance</param>
        protected override void MethodInvoke(IInvocation invocation)
        {
            try
            {
                // call base
                base.MethodInvoke(invocation);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                // log any errors
                Logger.Error($"Exception thrown running {invocation?.TargetType.FullName}.{invocation?.Method.Name}", e);
            }
        }

        /// <summary>
        /// Calls logging interceptor, validation interceptor and workflow interceptor
        /// before service method is called. 
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/> instance</param>
        protected override void PreMethodInvoke(IInvocation invocation)
        {
            try
            {
                // Call logging interceptor
                loggingInterceptor.PreMethodInvoke(invocation);

                // call validation interceptor
                validationInterceptor.PreMethodInvoke(invocation);

                // call workflow interceptor
                workflowInterceptor.PreMethodInvoke(invocation);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                // Log any errors. 
                if (Logger.IsErrorEnabled)
                    Logger.Error($"Error executing {nameof(PreMethodInvoke)}", e);
            }
        }

        /// <summary>
        /// <see cref="ILog"/> instance
        /// </summary>
        public ILog Logger { get; set; }
    }
}
