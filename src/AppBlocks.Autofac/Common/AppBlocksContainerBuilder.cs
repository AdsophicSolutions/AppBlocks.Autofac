using AppBlocks.Autofac.Interceptors;
using AppBlocks.Autofac.Services;
using AppBlocks.Autofac.Support;
using Autofac;
using Autofac.Builder;
using Autofac.Extras.DynamicProxy;
using log4net;
using MediatR;
using MediatR.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AppBlocks.Autofac.Common
{
    public abstract class AppBlocksContainerBuilder
    {
        private static readonly ILog logger =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        protected readonly ApplicationConfiguration ApplicationConfiguration;
        protected internal readonly AppBlocksApplicationMode ApplicationMode;
        private IContext applicationContext;

        public AppBlocksContainerBuilder(
            ApplicationConfiguration applicationConfiguration,
            AppBlocksApplicationMode applicationMode)
        {
            ApplicationConfiguration = applicationConfiguration ?? throw new ArgumentNullException("Application configuration cannot be null");
            ApplicationMode = applicationMode;
        }

        public AppBlocksContainerBuilder(AppBlocksApplicationMode applicationMode)
        {
            ApplicationConfiguration = new ApplicationConfiguration();
            ApplicationMode = applicationMode;
        }

        public IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            InitializeContainer(builder);
            return builder.Build();
        }

        public void InitializeContainer(ContainerBuilder builder)
        {
            builder.Register(c => ApplicationConfiguration).AsSelf().SingleInstance();
            applicationContext = new ApplicationContextService(ApplicationConfiguration);
            builder.Register(c => applicationContext)
                .As<IContext>()
                .SingleInstance();

            builder.RegisterModule<LoggingModule>();

            RegisterMediatr(builder);
            RegisterExternalServices(builder, applicationContext);

            builder.Register(c => new LoggingConfiguration(c.Resolve<ApplicationConfiguration>()))
                .As<ILoggingConfiguration>()
                .SingleInstance();

            RegisterInterceptors(builder);
            RegisterInMemoryCache(builder);

            RegisterAssembly(Assembly.GetExecutingAssembly(), builder);
            RegisterAssemblyServices(builder);
            RegisterExternalDirectories(builder);
        }

        private void RegisterMediatr(ContainerBuilder builder)
        {
            builder
                .RegisterAssemblyTypes(typeof(IMediator).Assembly)
                .AsImplementedInterfaces();

            builder.RegisterGeneric(typeof(RequestPostProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(RequestPreProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));

            builder.RegisterGeneric(typeof(LogMediatrRequest<>)).As(typeof(IRequestPreProcessor<>));
            builder.RegisterGeneric(typeof(LogMediatrResponse<,>)).As(typeof(IRequestPostProcessor<,>));
            builder.RegisterGeneric(typeof(LogMediatrNotification<>)).As(typeof(INotificationHandler<>));


            //var mediatrOpenTypes = new[]
            //{
            //    typeof(IRequestHandler<,>),
            //    typeof(IRequestExceptionHandler<,,>),
            //    typeof(IRequestExceptionAction<,>),
            //    typeof(INotificationHandler<>),
            //};

            //foreach (var mediatrOpenType in mediatrOpenTypes)
            //{
            //    builder
            //        .RegisterAssemblyTypes(typeof(Ping).GetTypeInfo().Assembly)
            //        .AsClosedTypesOf(mediatrOpenType)
            //        // when having a single class implementing several handler types
            //        // this call will cause a handler to be called twice
            //        // in general you should try to avoid having a class implementing for instance `IRequestHandler<,>` and `INotificationHandler<>`
            //        // the other option would be to remove this call
            //        // see also https://github.com/jbogard/MediatR/issues/462
            //        .AsImplementedInterfaces();
            //}

            //// It appears Autofac returns the last registered types first
            //builder.RegisterGeneric(typeof(RequestPostProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            //builder.RegisterGeneric(typeof(RequestPreProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            //builder.RegisterGeneric(typeof(RequestExceptionActionProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            //builder.RegisterGeneric(typeof(RequestExceptionProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            //builder.RegisterGeneric(typeof(GenericRequestPreProcessor<>)).As(typeof(IRequestPreProcessor<>));
            //builder.RegisterGeneric(typeof(GenericRequestPostProcessor<,>)).As(typeof(IRequestPostProcessor<,>));
            //builder.RegisterGeneric(typeof(GenericPipelineBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            //builder.RegisterGeneric(typeof(ConstrainedRequestPostProcessor<,>)).As(typeof(IRequestPostProcessor<,>));
            //builder.RegisterGeneric(typeof(ConstrainedPingedHandler<>)).As(typeof(INotificationHandler<>));

            // request & notification handlers
            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });
        }

        protected virtual void RegisterExternalServices(ContainerBuilder builder,
            IContext applicationContext) { }

        protected abstract void RegisterAssemblyServices(ContainerBuilder builder);
        protected internal virtual bool ShouldRegisterService(Type type, AppBlocksServiceAttributeBase serviceAttribute) => true;

        protected void RegisterAssembly(Assembly assembly, ContainerBuilder builder)
        {
            RegistrationUtils.RegisterAssembly(assembly, builder, this);
        }

        protected void RegisterModule(ContainerBuilder builder, AppBlocksModule appBlocksModule)
        {
            appBlocksModule.RegisterExternalServices(builder, applicationContext);
            appBlocksModule.RegisterAssemblyServices(builder, this);
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

        private void RegisterExternalDirectories(ContainerBuilder builder)
        {
            //Iterate through list of directories set up as Autofac probe directories 
            foreach (string directory in (ApplicationConfiguration?.AutofacDirectories.Value ?? new string[0]))
            {
                if (!Directory.Exists(directory)) throw new InvalidDataException($"Configured directory path {directory} is invalid");
                foreach (string filePath in Directory.GetFiles(directory))
                    if (Utility.IsAssembly(filePath)) RegisterFromAssembly(builder, filePath);
            }
        }

        /// <summary>
        /// Registers InMemoryCache service 
        /// </summary>
        /// <param name="builder"></param>
        private void RegisterInMemoryCache(ContainerBuilder builder)
        {
            builder.Register(c => new InMemoryCacheService())
                .As<ICacheService>()
                .SingleInstance();
        }

        private void RegisterInterceptors(ContainerBuilder builder)
        {
            // Typed registration
            logger.Debug("Registering Interceptor");
            builder.RegisterType<AppBlocksServiceInterceptor>().AsSelf().SingleInstance();
            //logger.Debug("Registering Validation Interceptor");
            //builder.RegisterType<ValidationInterceptor>().AsSelf().SingleInstance();
            //logger.Debug("Registering Workflow Interceptor");
            //builder.RegisterType<WorkflowInterceptor>().AsSelf().SingleInstance();
        }

        
       private void RegisterFromAssembly(ContainerBuilder builder, string assemblyPath)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            if (assembly.GetCustomAttributes<AppBlocksAssemblyAttribute>().Any())
            {
                RegisterAssembly(assembly, builder);
            }
        }
    }
}
