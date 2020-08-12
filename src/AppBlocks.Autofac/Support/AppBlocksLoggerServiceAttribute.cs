using AppBlocks.Autofac.Common;
using System;

namespace AppBlocks.Autofac.Support
{
    /// <summary>
    /// Specifies that a service is a custom logger
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AppBlocksLoggerServiceAttribute : AppBlocksServiceAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Name">Type logged by service</param>
        public AppBlocksLoggerServiceAttribute(string Name) :
            base(
                AppBlocksServiceDependencyType.NonLive,
                Name,
                typeof(IServiceLogger),
                AppBlocksInstanceLifetime.SingleInstance,
                Array.Empty<string>(),
                Array.Empty<string>(),
                true)
        {
            if (string.IsNullOrWhiteSpace(Name)) throw new Exception("Name cannot be null or whitespace");
        }
    }
}
