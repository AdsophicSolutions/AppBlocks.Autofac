using Castle.DynamicProxy;
using System.Linq;
using System.Reflection;

namespace AppBlocks.Autofac.Common
{
    public static class Utility
    {
        public static bool IsAssembly(string assemblyPath)
        {
            try
            {
                // Attempt to resolve the assembly
                var assembly = AssemblyName.GetAssemblyName(assemblyPath);
                return true;
            }
            catch { }
            return false;
        }

        public static object GetAsyncInvocationResult(IInvocation invocation)
        {
            var resultMethod = invocation
                .ReturnValue
                .GetType()
                .GetMethods()
                .FirstOrDefault(n => n.Name == "get_Result");

            return resultMethod?.Invoke(invocation.ReturnValue, null);
        }
    }
}
