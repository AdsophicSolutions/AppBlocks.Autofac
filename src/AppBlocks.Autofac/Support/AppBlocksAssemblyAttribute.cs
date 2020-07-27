using System;

namespace AppBlocks.Autofac.Support
{
    /// <summary>
    /// Mark assembly AppBlocks assembly. Dynamically loaded assemblies must 
    /// be decorated with this assembly attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class AppBlocksAssemblyAttribute : Attribute
    {
    }
}
