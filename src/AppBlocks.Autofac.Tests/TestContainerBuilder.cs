using AppBlocks.Autofac.Common;
using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Tests
{
    public class TestContainerBuilder : AppBlocksContainerBuilder
    {
        protected override void RegisterAssemblyServices(ContainerBuilder builder)
        {
            RegisterAssembly(typeof(TestContainerBuilder).Assembly, builder);
            base.RegisterAssemblyServices(builder);
        }
    }
}
