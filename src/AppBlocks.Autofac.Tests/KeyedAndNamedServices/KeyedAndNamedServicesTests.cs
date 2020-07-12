using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Tests.KeyedAndNamedServices
{
    [TestClass]
    public class KeyedAndNamedServicesTests
    {
        private static IContainer autofacContainer;

        [ClassInitialize]
        public static void InitSimpleServiceTest(TestContext testContext)
        {
            var containerBuilder = new TestContainerBuilder();
            autofacContainer = containerBuilder.BuildContainer();
        }

        [TestMethod]
        public void Named_Service_Test()
        {
            using var scope = autofacContainer.BeginLifetimeScope();
            NamedService.ResetCount();
            var service = 
                scope.ResolveNamed<INamedService>("AppBlocks.Autofac.Tests.KeyedAndNamedServices.NamedService");

            service.RunService();

            Assert.AreEqual(expected: 1, actual: NamedService.GetCallCount());
        }

        [TestMethod]
        public void Keyed_Service_Test()
        {
            using var scope = autofacContainer.BeginLifetimeScope();
            KeyedServiceReceiver.ResetCount();
            KeyedService1.ResetCount();
            KeyedService2.ResetCount();

            var service =
                scope.Resolve<IKeyedServiceReceiver>();

            service.RunKeyedServices();

            Assert.AreEqual(expected: 1, actual: KeyedServiceReceiver.GetCallCount());
            Assert.AreEqual(expected: 1, actual: KeyedService1.GetCallCount());
            Assert.AreEqual(expected: 1, actual: KeyedService2.GetCallCount());
        }
    }
}
