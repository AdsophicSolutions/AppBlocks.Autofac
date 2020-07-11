using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Support
{
    public class AppBlocksLiveServiceAttribute : AppBlocksServiceAttribute
    {
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
