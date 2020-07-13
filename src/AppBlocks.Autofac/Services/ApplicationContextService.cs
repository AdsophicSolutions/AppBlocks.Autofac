using AppBlocks.Autofac.Common;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace AppBlocks.Autofac.Services
{
    internal sealed class ApplicationContextService : IContext
    {
        private readonly Dictionary<string, object> context =
            new Dictionary<string, object>();

        public ApplicationContextService(ApplicationConfiguration configuration)
        {
            Initialize(configuration);
        }

        private void Initialize(ApplicationConfiguration configuration)
        {
            foreach (string configurationFilePath in configuration.ConfigurationFilePaths.Value)
            {
                var builder = new ConfigurationBuilder()
                    .AddJsonFile(configurationFilePath);

                var jsonConfiguration = builder.Build();
                var appContext = jsonConfiguration
                        .GetSection("appContext")
                        ?.GetChildren()
                        ?.ToDictionary(x => x.Key, x => x.Value);

                if(appContext != null)
                {
                    foreach(var kvp in appContext)
                    {
                        if (!context.ContainsKey(kvp.Key)) 
                            context.Add(kvp.Key, kvp.Value);
                    }
                }
            }
        }

        public object this[string key] 
        { 
            get => context[key]; 
            set => context[key] = value; 
        }

        public bool ContainsKey(string key) => context.ContainsKey(key);

        public bool TryGetValue(string key, out object value) => context.TryGetValue(key, out value);
    }
}
