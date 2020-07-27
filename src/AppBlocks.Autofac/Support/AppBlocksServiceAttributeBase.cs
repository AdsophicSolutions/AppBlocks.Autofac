using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Support
{
    /// <summary>
    /// Base class for specifying AppBlocks service 
    /// </summary>
    public abstract class AppBlocksServiceAttributeBase : Attribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ServiceDependencyType"><see cref="AppBlocksServiceDependencyType"/> type. Service can be 
        /// <see cref="AppBlocksServiceDependencyType.Live"/> or <see cref="AppBlocksServiceDependencyType.NonLive"/></param>
        /// <param name="ServiceScope"><see cref="AppBlocksInstanceLifetime"/> lifetime scope</param>
        public AppBlocksServiceAttributeBase(AppBlocksServiceDependencyType ServiceDependencyType, 
            AppBlocksInstanceLifetime ServiceScope)
        {
            this.ServiceDependencyType = ServiceDependencyType;
            this.ServiceScope = ServiceScope;
        }

        /// <summary>
        /// <see cref="AppBlocksServiceDependencyType"/> type. Service can be 
        /// <see cref="AppBlocksServiceDependencyType.Live"/> or <see cref="AppBlocksServiceDependencyType.NonLive"/>
        /// </summary>
        public AppBlocksServiceDependencyType ServiceDependencyType { get; }

        /// <summary>
        /// <see cref="AppBlocksInstanceLifetime"/> lifetime scope
        /// </summary>
        public AppBlocksInstanceLifetime ServiceScope { get; }
    }
}
