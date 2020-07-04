using AppBlocks.Autofac.Common;
using AppBlocks.Autofac.Support;
using Autofac.Features.Indexed;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppBlocks.Autofac.Interceptors
{
    public class WorkflowInterceptor : AutofacInterceptorBase
    {   
        private readonly IIndex<string, IWorkflowWriter> workflowWriters;
        private Lazy<Dictionary<string, IList<IWorkflowWriter>>> workflowWriterServiceDictionary =
            new Lazy<Dictionary<string, IList<IWorkflowWriter>>>(() => new Dictionary<string, IList<IWorkflowWriter>>());

        public WorkflowInterceptor(IIndex<string, IWorkflowWriter> workflowWriters)
        {            
            this.workflowWriters = workflowWriters;
        }

        protected override void PostMethodInvoke(IInvocation invocation)
        {
            var writers = GetWorkflowWriters(invocation);
            foreach (var writer in writers)
            {
                writer.PostMethodInvocationOutput(invocation);
            }
        }

        protected override void PreMethodInvoke(IInvocation invocation)
        {
            var writers = GetWorkflowWriters(invocation);
            foreach (var writer in writers)
            {
                writer.PreMethodInvocationOutput(invocation);
            }
        }

        private IEnumerable<IWorkflowWriter> GetWorkflowWriters(IInvocation invocation)
        {
            if (workflowWriterServiceDictionary.Value.TryGetValue(
                invocation.TargetType.FullName, out IList<IWorkflowWriter> writers))
            {
                return writers;
            }

            var appblocksServiceAttribute = invocation
                .TargetType                
                .GetCustomAttributes<AppBlocksServiceAttribute>();
            if (appblocksServiceAttribute.Count() == 0)
            {
                throw new ArgumentException($"Oops! Not sure how you ended up here, but service is not decorated with AutofacServiceAttribute");
            }

            var typedServiceAttribute = appblocksServiceAttribute.First();
            if ((typedServiceAttribute.Workflows ?? new string[0]).Count() == 0)
            {
                throw new ArgumentException($"Oops! Not sure how you ended up here, but AutofacServiceAttribute definition does not have a valid workflow name");
            }

            if (!typedServiceAttribute.Workflows.All(
                w => workflowWriters.TryGetValue(w, out IWorkflowWriter writer)))
            {
                throw new ArgumentException($"No workflow writers defined for workflows" +
                    $"{string.Join(",", typedServiceAttribute.Workflows.Where(w => !workflowWriters.TryGetValue(w, out IWorkflowWriter workflowWriter)))}." +
                    $" set using AutofacServiceAttribute on Type {invocation.TargetType.FullName}");
            }

            foreach (string workflow in typedServiceAttribute.Workflows)
            {
                if (workflowWriters.TryGetValue(workflow, out IWorkflowWriter writer))
                {
                    writers = null;
                    if (!workflowWriterServiceDictionary.Value.TryGetValue(invocation.TargetType.FullName,
                        out writers))
                    {
                        writers = new List<IWorkflowWriter>();
                        workflowWriterServiceDictionary.Value[invocation.TargetType.FullName] = writers;
                    }

                    writers.Add(writer);
                }
            }

            return workflowWriterServiceDictionary.Value[invocation.TargetType.FullName];
        }
    }
}
