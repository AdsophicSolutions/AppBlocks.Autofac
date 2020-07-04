using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Support
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AppBlocksServiceAttribute : Attribute
    {
        public AppBlocksServiceAttribute(
            string Name = "",
            Type ServiceType = null,
            EnumAppBlocksInstanceLifetime ServiceScope = 
                EnumAppBlocksInstanceLifetime.InstancePerLifetimeScope,
            string[] Interceptors = null,
            string[] Workflows = null,
            bool IsKeyed = false)
        {
            this.Name = Name;
            this.ServiceType = ServiceType;
            this.ServiceScope = ServiceScope;
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
        public EnumAppBlocksInstanceLifetime ServiceScope { get; }
        public IEnumerable<string> Interceptors { get; }
        public bool IsKeyed { get; }
    }
}
