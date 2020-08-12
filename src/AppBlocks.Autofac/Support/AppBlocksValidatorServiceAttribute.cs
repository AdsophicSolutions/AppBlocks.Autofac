using AppBlocks.Autofac.Common;
using System;

namespace AppBlocks.Autofac.Support
{
    /// <summary>
    /// Specifies type is a validator
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AppBlocksValidatorServiceAttribute : AppBlocksServiceAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ClassFullName">Full name of type validated</param>
        public AppBlocksValidatorServiceAttribute(string ClassFullName) :
            base(
                AppBlocksServiceDependencyType.NonLive,
                ClassFullName,
                typeof(IServiceValidator),
                AppBlocksInstanceLifetime.SingleInstance,
                Array.Empty<string>(),
                Array.Empty<string>(),
                true)
        {
            if (string.IsNullOrWhiteSpace(ClassFullName)) throw new Exception("ClassFullName Name cannot be null or whitespace");
            this.ClassFullName = ClassFullName;
        }

        /// <summary>
        /// Service type validated
        /// </summary>
        public string ClassFullName { get; }
    }
}
