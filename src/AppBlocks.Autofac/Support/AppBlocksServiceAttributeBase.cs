using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Support
{
    public abstract class AppBlocksServiceAttributeBase : Attribute
    {
        public AppBlocksServiceAttributeBase(AppBlocksServiceDependencyType ServiceDependencyType, 
            AppBlocksInstanceLifetime ServiceScope)
        {
            this.ServiceDependencyType = ServiceDependencyType;
            this.ServiceScope = ServiceScope;
        }

        public AppBlocksServiceDependencyType ServiceDependencyType { get; }
        public AppBlocksInstanceLifetime ServiceScope { get; }
    }
}
