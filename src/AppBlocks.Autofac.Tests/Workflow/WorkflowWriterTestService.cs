using AppBlocks.Autofac.Support;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AppBlocks.Autofac.Tests.Workflow
{
    [AppBlocksService(Name: "", 
        ServiceType: null, 
        ServiceScope: AppBlocksInstanceLifetime.InstancePerLifetimeScope,
        Interceptors: new string[0],
        Workflows: new [] { "TestWorkflowWriter" },
        IsKeyed: false)]
    public class WorkflowWriterTestService : IWorkflowWriterTestService
    {
        private static readonly ILog logger =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Method1()
        {
            if (logger.IsInfoEnabled)
                logger.Info($"{nameof(WorkflowWriterTestService)}.{nameof(Method1)} called successfully");
        }
    }
}
