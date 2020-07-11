namespace AppBlocks.Autofac.Support
{
    public enum AppBlocksInstanceLifetime
    {
        SingleInstance,
        InstancePerLifetimeScope,        
        InstancePerDependency
    }
}
