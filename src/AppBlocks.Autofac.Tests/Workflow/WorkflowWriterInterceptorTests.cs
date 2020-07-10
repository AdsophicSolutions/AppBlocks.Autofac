using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Tests.Workflow
{
    [TestClass]
    public class WorkflowWriterInterceptorTests
    {
        [TestMethod]
        public void WorkflowWriter_Service_Test()
        {
            var containerBuilder = new TestContainerBuilder();
            var autofacContainer = containerBuilder.BuildContainer();

            WorkflowWriterTestServiceWriter.ResetCount();
            using (var scope = autofacContainer.BeginLifetimeScope())
            {
                var service = scope.Resolve<IWorkflowWriterTestService>();
                service.Method1();
            }

            Assert.AreEqual(expected: 1, actual: WorkflowWriterTestServiceWriter.GetPreInvocationCallCount());
            Assert.AreEqual(expected: 1, actual: WorkflowWriterTestServiceWriter.GetPostInvocationCallCount());
        }

        [TestMethod]
        public void WorkflowWriter_Service_Exception_Test()
        {
            var containerBuilder = new TestContainerBuilder();
            var autofacContainer = containerBuilder.BuildContainer();

            WorkflowWriterTestExceptionServiceWriter.ResetCount();
            WorkflowWriterTestExceptionService.ResetCount();

            using (var scope = autofacContainer.BeginLifetimeScope())
            {
                var service = scope.Resolve<IWorkflowWriterTestExceptionService>();
                service.Method1();
                service.Method1();
            }

            Assert.AreEqual(expected: 2, actual: WorkflowWriterTestExceptionService.GetCallCount());
            Assert.AreEqual(expected: 1, actual: WorkflowWriterTestExceptionServiceWriter.GetPreInvocationCallCount());
            Assert.AreEqual(expected: 1, actual: WorkflowWriterTestExceptionServiceWriter.GetPostInvocationCallCount());
        }
    }
}
