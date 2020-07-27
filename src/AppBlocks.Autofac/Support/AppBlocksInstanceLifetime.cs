namespace AppBlocks.Autofac.Support
{
    /// <summary>
    /// Specify lifetime for AppBlocks service
    /// </summary>
    public enum AppBlocksInstanceLifetime
    {
        /// <summary>
        /// Single instance across all scopes. Equivalent to Singleton design pattern
        /// </summary>
        SingleInstance,

        /// <summary>
        /// Single instance per scope. 
        /// </summary>
        InstancePerLifetimeScope,      
        
        /// <summary>
        /// Instance per dependency. New instance of the service is created for each
        /// dependency encountered
        /// </summary>
        InstancePerDependency
    }
}
