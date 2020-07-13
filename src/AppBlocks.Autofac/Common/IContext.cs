using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Common
{
    public interface IContext
    {
        bool ContainsKey(string key);       

        bool TryGetValue(string key, out object value);

        object this[string key] { get; set; }
    }
}
