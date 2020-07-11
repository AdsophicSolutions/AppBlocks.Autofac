using AppBlocks.Autofac.Common;
using System;

namespace AppBlocks.Autofac.Support
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AppBlocksLoggerServiceAttribute : AppBlocksServiceAttribute
    {
        public AppBlocksLoggerServiceAttribute(string Name) :
            base(
                AppBlocksServiceDependencyType.NonLive,
                Name,
                typeof(IServiceLogger),
                AppBlocksInstanceLifetime.SingleInstance,
                new string[0],
                new string[0],
                true)
        {
            if (string.IsNullOrWhiteSpace(Name)) throw new Exception("Name cannot be null or whitespace");
        }
    }
}
