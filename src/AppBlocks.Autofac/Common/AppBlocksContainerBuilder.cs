using AppBlocks.Autofac.Interceptors;
using AppBlocks.Autofac.Services;
using AppBlocks.Autofac.Support;
using Autofac;
using Autofac.Builder;
using Autofac.Extras.DynamicProxy;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AppBlocks.Autofac.Common
{
    public abstract class AppBlocksContainerBuilder
    {
        private static readonly ILog logger = 
            LogManager.GetLogger("default", MethodBase.GetCurrentMethod().DeclaringType);

        protected readonly ApplicationConfiguration ApplicationConfiguration;
        public AppBlocksContainerBuilder(ApplicationConfiguration applicationConfiguration)
        {
            ApplicationConfiguration = applicationConfiguration ?? throw new ArgumentNullException("Application configuration cannot be null");
        }

        public AppBlocksContainerBuilder() { }

        public IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            InitializeContainer(builder);
            return builder.Build();
        }

        public void InitializeContainer(ContainerBuilder builder)
        {
            builder.Register(c => ApplicationConfiguration).AsSelf().SingleInstance();
            RegisterGlobalServices(builder);
            builder.RegisterModule<LoggingModule>();

            builder.Register(c => new LoggingConfiguration(c.Resolve<ApplicationConfiguration>()))
                .As<ILoggingConfiguration>()
                .SingleInstance();

            RegisterInterceptors(builder);

            RegisterAssembly(Assembly.GetExecutingAssembly(), builder);
            RegisterAssemblyServices(builder);

            RegisterExternalDirectories(builder);
        }

        protected virtual void RegisterGlobalServices(ContainerBuilder builder) { }
        protected virtual void RegisterAssemblyServices(ContainerBuilder builder) { }

        protected void RegisterAssembly(Assembly assembly, ContainerBuilder builder)
        {   
            RegisterAsInterfaces(builder, assembly);
            RegisterNamedServices(builder, assembly);
            RegisterKeyedServices(builder, assembly);
        }

        private void RegisterExternalDirectories(ContainerBuilder builder)
        {
            //Iterate through list of directories set up as Autofac probe directories 
            foreach (string directory in (ApplicationConfiguration?.AutofacDirectories ?? new string[0]))
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
        protected void RegisterInMemoryCache(ContainerBuilder builder)
        {
            builder.Register(c => new InMemoryCacheService())
                .As<ICacheService>()
                .SingleInstance();
        }

        private void RegisterInterceptors(ContainerBuilder builder)
        {
            // Typed registration
            logger.Debug("Registering Logging Interceptor");
            builder.RegisterType<LoggingInterceptor>().AsSelf().SingleInstance();
            logger.Debug("Registering Validation Interceptor");
            builder.RegisterType<ValidationInterceptor>().AsSelf().SingleInstance();
            logger.Debug("Registering Workflow Interceptor");
            builder.RegisterType<WorkflowInterceptor>().AsSelf().SingleInstance();
        }

        static IEnumerable<Type> GetTypesRegisteredAsServices(Assembly assembly)
        {
            return assembly.GetTypes().
                Where(t => t.GetCustomAttributes(typeof(AppBlocksServiceAttribute), true).Length == 1);
        }

        /// <summary>
        /// Registers types attributed with AutofacServiceAttribute as interfaces. 
        /// </summary>
        /// <param name="builder">Services Container builder</param>
        /// <param name="assembly">Assembly to process</param>
        private void RegisterAsInterfaces(ContainerBuilder builder, Assembly assembly)
        {
            //Anonymous service is one that has AutofacServiceAttribute with no name specified
            bool isAnonymousService(Type t) => t.GetCustomAttributes(typeof(AppBlocksServiceAttribute), true).Length == 1
                    && string.IsNullOrEmpty(((AppBlocksServiceAttribute)t.GetCustomAttribute(typeof(AppBlocksServiceAttribute))).Name)
                    && !((AppBlocksServiceAttribute)t.GetCustomAttribute(typeof(AppBlocksServiceAttribute))).IsKeyed;

            assembly.GetTypes()
                .Where(t => isAnonymousService(t))
                .Select(t =>
                    new
                    {
                        TypeInformation = t,
                        AttributeInformation = (AppBlocksServiceAttribute)t
                            .GetCustomAttribute(typeof(AppBlocksServiceAttribute))
                    })
                .ToList()
                .ForEach(i =>
                {
                    logger.Debug($"Registering {i.TypeInformation.FullName} as implemented interfaces");

                    var registration = builder.RegisterType(i.TypeInformation).AsImplementedInterfaces();
                    SetTypeScope(i.AttributeInformation, registration);
                    AddTypeInterceptors(i.AttributeInformation, registration);
                });
        }

        private void RegisterNamedServices(ContainerBuilder builder, Assembly assembly)
        {
            //Named service is one that has AutofacServiceAttribute with no name specified
            bool isNamedService(Type t) => t.GetCustomAttributes(typeof(AppBlocksServiceAttribute), true).Length == 1
                    && !string.IsNullOrEmpty(((AppBlocksServiceAttribute)t.GetCustomAttribute(typeof(AppBlocksServiceAttribute))).Name)
                    && !((AppBlocksServiceAttribute)t.GetCustomAttribute(typeof(AppBlocksServiceAttribute))).IsKeyed;

            assembly.GetTypes()
                .Where(t => isNamedService(t))
                .Select(t =>
                    new
                    {
                        TypeInformation = t,
                        AttributeInformation = (AppBlocksServiceAttribute)t
                            .GetCustomAttribute(typeof(AppBlocksServiceAttribute))
                    })
                .ToList()
                .ForEach(i =>
                {
                    //Find out what to Interface to use for service registration
                    logger.Debug($"Registering {i.TypeInformation.FullName} as named service {i.AttributeInformation.Name}");
                    var registrationType = GetServiceRegistrationType(i.TypeInformation, i.AttributeInformation);
                    var registration = builder.RegisterType(i.TypeInformation).Named(i.AttributeInformation.Name, registrationType);
                    SetTypeScope(i.AttributeInformation, registration);
                    AddTypeInterceptors(i.AttributeInformation, registration);
                });
        }

        private void RegisterKeyedServices(ContainerBuilder builder, Assembly assembly)
        {
            //Named service is one that has AutofacServiceAttribute with no name specified
            bool isKeyedService(Type t) => t.GetCustomAttributes(typeof(AppBlocksServiceAttribute), true).Length == 1
                    && !string.IsNullOrEmpty(((AppBlocksServiceAttribute)t.GetCustomAttribute(typeof(AppBlocksServiceAttribute))).Name)
                    && ((AppBlocksServiceAttribute)t.GetCustomAttribute(typeof(AppBlocksServiceAttribute))).IsKeyed;

            assembly.GetTypes()
                .Where(t => isKeyedService(t))
                .Select(t =>
                    new
                    {
                        TypeInformation = t,
                        AttributeInformation = (AppBlocksServiceAttribute)t
                            .GetCustomAttribute(typeof(AppBlocksServiceAttribute))
                    })
                .ToList()
                .ForEach(i =>
                {
                    //Find out what to Interface to use for service registration. For keyed service, we enforce
                    //having attribute declared with non-null ServiceType. In other words, keyed services 
                    //must explicitly specify service type.  
                    logger.Debug($"Registering {i.TypeInformation.FullName} as keyed service of type {i.AttributeInformation.ServiceType.FullName} " +
                        $"with key {i.AttributeInformation.Name}");

                    var registrationType = GetServiceRegistrationType(i.TypeInformation, i.AttributeInformation);
                    var registration = builder.RegisterType(i.TypeInformation).Keyed(i.AttributeInformation.Name, registrationType);
                    SetTypeScope(i.AttributeInformation, registration);
                    AddTypeInterceptors(i.AttributeInformation, registration);
                });
        }

        private void RegisterFromAssembly(ContainerBuilder builder, string assemblyPath)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            if (assembly.GetCustomAttributes<AppBlocksAssemblyAttribute>().Any())
            {
                RegisterAssembly(assembly, builder);
            }
        }

        private void SetTypeScope(
            AppBlocksServiceAttribute attribute,
            IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> registration)
        {
            //Service is available as all implemented interfaces. 
            switch (attribute.ServiceScope)
            {
                case EnumAppBlocksInstanceLifetime.SingleInstance:
                    registration.SingleInstance();
                    break;

                case EnumAppBlocksInstanceLifetime.InstancePerDependency:
                    registration.InstancePerDependency();
                    break;

                case EnumAppBlocksInstanceLifetime.InstancePerLifetimeScope:
                    registration.InstancePerLifetimeScope();
                    break;
            }
        }

        private void AddTypeInterceptors(
            AppBlocksServiceAttribute attribute,
            IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> registration)
        {
            bool isEnabledForInterception = false;
            if ((attribute.Interceptors?.Count() ?? 0) == 0) return;
            var interceptorsSet = attribute.Interceptors.ToHashSet<string>();
            if (interceptorsSet.Contains(AppBlocksInterceptorConstants.Logging))
            {
                registration = registration
                    .EnableInterfaceInterceptors().InterceptedBy(typeof(LoggingInterceptor));
                isEnabledForInterception = true;
            }

            if (interceptorsSet.Contains(AppBlocksInterceptorConstants.Validation))
            {
                if (isEnabledForInterception) registration = registration.InterceptedBy(typeof(ValidationInterceptor));
                else registration.EnableInterfaceInterceptors().InterceptedBy(typeof(ValidationInterceptor));
                isEnabledForInterception = true;
            }

            //A class should only be incercepted by workflow interceptor if 
            //Worflows fields has at least one non whitespace workflow name
            if ((attribute.Workflows ?? new string[0]).Any(s => !string.IsNullOrWhiteSpace(s)))
            {
                if (isEnabledForInterception) registration = registration.InterceptedBy(typeof(WorkflowInterceptor));
                else registration.EnableInterfaceInterceptors().InterceptedBy(typeof(WorkflowInterceptor));
                isEnabledForInterception = true;
            }
        }

        private static Type GetServiceRegistrationType(Type type, AppBlocksServiceAttribute attribute)
        {
            //If service type value is set make sure it is assignable. 
            if (attribute.ServiceType != null)
            {
                if (!attribute.ServiceType.IsAssignableFrom(type))
                {
                    throw new Exception($"Type {type.FullName} cannot be registered as service " +
                        $"{attribute.ServiceType.FullName}. {type.FullName} does not implement this interface");
                }

                return attribute.ServiceType;
            }
            else
            {
                return type.GetInterfaces().Count() > 0 ? type.GetInterfaces()[0] : type;
            }
        }
    }
}
