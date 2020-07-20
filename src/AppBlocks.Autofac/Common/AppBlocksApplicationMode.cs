using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Common
{
    /// <summary>
    /// Specifies the AppBlocks application mode
    /// </summary>
    public enum AppBlocksApplicationMode
    {
        /// <summary>
        /// Services marked with Live <see cref="Support.AppBlocksServiceDependencyType"/> 
        /// are registered in this mode
        /// </summary>
        Live,

        /// <summary>
        /// Services marked with Live <see cref="Support.AppBlocksServiceDependencyType"/> 
        /// are not registered in this mode
        /// </summary>
        Test
    }
}
