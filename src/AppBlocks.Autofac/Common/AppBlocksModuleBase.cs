using AppBlocks.Autofac.Interceptors;
using Autofac;
using Autofac.Core;
using System;

namespace AppBlocks.Autofac.Common
{
    /// <summary>
    /// Used to register assemblies as Autofac modules. Inherit from <see cref="AppBlocksModuleBase"/>
    /// and override methods to register Autofac services. 
    /// </summary>
    public abstract class AppBlocksModuleBase : Module
    {
        /// <summary>
        /// Get <see cref="IContext"/> for the AppBlocks application
        /// </summary>
        protected IContext ApplicationContext => AppBlocksContainerBuilder.ApplicationContext;

        /// <summary>
        /// Get <see cref="AppBlocksContainerBuilder"/> for the AppBlocks application
        /// </summary>
        protected AppBlocksContainerBuilder AppBlocksContainerBuilder { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="appBlocksContainerBuilder"><see cref="AppBlocksContainerBuilder"/> for the AppBlocks application</param>
        public AppBlocksModuleBase(AppBlocksContainerBuilder appBlocksContainerBuilder)
        {            
            AppBlocksContainerBuilder = appBlocksContainerBuilder;
        }
        // This is a private constant from the Autofac.Extras.DynamicProxy2 assembly
        // that is needed to "poke" interceptors into registrations.
        //const string InterceptorsPropertyName = "Autofac.Extras.DynamicProxy2.RegistrationExtensions.InterceptorsPropertyName";

        /// <summary>
        /// Override <see cref="global::Autofac.Module.Load(ContainerBuilder)"/>
        /// </summary>
        /// <param name="builder">Instance of <see cref="global::Autofac.ContainerBuilder"/> being initialized</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            //builder.Register(c => new LoggingConfiguration(c.Resolve<ApplicationConfiguration>()))
            //    .As<ILoggingConfiguration>()
            //    .SingleInstance();

            //RegisterInterceptors(builder);
            // Call RegisterExternalServices. Override in inheriting class to 
            // register SingleInstance global services
            RegisterExternalServices(builder);

            // Override in inheriting class to register Assembly services
            RegisterAssemblyServices(builder);
        }

        //private void RegisterInterceptors(ContainerBuilder builder)
        //{
        //    // Typed registration
        //    builder.RegisterType<LoggingInterceptor>().AsSelf().SingleInstance();
        //    builder.RegisterType<ValidationInterceptor>().AsSelf().SingleInstance();
        //    builder.RegisterType<WorkflowInterceptor>().AsSelf().SingleInstance();
        //}

        /// <summary>
        /// Override to add instances of 3rd party classes as AutoFac services. 
        /// This is useful when you want an instance of a 3rd party class to be injected.
        /// Please consider registering these services with 
        /// <see cref="Support.AppBlocksInstanceLifetime.SingleInstance"/>
        /// </summary>
        /// <param name="builder"></param>
        protected abstract void RegisterExternalServices(ContainerBuilder builder);

        /// <summary>
        /// Override to register your application assembly. Typically you will call 
        /// <see cref="RegisterAssembly(System.Reflection.Assembly, ContainerBuilder, AppBlocksContainerBuilder)"/> here to 
        /// register attributed services from your <see cref="System.Reflection.Assembly"/>
        /// </summary>
        /// <param name="builder"><see cref="global::Autofac.ContainerBuilder"/> instance</param>
        protected abstract void RegisterAssemblyServices(ContainerBuilder builder);

        /// <summary>
        /// Scan assembly for attributed Autofac services. 
        /// </summary>
        /// <param name="assembly"><see cref="System.Reflection.Assembly"/> to scam</param>
        /// <param name="builder"><see cref="global::Autofac.ContainerBuilder"/> to add services to</param>
        /// <param name="appBlocksContainerBuilder"><see cref="AppBlocksContainerBuilder"/> for the AppBlocks application</param>
        protected static void RegisterAssembly(
            System.Reflection.Assembly assembly,
            ContainerBuilder builder,
            AppBlocksContainerBuilder appBlocksContainerBuilder)
        {
            // Register attributed services in assembly
            RegistrationUtils.RegisterAssembly
                (assembly, builder, appBlocksContainerBuilder);
        }

        /// <summary>
        /// Registers an instance of a class as a 
        /// <see cref="Support.AppBlocksInstanceLifetime.SingleInstance"/> service
        /// </summary>
        /// <typeparam name="T">Type of class to register </typeparam>
        /// <param name="builder"><see cref="global::Autofac.ContainerBuilder"/> to add service to</param>
        /// <param name="service">Instance of class to register</param>
        protected static void RegisterAsSingleInstance<T>(ContainerBuilder builder, T service)
            where T : class
        {
            // throw exception if reference is null
            if (service == null)
                throw new ArgumentNullException("Cannot register service as null");

            // Register service in builder
            builder
                .Register(c => service)
                .AsSelf()
                .SingleInstance();
        }
    }
}
