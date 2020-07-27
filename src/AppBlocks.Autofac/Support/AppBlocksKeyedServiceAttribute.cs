using System;
using System.Collections.Generic;

namespace AppBlocks.Autofac.Support
{   
    /// <summary>
    /// Specifies a type as a AppBlocks keyed service
    /// </summary>
    public class AppBlocksKeyedServiceAttribute : AppBlocksServiceAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ServiceKey">Keyed service key</param>
        /// <param name="ServiceType">Type implemented</param>
        /// <param name="ServiceDependencyType"><see cref="AppBlocksServiceDependencyType"/> type. Service can be 
        /// <see cref="AppBlocksServiceDependencyType.Live"/> or <see cref="AppBlocksServiceDependencyType.NonLive"/></param>
        /// <param name="ServiceScope"><see cref="AppBlocksInstanceLifetime"/> lifetime scope</param>
        /// <param name="Interceptors">Service interceptors</param>
        /// <param name="Workflows">Type workflow writers</param>
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
