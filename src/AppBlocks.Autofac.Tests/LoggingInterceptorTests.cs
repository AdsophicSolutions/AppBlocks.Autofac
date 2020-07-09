using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Tests
{
    [TestClass]
    public class LoggingInterceptorTests
    {
        [TestMethod]
        public void Logging_Service_Test()
        {
            var containerBuilder = new TestContainerBuilder();
            var autofacContainer = containerBuilder.BuildContainer();

            LoggingTestServiceLogger.ResetCount(); 
            using (var scope = autofacContainer.BeginLifetimeScope())
            {
                var service = scope.Resolve<ILoggingTestService>();
                service.Method1();
            }

            Assert.AreEqual(expected: 1, actual: LoggingTestServiceLogger.GetPreInvocationCallCount());
            Assert.AreEqual(expected: 1, actual: LoggingTestServiceLogger.GetPostInvocationCallCount());
        }

        [TestMethod]
        public void Logging_Service_Exception_Test()
        {
            var containerBuilder = new TestContainerBuilder();
            var autofacContainer = containerBuilder.BuildContainer();

            LoggingTestExceptionServiceLogger.ResetCount();
            LoggingTestExceptionService.ResetCount();

            using (var scope = autofacContainer.BeginLifetimeScope())
            {
                var service = scope.Resolve<ILoggingTestExceptionService>();
                service.Method1();
                service.Method1();
            }

            Assert.AreEqual(expected: 2, actual: LoggingTestExceptionService.GetCallCount());            
            Assert.AreEqual(expected: 1, actual: LoggingTestExceptionServiceLogger.GetPreInvocationCallCount());
            Assert.AreEqual(expected: 1, actual: LoggingTestExceptionServiceLogger.GetPostInvocationCallCount());
        }
    }
}
