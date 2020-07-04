using AppBlocks.Autofac.Interceptors;
using Autofac;

namespace AppBlocks.Autofac.Common
{
    public class AppBlocksModule : Module
    {
        // This is a private constant from the Autofac.Extras.DynamicProxy2 assembly
        // that is needed to "poke" interceptors into registrations.
        //const string InterceptorsPropertyName = "Autofac.Extras.DynamicProxy2.RegistrationExtensions.InterceptorsPropertyName";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => new LoggingConfiguration(c.Resolve<ApplicationConfiguration>()))
                .As<ILoggingConfiguration>()
                .SingleInstance();

            RegisterInterceptors(builder);
        }

        private void RegisterInterceptors(ContainerBuilder builder)
        {
            // Typed registration
            builder.RegisterType<LoggingInterceptor>().AsSelf().SingleInstance();
            builder.RegisterType<ValidationInterceptor>().AsSelf().SingleInstance();
            builder.RegisterType<WorkflowInterceptor>().AsSelf().SingleInstance();
        }
    }
}
