using System;
using System.Collections.Generic;

namespace AppBlocks.Autofac.Support
{   
    public class AppBlocksKeyedServiceAttribute : AppBlocksServiceAttribute
    {
        public AppBlocksKeyedServiceAttribute(
            string ServiceKey,
            Type ServiceType,
            AppBlocksServiceDependencyType ServiceDependencyType = 
                AppBlocksServiceDependencyType.NonLive,                        
            AppBlocksInstanceLifetime ServiceScope = 
                AppBlocksInstanceLifetime.InstancePerLifetimeScope,
            string[] Interceptors = null,
            string[] Workflows = null) : base(
                ServiceDependencyType,
                ServiceKey, 
                ServiceType,
                ServiceScope,
                Interceptors, 
                Workflows,
                true)
        {
            
        }
    }
}
