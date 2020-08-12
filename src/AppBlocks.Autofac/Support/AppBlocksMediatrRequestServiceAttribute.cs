using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Support
{
    /// <summary>
    /// Specifies service is a MediatR request handler
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AppBlocksMediatrRequestServiceAttribute : AppBlocksServiceBaseAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AppBlocksMediatrRequestServiceAttribute() : this(AppBlocksServiceDependencyType.NonLive) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ServiceDependencyType"><see cref="AppBlocksServiceDependencyType"/> type. Service can be 
        /// <see cref="AppBlocksServiceDependencyType.Live"/> or <see cref="AppBlocksServiceDependencyType.NonLive"/></param>
        public AppBlocksMediatrRequestServiceAttribute(AppBlocksServiceDependencyType ServiceDependencyType)
            : base(ServiceDependencyType, AppBlocksInstanceLifetime.InstancePerLifetimeScope)
        {            
        }
    }
}
