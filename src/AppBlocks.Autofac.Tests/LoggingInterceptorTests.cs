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
        public void Create_And_Call_Logging_Interceptor()
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
    }
}
