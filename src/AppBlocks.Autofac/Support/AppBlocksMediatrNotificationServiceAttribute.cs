using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Support
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AppBlocksMediatrNotificationServiceAttribute : AppBlocksServiceAttributeBase
    {
        public AppBlocksMediatrNotificationServiceAttribute() : this(AppBlocksServiceDependencyType.NonLive) { }

        public AppBlocksMediatrNotificationServiceAttribute(AppBlocksServiceDependencyType ServiceDependencyType)
            : base(ServiceDependencyType, AppBlocksInstanceLifetime.InstancePerLifetimeScope)
        {            
        }
    }
}
