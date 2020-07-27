using AppBlocks.Autofac.Common;
using System;

namespace AppBlocks.Autofac.Support
{
    /// <summary>
    /// Specifies type is a workflow writer
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AppBlocksWorkflowWriterServiceAttribute : AppBlocksServiceAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="WorkflowName">Workflow name to support</param>
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

        /// <summary>
        /// Workflow supported
        /// </summary>
        public string WorkflowName { get; }
    }
}
