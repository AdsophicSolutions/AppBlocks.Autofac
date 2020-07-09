using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Tests.Validation
{
    [TestClass]
    public class ValidationInterceptorTests
    {
        [TestMethod]
        public void Validation_Service_Test()
        {
            var containerBuilder = new TestContainerBuilder();
            var autofacContainer = containerBuilder.BuildContainer();

            ValidationTestServiceValidator.ResetCount();
            using (var scope = autofacContainer.BeginLifetimeScope())
            {
                var service = scope.Resolve<IValidationTestService>();
                service.Method1();
            }

            Assert.AreEqual(expected: 1, actual: ValidationTestServiceValidator.GetInputParametersCallCount());
            Assert.AreEqual(expected: 1, actual: ValidationTestServiceValidator.GetResultCallCount());
        }

        [TestMethod]
        public void Validation_Service_Exception_Test()
        {
            var containerBuilder = new TestContainerBuilder();
            var autofacContainer = containerBuilder.BuildContainer();

            ValidationTestExceptionServiceValidator.ResetCount();
            ValidationTestExceptionService.ResetCount();

            using (var scope = autofacContainer.BeginLifetimeScope())
            {
                var service = scope.Resolve<IValidationTestExceptionService>();
                service.Method1();
                service.Method1();
            }

            Assert.AreEqual(expected: 2, actual: ValidationTestExceptionService.GetCallCount());
            Assert.AreEqual(expected: 1, actual: ValidationTestExceptionServiceValidator.GetInputParametersCallCount());
            Assert.AreEqual(expected: 1, actual: ValidationTestExceptionServiceValidator.GetResultCallCount());
        }
    }
}
