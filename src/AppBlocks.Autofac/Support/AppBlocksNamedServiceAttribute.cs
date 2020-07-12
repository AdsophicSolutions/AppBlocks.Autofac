using System;
using System.Collections.Generic;

namespace AppBlocks.Autofac.Support
{   
    public class AppBlocksNamedServiceAttribute : AppBlocksServiceAttribute
    {
        public AppBlocksNamedServiceAttribute(
            string Name,
            AppBlocksServiceDependencyType ServiceDependencyType = 
                AppBlocksServiceDependencyType.NonLive,            
            Type ServiceType = null,
            AppBlocksInstanceLifetime ServiceScope = 
                AppBlocksInstanceLifetime.InstancePerLifetimeScope,
            string[] Interceptors = null,
            string[] Workflows = null) : base(
                ServiceDependencyType,
                Name, 
                ServiceType,
                ServiceScope,
                Interceptors, 
                Workflows,
                false)
        {
            
        }
    }
}
