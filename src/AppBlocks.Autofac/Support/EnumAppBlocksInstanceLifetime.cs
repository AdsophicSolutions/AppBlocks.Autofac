using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Support
{
    public enum EnumAppBlocksInstanceLifetime
    {
        SingleInstance,
        InstancePerLifetimeScope,        
        InstancePerDependency
    }
}
