using Autofac;
using log4net;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace AppBlocks.Autofac.Tests
{
    [TestClass]
    public class SimpleServiceTest
    {
        private static IContainer autofacContainer;

        [ClassInitialize]
        public static void InitSimpleServiceTest(TestContext testContext)
        {
            var containerBuilder = new TestContainerBuilder();
            autofacContainer = containerBuilder.BuildContainer();
        }

        [TestMethod]
        public void Test_Resolve_And_Run_Service()
        {
            using var scope = autofacContainer.BeginLifetimeScope();
            var service = scope.Resolve<IService>();
            Assert.AreEqual(expected: 0, actual: service.RunService());
        }
    }
}
