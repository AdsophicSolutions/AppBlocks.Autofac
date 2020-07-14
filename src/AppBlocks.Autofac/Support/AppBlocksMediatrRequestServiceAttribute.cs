using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Support
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AppBlocksMediatrRequestServiceAttribute : AppBlocksServiceAttributeBase
    {
        public AppBlocksMediatrRequestServiceAttribute() : this(AppBlocksServiceDependencyType.NonLive) { }

        public AppBlocksMediatrRequestServiceAttribute(AppBlocksServiceDependencyType ServiceDependencyType)
            : base(ServiceDependencyType, AppBlocksInstanceLifetime.InstancePerLifetimeScope)
        {            
        }
    }
}
