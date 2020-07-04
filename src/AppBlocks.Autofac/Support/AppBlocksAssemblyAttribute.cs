using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Support
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class AppBlocksAssemblyAttribute : Attribute
    {
    }
}
