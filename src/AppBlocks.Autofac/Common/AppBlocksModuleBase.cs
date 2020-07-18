using AppBlocks.Autofac.Interceptors;
using Autofac;
using Autofac.Core;
using System;

namespace AppBlocks.Autofac.Common
{
    public abstract class AppBlocksModuleBase : Module
    {
        protected IContext ApplicationContext => AppBlocksContainerBuilder.ApplicationContext;

        protected readonly AppBlocksContainerBuilder AppBlocksContainerBuilder;

        public AppBlocksModuleBase(AppBlocksContainerBuilder appBlocksContainerBuilder)
        {
            
            AppBlocksContainerBuilder = appBlocksContainerBuilder;
        }
        // This is a private constant from the Autofac.Extras.DynamicProxy2 assembly
        // that is needed to "poke" interceptors into registrations.
        //const string InterceptorsPropertyName = "Autofac.Extras.DynamicProxy2.RegistrationExtensions.InterceptorsPropertyName";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            //builder.Register(c => new LoggingConfiguration(c.Resolve<ApplicationConfiguration>()))
            //    .As<ILoggingConfiguration>()
            //    .SingleInstance();

            //RegisterInterceptors(builder);
            RegisterExternalServices(builder);
            RegisterAssemblyServices(builder);
        }

        //private void RegisterInterceptors(ContainerBuilder builder)
        //{
        //    // Typed registration
        //    builder.RegisterType<LoggingInterceptor>().AsSelf().SingleInstance();
        //    builder.RegisterType<ValidationInterceptor>().AsSelf().SingleInstance();
        //    builder.RegisterType<WorkflowInterceptor>().AsSelf().SingleInstance();
        //}

        protected abstract void RegisterExternalServices(ContainerBuilder builder);

        protected abstract void RegisterAssemblyServices(ContainerBuilder builder);

        protected void RegisterAssembly(
            System.Reflection.Assembly assembly,
            ContainerBuilder builder,
            AppBlocksContainerBuilder appBlocksContainerBuilder)
        {
            RegistrationUtils.RegisterAssembly(assembly, builder, appBlocksContainerBuilder);
        }

        protected void RegisterAsSingleInstance<T>(ContainerBuilder builder, T service)
            where T : class
        {
            if (service == null)
                throw new ArgumentNullException("Cannot register service as null");

            builder
                .Register(c => service)
                .AsSelf()
                .SingleInstance();
        }
    }
}
