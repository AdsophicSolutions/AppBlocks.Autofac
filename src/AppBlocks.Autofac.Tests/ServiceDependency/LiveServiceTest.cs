using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Tests.ServiceDependency
{
    [TestClass]
    public class LiveServiceTest
    {
        [TestMethod]
        public void Not_Executed_In_Test_Mode()
        {
            var containerBuilder = new TestContainerBuilder();
            var autofacContainer = containerBuilder.BuildContainer();

            Service.ResetCount();
            LiveService.ResetCount();

            using (var scope = autofacContainer.BeginLifetimeScope())
            {
                var service = scope.Resolve<IService>();
                service.RunService();
            }

            // Service will be called once
            Assert.AreEqual(expected: 1, actual: Service.GetCallCount());

            // Live service implementation should not be called. 
            Assert.AreEqual(expected: 0, actual: LiveService.GetCallCount());
        }
    }
}
