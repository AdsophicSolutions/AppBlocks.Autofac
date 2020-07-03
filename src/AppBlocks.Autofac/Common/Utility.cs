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
    }
}
