using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Common
{
    public interface ILoggingConfiguration
    {
        bool ShouldUseDefaultLogger(string fullTypeName);
    }
}
