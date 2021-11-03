using AppBlocks.Autofac.Interceptors;
using AppBlocks.Autofac.Services;
using AppBlocks.Autofac.Support;
using Autofac;
using log4net;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AppBlocks.Autofac.Common
{
    /// <summary>
    /// All AppBlocks applications must define a class inherited from 
    /// <see cref="AppBlocksContainerBuilder"/>. Manages registration
    /// of Autofac services. 
    /// </summary>
    public abstract class AppBlocksContainerBuilder
    {
        private readonly static ILogger<AppBlocksContainerBuilder> logger =
            new Logger<AppBlocksContainerBuilder>(AppBlocksLogging.Instance.GetLoggerFactory());

        /// <summary>
        /// <see cref="ApplicationConfiguration"/> for the AppBlocks application
        /// </summary>
        protected ApplicationConfiguration ApplicationConfiguration { get; set; }

        /// <summary>
        /// <see cref="AppBlocksApplicationMode"/> for AppBlocks application
        /// </summary>
        protected internal AppBlocksApplicationMode ApplicationMode { get; set; }

        /// <summary>
        /// <see cref="IContext"/> provides a dictionary of key / value pairs 
        /// that defined the application context. Default <see cref="IContext"/>
        /// is generated via <see cref="ApplicationConfiguration"/> source file
        /// </summary>
        protected internal IContext ApplicationContext { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="applicationConfiguration">Instance of <see cref="ApplicationConfiguration"/> for the AppBlocks application</param>
        /// <param name="applicationMode"><see cref="AppBlocksApplicationMode"/> for the AppBlocks application. <see cref="AppBlocksApplicationMode"/> can be Live or Test</param>
        public AppBlocksContainerBuilder(
            ApplicationConfiguration applicationConfiguration,
            AppBlocksApplicationMode applicationMode)
        {
            ApplicationConfiguration = applicationConfiguration ?? 
                throw new ArgumentNullException(
                    paramName: nameof(applicationConfiguration),
                    message: "Application configuration cannot be null");
            ApplicationMode = applicationMode;
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="applicationMode"><see cref="AppBlocksApplicationMode"/> for the AppBlocks application. <see cref="AppBlocksApplicationMode"/> can be Live or Test</param>
        public AppBlocksContainerBuilder(AppBlocksApplicationMode applicationMode)
            : this(new ApplicationConfiguration(), applicationMode)
        {
        }

        /// <summary>
        /// Initializes <see cref="global::Autofac.IContainer"/> for an 
        /// AppBlocks application
        /// </summary>
        /// <returns>Reference to <see cref="global::Autofac.IContainer"/></returns>
        public IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            InitializeContainer(builder, true);
            return builder.Build();
        }

        /// <summary>
        /// Initializes <see cref="global::Autofac.IContainer"/> for an 
        /// AppBlocks application. Used for Web project integration where
        /// <see cref="global::Autofac.IContainer"/> implementation is passed in as a parameter 
        /// </summary>
        /// <param name="builder">Builder to initialize</param>
        public void BuildContainer(ContainerBuilder builder) =>
            BuildContainer(builder, false);

        /// <summary>
        /// Initializes <see cref="global::Autofac.IContainer"/> for an 
        /// AppBlocks application. Used for Web project integration where
        /// <see cref="global::Autofac.IContainer"/> implementation is passed in as a parameter 
        /// </summary>
        /// <param name="builder">Builder to initialize</param>
        /// <param name="registerAppBlocksLogging"><c>true</c> if AppBlocks should be used; otherwise <c>false</c>. 
        /// Required to support Asp.Net Core since we want to use logger factory registered in Asp.NET core services </param>
        public void BuildContainer(ContainerBuilder builder, bool registerAppBlocksLogging)
            => InitializeContainer(builder, registerAppBlocksLogging);

        private void InitializeContainer(ContainerBuilder builder, bool registerAppBlocksLogging)
        {
            // Registation ApplicationConfiguration as single instance
            builder.Register(c => ApplicationConfiguration).AsSelf().SingleInstance();

            // Initialize application context based on ApplicationConfiguration
            ApplicationContext = new ApplicationContextService(ApplicationConfiguration);

            // Register ApplicationContext as SingleInstance
            builder.Register(c => ApplicationContext)
                .As<IContext>()
                .SingleInstance();

            if (registerAppBlocksLogging)
            {
                // Register log factory to create loggers
                builder.RegisterInstance(AppBlocksLogging.Instance.GetLoggerFactory()).As<ILoggerFactory>().SingleInstance();
                builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();
            }

            // MediatR registration
            RegisterMediatr(builder);

            // Let inheriting classes register external services
            RegisterExternalServices(builder);

            // Set up LoggingConfiguration
            builder.Register(c => new LoggingConfiguration(c.Resolve<ApplicationConfiguration>()))
                .As<ILoggingConfiguration>()
                .SingleInstance();

            // Register Interceptors 
            RegisterInterceptors(builder);

            // Register global cache service
            RegisterInMemoryCache(builder);

            // Register current assembly
            RegisterAssembly(typeof(AppBlocksContainerBuilder).Assembly, builder);

            // Let inheriting class register services
            RegisterAssemblyServices(builder);

            // Finally register any external directories setup via
            // application configuration
            RegisterExternalDirectories(builder);
        }

        private static void RegisterMediatr(ContainerBuilder builder)
        {
            // Register all types from IMediatR assembly
            builder
                .RegisterAssemblyTypes(typeof(IMediator).Assembly)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            // Required for intercepting MediatR Request Response
            builder.RegisterGeneric(typeof(RequestPostProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(RequestPreProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));

            // Required for interception MediatR notifications
            builder.RegisterGeneric(typeof(LogMediatrRequest<>)).As(typeof(IRequestPreProcessor<>));
            builder.RegisterGeneric(typeof(LogMediatrResponse<,>)).As(typeof(IRequestPostProcessor<,>));
            builder.RegisterGeneric(typeof(LogMediatrNotification<>)).As(typeof(INotificationHandler<>));

            // request & notification handlers
            builder.Register<ServiceFactory>(context =>
            {   
                var c = context.Resolve<IComponentContext>();                
                return t => c.Resolve(t);
            });
        }

        /// <summary>
        /// Override to add instances of 3rd party classes as AutoFac services. 
        /// This is useful when you want an instance of a 3rd party class to be injected.
        /// Please consider registering these services with 
        /// <see cref="Support.AppBlocksInstanceLifetime.SingleInstance"/>
        /// </summary>
        /// <param name="builder"></param>
        protected virtual void RegisterExternalServices(ContainerBuilder builder) { }

        /// <summary>
        /// Override to register your application assembly. Typically you will call 
        /// <see cref="RegisterAssembly(Assembly, ContainerBuilder)"/> here to 
        /// register attributed services from your <see cref="Assembly"/>
        /// </summary>
        /// <param name="builder"><see cref="global::Autofac.ContainerBuilder"/> instance</param>
        protected abstract void RegisterAssemblyServices(ContainerBuilder builder);

        /// <summary>
        /// Override to prevent certain services from being registered. This may be useful while running 
        /// tests when you want to mock certain services and prevent the live service from being registered
        /// </summary>
        /// <param name="type"><see cref="Type"/> for service being registered</param>
        /// <param name="serviceAttribute"><see cref="AppBlocksServiceAttribute"/> on the service</param>
        /// <returns><c>true</c> if you want service to be registered; otherwise <c>false</c>.</returns>
        protected internal virtual bool ShouldRegisterService(Type type, AppBlocksServiceBaseAttribute serviceAttribute) => true;

        /// <summary>
        /// Scan assembly for attributed Autofac services. 
        /// </summary>
        /// <param name="assembly"><see cref="Assembly"/> to scam</param>
        /// <param name="builder"><see cref="global::Autofac.ContainerBuilder"/> to add services to</param>
        protected void RegisterAssembly(
            Assembly assembly,
            ContainerBuilder builder)
        {
            // Register attributed services in assembly
            RegistrationUtils.RegisterAssembly(assembly, builder, this);
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
                throw new ArgumentNullException(
                    paramName: nameof(service),
                    message: "Cannot register service as null");

            // Register service in builder
            builder
                .Register(c => service)
                .AsSelf()
                .SingleInstance();
        }

        private void RegisterExternalDirectories(ContainerBuilder builder)
        {
            //Iterate through list of directories set up as Autofac probe directories 
            foreach (string directory in (ApplicationConfiguration?.AutofacDirectories.Value ?? Array.Empty<string>()))
            {
                if (!Directory.Exists(directory)) throw new InvalidDataException($"Configured directory path {directory} is invalid");

                // Register assemblies in directory
                foreach (string filePath in Directory.GetFiles(directory))
                    if (Utility.IsAssembly(filePath)) RegisterFromAssembly(builder, filePath);
            }
        }

        /// <summary>
        /// Registers InMemoryCache service 
        /// </summary>
        /// <param name="builder"></param>
        private static void RegisterInMemoryCache(ContainerBuilder builder)
        {
            // Global in memory cache registration
            builder.Register(c => new InMemoryCacheService())
                .As<ICacheService>()
                .SingleInstance();
        }

        private static void RegisterInterceptors(ContainerBuilder builder)
        {
            // Typed registration
            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("Registering Interceptor");

            builder.RegisterType<AppBlocksServiceInterceptor>().AsSelf().SingleInstance();
            //logger.Debug("Registering Validation Interceptor");
            //builder.RegisterType<ValidationInterceptor>().AsSelf().SingleInstance();
            //logger.Debug("Registering Workflow Interceptor");
            //builder.RegisterType<WorkflowInterceptor>().AsSelf().SingleInstance();
        }

        // Registers an assembly from assembly path
        private void RegisterFromAssembly(ContainerBuilder builder, string assemblyPath)
        {
            // Load assembly
            Assembly assembly = Assembly.LoadFrom(assemblyPath);

            // Assembly must be marked with AppBlocksAssembly atttribute
            if (assembly.GetCustomAttributes<AppBlocksAssemblyAttribute>().Any())
            {
                // call register assembly to register all services in
                // the assembly
                RegisterAssembly(assembly, builder);
            }
        }
    }
}
