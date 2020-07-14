using System;
using System.Collections.Generic;

namespace AppBlocks.Autofac.Support
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AppBlocksServiceAttribute : AppBlocksServiceAttributeBase
    {
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

        
        public string Name { get; }
        public string[] Workflows { get; }
        public Type ServiceType { get; }        
        public IEnumerable<string> Interceptors { get; }
        public bool IsKeyed { get; }
    }
}
