using System;
using System.Collections.Generic;

namespace AppBlocks.Autofac.Support
{
    /// <summary>
    /// Specifies service as an AppBlocks service
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AppBlocksServiceAttribute : AppBlocksServiceBaseAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ServiceDependencyType"><see cref="AppBlocksServiceDependencyType"/> type. Service can be 
        /// <see cref="AppBlocksServiceDependencyType.Live"/> or <see cref="AppBlocksServiceDependencyType.NonLive"/></param>
        /// <param name="Name">Service name</param>
        /// <param name="ServiceType">Type implemented</param>
        /// <param name="ServiceScope"><see cref="AppBlocksInstanceLifetime"/> lifetime scope</param>
        /// <param name="Interceptors">Service interceptors</param>
        /// <param name="Workflows">Type workflow writers</param>
        /// <param name="IsKeyed"><c>true</c> if service is keyed; otherwise <c>false</c></param>
        public AppBlocksServiceAttribute(
            AppBlocksServiceDependencyType ServiceDependencyType = 
                AppBlocksServiceDependencyType.NonLive,
            string Name = "",
            Type ServiceType = null,
            AppBlocksInstanceLifetime ServiceScope = 
                AppBlocksInstanceLifetime.InstancePerLifetimeScope,
            string[] Interceptors = null,
            string[] Workflows = null,
            bool IsKeyed = false) : base(ServiceDependencyType, ServiceScope)
        {   
            this.Name = Name;
            this.ServiceType = ServiceType;            
            this.Interceptors =
                Interceptors ?? new[]
                    {
                        AppBlocksInterceptorConstants.Logging,
                        AppBlocksInterceptorConstants.Validation
                    };
            this.Workflows = Workflows;
            this.IsKeyed = IsKeyed;

            //Keyed services must define service type
            if (IsKeyed && ServiceType == null)
            {
                throw new ArgumentException("Keyed services must define service type");
            }
        }

        /// <summary>
        /// Service Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Service workflows
        /// </summary>
        public string[] Workflows { get; }

        /// <summary>
        /// Type implemented
        /// </summary>
        public Type ServiceType { get; }        

        /// <summary>
        /// Service interceptors
        /// </summary>
        public IEnumerable<string> Interceptors { get; }

        /// <summary>
        /// Specifies if service is keyed or not
        /// </summary>
        public bool IsKeyed { get; }
    }
}
