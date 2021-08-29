using AppBlocks.Autofac.Common;
using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Tests
{
    public class TestContainerBuilder : AppBlocksContainerBuilder
    {
        public TestContainerBuilder(ApplicationConfiguration applicationConfiguration) 
            : base(applicationConfiguration, AppBlocksApplicationMode.Test) { }

        public TestContainerBuilder() : base(AppBlocksApplicationMode.Test) { }

        protected override void RegisterAssemblyServices(ContainerBuilder builder)
        {
            RegisterAssembly(typeof(TestContainerBuilder).Assembly, builder);
        }
    }
}
