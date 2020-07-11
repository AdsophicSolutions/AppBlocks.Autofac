using AppBlocks.Autofac.Common;
using AppBlocks.Autofac.Support;
using Autofac.Features.Indexed;
using Castle.DynamicProxy;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppBlocks.Autofac.Interceptors
{
    [AppBlocksService(
        Name: "",
        ServiceType: null,
        ServiceScope: AppBlocksInstanceLifetime.SingleInstance,
        Interceptors: new string[0],
        Workflows: null,
        IsKeyed: false)]
    public class WorkflowInterceptor : IWorkflowInterceptor
    {   
        private readonly IIndex<string, IWorkflowWriter> workflowWriters;
        private readonly Lazy<Dictionary<string, Dictionary<string, IWorkflowWriter>>> workflowWriterServiceDictionary =
            new Lazy<Dictionary<string, Dictionary<string, IWorkflowWriter>>>(() => new Dictionary<string, Dictionary<string, IWorkflowWriter>>());
        private readonly HashSet<string> disabledWorkflowWriters = 
            new HashSet<string>();

        public WorkflowInterceptor(IIndex<string, IWorkflowWriter> workflowWriters)
        {            
            this.workflowWriters = workflowWriters;
        }

        public void PostMethodInvoke(IInvocation invocation)
        {
            var writers = GetWorkflowWriters(invocation);
            if (writers == null) return;

            foreach (var writer in writers)
            {
                if (disabledWorkflowWriters.Contains(writer.Key)) continue;

                try
                {
                    writer.Value.PostMethodInvocationOutput(invocation);
                }
                catch(Exception e)
                {
                    if (Logger.IsErrorEnabled)
                    {
                        Logger.Error($"Workflow writer {writer.Key}:{writer.Value.GetType().FullName} threw an exception during PostMethodInvoke method call. " +
                            $"Writer will be disabled", e);
                    }

                    disabledWorkflowWriters.Add(writer.Key);
                }
            }
        }

        public void PreMethodInvoke(IInvocation invocation)
        {
            var writers = GetWorkflowWriters(invocation);
            if (writers == null) return;

            foreach (var writer in writers)
            {
                if (disabledWorkflowWriters.Contains(writer.Key)) continue;

                try
                {
                    writer.Value.PreMethodInvocationOutput(invocation);
                }
                catch (Exception e)
                {
                    if (Logger.IsErrorEnabled)
                    {
                        Logger.Error($"Workflow writer {writer.Key}:{writer.Value.GetType().FullName} threw an exception during PreMethodInvoke method call. " +
                            $"Writer will be disabled", e);
                    }

                    disabledWorkflowWriters.Add(writer.Key);
                }
            }
        }

        private Dictionary<string, IWorkflowWriter> GetWorkflowWriters(IInvocation invocation)
        {
            if (workflowWriterServiceDictionary.Value.TryGetValue(
                invocation.TargetType.FullName, out Dictionary<string, IWorkflowWriter> writers))
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
                workflowWriterServiceDictionary.Value[invocation.TargetType.FullName] = null;
                return null;
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
                        writers = new Dictionary<string, IWorkflowWriter>();
                        workflowWriterServiceDictionary.Value[invocation.TargetType.FullName] = writers;
                    }

                    writers.Add(workflow, writer);
                }
            }

            return workflowWriterServiceDictionary.Value[invocation.TargetType.FullName];
        }

        public ILog Logger { get; set; }
    }
}
