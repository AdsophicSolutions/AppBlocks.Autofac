using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Support
{
    /// <summary>
    /// Specifies a service as live serivce. A live service is not registered if <see cref="Common.AppBlocksApplicationMode"/> is <see cref="Common.AppBlocksApplicationMode.Test"/>
    /// </summary>
    public class AppBlocksLiveServiceAttribute : AppBlocksServiceAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Name">Service</param>
        /// <param name="ServiceType">Type implemented</param>
        /// <param name="ServiceScope"><see cref="AppBlocksInstanceLifetime"/> lifetime scope</param>
        /// <param name="Interceptors">Service interceptors</param>
        /// <param name="Workflows">Type workflow writers</param>
        /// <param name="IsKeyed">Service is keyed</param>
        public AppBlocksLiveServiceAttribute(            
            string Name = "",
            Type ServiceType = null,
            AppBlocksInstanceLifetime ServiceScope =
                AppBlocksInstanceLifetime.InstancePerLifetimeScope,
            string[] Interceptors = null,
            string[] Workflows = null,
            bool IsKeyed = false) : 
            base(AppBlocksServiceDependencyType.Live, Name, ServiceType, ServiceScope, Interceptors, Workflows, IsKeyed)
        {

        }
    }
}
