using AppBlocks.Autofac.Support;
using log4net;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace AppBlocks.Autofac.Tests.Workflow
{
    [AppBlocksService(Name: "",
        ServiceType: null,
        ServiceScope: AppBlocksInstanceLifetime.InstancePerLifetimeScope,
        Interceptors: new string[0],
        Workflows: new[] { "TestExceptionWorkflowWriter" },
        IsKeyed: false)]
    public class WorkflowWriterTestExceptionService : IWorkflowWriterTestExceptionService
    {
        private static int callCount = 0;
        public static int GetCallCount() => callCount;

        private readonly ILogger<WorkflowWriterTestExceptionService> logger;

        public WorkflowWriterTestExceptionService(ILogger<WorkflowWriterTestExceptionService> logger)
        {
            this.logger = logger;
        }

        internal static void ResetCount()
        {
            callCount = 0;
        }

        public void Method1()
        {
            callCount++;

            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation($"{ nameof(WorkflowWriterTestExceptionService)}.{ nameof(Method1)} called successfully");
        }
    }
}