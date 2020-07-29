﻿using AppBlocks.Autofac.Common;
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
    /// <summary>
    /// WorkflowInterceptor calls registered workflow writers before and after
    /// Conceptually workflow writer should be used across the application to tie 
    /// together data workflow.
    /// </summary>
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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="workflowWriters">Keyed instances of <see cref="IWorkflowWriter"/></param>
        public WorkflowInterceptor(IIndex<string, IWorkflowWriter> workflowWriters)
        {            
            this.workflowWriters = workflowWriters;
        }

        /// <summary>
        /// Called before service method
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/> instance</param>
        public void PreMethodInvoke(IInvocation invocation)
        {
            // Get workflow writers for service 
            var writers = GetWorkflowWriters(invocation);

            // No writers setup for service
            if (writers == null) return;

            // Iterate through each writer
            foreach (var writer in writers)
            {
                // Ignore disabled writers
                if (disabledWorkflowWriters.Contains(writer.Key)) continue;

                try
                {
                    // Call writer method
                    writer.Value.PreMethodInvocationOutput(invocation);
                }
                catch (Exception e)
                {
                    // Log any exceptions
                    if (Logger.IsErrorEnabled)
                    {
                        Logger.Error($"Workflow writer {writer.Key}:{writer.Value.GetType().FullName} threw an exception during PreMethodInvoke method call. " +
                            $"Writer will be disabled", e);
                    }

                    // Disable workflow writer if it throws an exception
                    disabledWorkflowWriters.Add(writer.Key);
                }
            }
        }

        /// <summary>
        /// Called after service method returns
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/> instance</param>
        public void PostMethodInvoke(IInvocation invocation)
        {
            // Get workflow writers for service type
            var writers = GetWorkflowWriters(invocation);

            // No workflow writers for service
            if (writers == null) return;

            // Iterate through service type writers
            foreach (var writer in writers)
            {
                // Ignore disabled writers
                if (disabledWorkflowWriters.Contains(writer.Key)) continue;

                try
                {
                    // Call writer with service return value
                    writer.Value.PostMethodInvocationOutput(invocation);
                }
                catch (Exception e)
                {
                    // Log any errors
                    if (Logger.IsErrorEnabled)
                    {
                        Logger.Error($"Workflow writer {writer.Key}:{writer.Value.GetType().FullName} threw an exception during PostMethodInvoke method call. " +
                            $"Writer will be disabled", e);
                    }

                    // Disable writer. Writers that throw exceptions are disabled
                    disabledWorkflowWriters.Add(writer.Key);
                }
            }
        }

        /// <summary>
        /// Gets workflow writers for service 
        /// </summary>        
        private Dictionary<string, IWorkflowWriter> GetWorkflowWriters(
            IInvocation invocation)
        {
            // Check if writers for this service are already initialized
            // If initialized, return writers
            if (workflowWriterServiceDictionary.Value.TryGetValue(
                invocation.TargetType.FullName, out Dictionary<string, IWorkflowWriter> writers))
            {
                return writers;
            }

            // Get AppBlocks service attribute for service
            var appblocksServiceAttribute = invocation
                .TargetType                
                .GetCustomAttributes<AppBlocksServiceAttribute>();

            // Just in case check. This should not happen. All AppBlock services
            // must be decorated with service attribute
            if (appblocksServiceAttribute.Count() == 0)
            {
                throw new ArgumentException($"Oops! Not sure how you ended up here, but service is not decorated with AutofacServiceAttribute");
            }

            // Get attribute reference
            var typedServiceAttribute = appblocksServiceAttribute.First();

            // Look for workflow names for service. Initialize as none 
            // if no workflows are set
            if ((typedServiceAttribute.Workflows ?? new string[0]).Count() == 0)
            {
                // initialize type with no workflows.  
                workflowWriterServiceDictionary.Value[invocation.TargetType.FullName] = null;
                return null;
            }

            // Workflow writers must be registered for each of the requested workflows
            if (!typedServiceAttribute.Workflows.All(
                w => workflowWriters.TryGetValue(w, out IWorkflowWriter writer)))
            {               
                throw new ArgumentException($"No workflow writers defined for workflows" +
                    $"{string.Join(",", typedServiceAttribute.Workflows.Where(w => !workflowWriters.TryGetValue(w, out IWorkflowWriter workflowWriter)))}." +
                    $" set using AutofacServiceAttribute on Type {invocation.TargetType.FullName}");
            }

            // Setup workflow writers for service
            foreach (string workflow in typedServiceAttribute.Workflows)
            {
                // Look up workflow writer
                if (workflowWriters.TryGetValue(workflow, out IWorkflowWriter writer))
                {
                    writers = null;

                    // Create dictionary for service type. We need to store the 
                    // writers as a dictionary because we want the ability to ignore writers
                    // that have been disabled. 
                    if (!workflowWriterServiceDictionary.Value.TryGetValue(invocation.TargetType.FullName,
                        out writers))
                    {
                        // Initialize new dictionary
                        writers = new Dictionary<string, IWorkflowWriter>();

                        // Set dictionary
                        workflowWriterServiceDictionary.Value[invocation.TargetType.FullName] = writers;
                    }

                    // Add writer for service type
                    writers.Add(workflow, writer);
                }
            }

            // return writer registered for service types
            return 
                workflowWriterServiceDictionary.Value[invocation.TargetType.FullName];
        }

        /// <summary>
        /// <see cref="ILog"/> instance
        /// </summary>
        public ILog Logger { get; set; }
    }
}
