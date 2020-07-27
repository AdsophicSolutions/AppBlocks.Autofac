using System;
using System.Collections.Generic;

namespace AppBlocks.Autofac.Support
{   
    /// <summary>
    /// Specifies service as a named service
    /// </summary>
    public class AppBlocksNamedServiceAttribute : AppBlocksServiceAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Name">Service name</param>
        /// <param name="ServiceDependencyType"><see cref="AppBlocksServiceDependencyType"/> type. Service can be 
        /// <see cref="AppBlocksServiceDependencyType.Live"/> or <see cref="AppBlocksServiceDependencyType.NonLive"/></param>
        /// <param name="ServiceType">Type implemented</param>
        /// <param name="ServiceScope"><see cref="AppBlocksInstanceLifetime"/> lifetime scope</param>
        /// <param name="Interceptors">Service interceptors</param>
        /// <param name="Workflows">Type workflow writers</param>
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
