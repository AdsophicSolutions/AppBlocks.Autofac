using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Tests.RegistrationFilter
{
    [TestClass]
    public class RegistrationFilterTests
    {
        [TestMethod]
        public void Filtered_In_Vs_Filtered_Out_Test()
        {
            var containerBuilder = new TestContainerBuilderWithFilter();
            var autofacContainer = containerBuilder.BuildContainer();

            TestFilteredInService.ResetCount();
            TestFilteredOutService.ResetCount();

            using (var scope = autofacContainer.BeginLifetimeScope())
            {
                var service = scope.Resolve<ITestFilterService>();
                service.RunService();
            }

            // Filtered in service is registered and called once
            Assert.AreEqual(expected: 1, actual: TestFilteredInService.GetCallCount());

            // Filtered out service is not registered and hence not called 
            Assert.AreEqual(expected: 0, actual: TestFilteredOutService.GetCallCount());
        }
    }
}
