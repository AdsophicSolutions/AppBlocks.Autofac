using Castle.DynamicProxy;
using System.Linq;
using System.Reflection;

namespace AppBlocks.Autofac.Common
{
    /// <summary>
    /// Utility methods
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Check if assembly is a .NET assembly
        /// </summary>
        /// <param name="assemblyPath">Full path to assembly</param>
        /// <returns><c>true</c> if assembly is .NET assembly; otherwise <c>false</c>.</returns>
        public static bool IsAssembly(string assemblyPath)
        {
            try
            {
                // Attempt to resolve the assembly
                var assembly = AssemblyName.GetAssemblyName(assemblyPath);
                return true;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch { }
#pragma warning restore CA1031 // Do not catch general exception types
            return false;
        }

        /// <summary>
        /// Gets result for asynchronous invocation
        /// </summary>
        /// <param name="invocation"><see cref="IInvocation"/> details</param>
        /// <returns>Result from an invocation</returns>
        public static object GetAsyncInvocationResult(IInvocation invocation)
        {
            var resultMethod = invocation?
                .ReturnValue
                .GetType()
                .GetMethods()
                .FirstOrDefault(n => n.Name == "get_Result");

            return resultMethod?.Invoke(invocation?.ReturnValue, null);
        }
    }
}
