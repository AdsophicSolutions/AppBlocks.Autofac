using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Tests.MediatR
{
    [TestClass]
    public class MediatRServicesTests
    {
        [TestMethod]
        public void Request_Response_Test()
        {
            var containerBuilder = new TestContainerBuilder();
            var autofacContainer = containerBuilder.BuildContainer();

            MediatRReceiverService.ResetCount();
            RequestResponseService.ResetCount();

            using (var scope = autofacContainer.BeginLifetimeScope())
            {
                var service = scope.Resolve<IMediatRReceiverService>();
                service.RunService();
            }
            
            Assert.AreEqual(expected: 1, actual: RequestResponseService.GetCallCount());            
            Assert.AreEqual(expected: 1, actual: MediatRReceiverService.GetCallCount());
        }
    }
}
