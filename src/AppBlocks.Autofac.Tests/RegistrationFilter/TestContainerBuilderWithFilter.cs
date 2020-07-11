using AppBlocks.Autofac.Common;
using AppBlocks.Autofac.Support;
using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Tests.RegistrationFilter
{
    public class TestContainerBuilderWithFilter : AppBlocksContainerBuilder
    {
        public TestContainerBuilderWithFilter() : base(AppBlocksApplicationMode.Test) { }

        protected override void RegisterAssemblyServices(ContainerBuilder builder)
        {
            RegisterAssembly(typeof(TestContainerBuilderWithFilter).Assembly, builder);
            base.RegisterAssemblyServices(builder);
        }

        protected override bool ShouldRegisterService(Type type, AppBlocksServiceAttribute serviceAttribute)
        {
            if (type.FullName == "AppBlocks.Autofac.Tests.RegistrationFilter.TestFilteredOutService") return false;

            return base.ShouldRegisterService(type, serviceAttribute);
        }
    }
}
