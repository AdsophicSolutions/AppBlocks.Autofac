using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Support
{
    /// <summary>
    /// Specifies if a service has a live dependency. A dependency is considered live if it retrieves data via an external source. In <see cref=" Common.AppBlocksApplicationMode.Test"/> mode
    /// live services are not registered. 
    /// </summary>
    public enum AppBlocksServiceDependencyType
    {
        /// <summary>
        /// Specify service as a live service. Live services are not registered in <see cref=" Common.AppBlocksApplicationMode.Test"/> mode
        /// </summary>
        Live, 

        /// <summary>
        /// Specifies service as a NonLive service. NonLive services are registered in both modes
        /// </summary>
        NonLive
    }
}
