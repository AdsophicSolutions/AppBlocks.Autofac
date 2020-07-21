using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Common
{
    /// <summary>
    /// Defines methods to turn logging off by type
    /// </summary>
    public interface ILoggingConfiguration
    {
        /// <summary>
        /// Implementation returns true for types excluded from logging
        /// </summary>
        /// <param name="fullTypeName">Full name for type to check</param>
        /// <returns><c>true</c> if type should be excluded; otherwise <c>false</c>.</returns>
        bool IsTypeExcluded(string fullTypeName);
    }
}
