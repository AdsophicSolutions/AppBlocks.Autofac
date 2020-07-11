using AppBlocks.Autofac.Common;
using System;

namespace AppBlocks.Autofac.Support
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AppBlocksWorkflowWriterServiceAttribute : AppBlocksServiceAttribute
    {
        public AppBlocksWorkflowWriterServiceAttribute(string WorkflowName) :
             base(
                    AppBlocksServiceDependencyType.NonLive,
                    WorkflowName,
                    typeof(IWorkflowWriter),
                    AppBlocksInstanceLifetime.SingleInstance,
                    new string[0],
                    new string[0],
                    true)
        {
            if (string.IsNullOrWhiteSpace(WorkflowName)) throw new Exception("Workflow Name cannot be null or whitespace");
            this.WorkflowName = WorkflowName;
        }

        public string WorkflowName { get; }
    }
}
