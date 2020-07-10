using AppBlocks.Autofac.Support;
using log4net;
using System.Reflection;

namespace AppBlocks.Autofac.Tests.Workflow
{
    [AppBlocksService(Name: "",
        ServiceType: null,
        ServiceScope: EnumAppBlocksInstanceLifetime.InstancePerLifetimeScope,
        Interceptors: new string[0],
        Workflows: new[] { "TestExceptionWorkflowWriter" },
        IsKeyed: false)]
    public class WorkflowWriterTestExceptionService : IWorkflowWriterTestExceptionService
    {
        private static readonly ILog logger =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static int callCount = 0;
        public static int GetCallCount() => callCount;

        internal static void ResetCount()
        {
            callCount = 0;
        }

        public void Method1()
        {
            callCount++;

            if (logger.IsInfoEnabled)
                logger.Info($"{nameof(WorkflowWriterTestExceptionService)}.{nameof(Method1)} called successfully");
        }
    }
}