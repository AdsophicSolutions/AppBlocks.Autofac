using AppBlocks.Autofac.Support;
using log4net;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<WorkflowWriterTestService> logger;

        public WorkflowWriterTestService(ILogger<WorkflowWriterTestService> logger)
        {
            this.logger = logger;
        }

        public void Method1()
        {
            logger.LogInformation($"{nameof(WorkflowWriterTestService)}.{nameof(Method1)} called successfully");
        }
    }
}
