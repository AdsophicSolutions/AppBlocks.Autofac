using AppBlocks.Autofac.Interceptors;
using AppBlocks.Autofac.Support;
using Autofac;
using Autofac.Builder;
using Autofac.Extras.DynamicProxy;
using log4net;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AppBlocks.Autofac.Common
{
    /// <summary>
    /// Utility method containing shared AppBlocks registration logic
    /// </summary>
    internal static class RegistrationUtils
    {
        private static readonly ILog logger =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        internal static void RegisterAssembly(Assembly assembly,
            ContainerBuilder builder,
            AppBlocksContainerBuilder appBlocksContainerBuilder)
        {
            // Scan assembly for services to be registered as interfaces
            RegisterAsInterfaces(builder, assembly, appBlocksContainerBuilder);

            // Scan assembly for named services
            RegisterNamedServices(builder, assembly, appBlocksContainerBuilder);

            // Scan assembly for keyed services
            RegisterKeyedServices(builder, assembly, appBlocksContainerBuilder);

            // Scan assembly for MediatR request services
            RegisterMediatrRequestServices(builder, assembly, appBlocksContainerBuilder);

            // Scan assembly for MediatR notification services
            RegisterMediatrNotificationServices(builder, assembly, appBlocksContainerBuilder);
        }

        /// <summary>
        /// Registers types attributed with AutofacServiceAttribute as interfaces. 
        /// </summary>
        /// <param name="builder">Services Container builder</param>
        /// <param name="assembly">Assembly to process</param>
        /// <param name="appBlocksContainerBuilder"><see cref="AppBlocksContainerBuilder"/> instance for this application</param>
        private static void RegisterAsInterfaces(
            ContainerBuilder builder, 
            Assembly assembly,
            AppBlocksContainerBuilder appBlocksContainerBuilder)
        {
            // local method to filter in services to be registered with implemented 
            // interfaces
            bool isAnonymousService(Type t)
            {
                // Look for AppBlocks service attribute on a type
                var serviceAttributes = t.GetCustomAttributes(typeof(AppBlocksServiceAttribute), true);

                // Attribute was not found, return. 
                if (serviceAttributes.Length == 0) return false;

                // Get reference to attribute
                var serviceAttribute = (AppBlocksServiceAttribute)serviceAttributes[0];

                // Return if service is to be registered as a named or keyed service
                if (!string.IsNullOrEmpty(serviceAttribute.Name) || serviceAttribute.IsKeyed) return false;

                // Exclude live service if current application mode is test
                if (appBlocksContainerBuilder.ApplicationMode == AppBlocksApplicationMode.Test
                    && serviceAttribute.ServiceDependencyType == AppBlocksServiceDependencyType.Live) return false;

                // Ask if AppBlocksContainerBuilder concrete implementation wants to exclude the service from registration
                return appBlocksContainerBuilder.ShouldRegisterService(t, serviceAttribute);
            }

            // Search all assembly types for services to register
            assembly.GetTypes()
                .Where(t => isAnonymousService(t))
                .Select(t =>
                    // Create anonymous type
                    new
                    {
                        TypeInformation = t,
                        AttributeInformation = (AppBlocksServiceAttribute)t
                            .GetCustomAttribute(typeof(AppBlocksServiceAttribute))
                    })
                .ToList()
                .ForEach(i =>
                {
                    // Perform attribute validations
                    ValidateRegistration(i.AttributeInformation);

                    if (logger.IsDebugEnabled)
                        logger.Debug($"Registering {i.TypeInformation.FullName} as implemented interfaces");

                    // Register type with Autofac container builder. 
                    var registration = builder.RegisterType(i.TypeInformation).AsImplementedInterfaces();

                    // Set type lifetime scope
                    SetTypeLifetimeScope(i.AttributeInformation, registration);

                    // Add type to interceptors based on attribute information
                    AddTypeInterceptors(i.AttributeInformation, registration);
                });
        }


        /// <summary>
        ///  Scan assembly for AppBlocks named services
        /// </summary>        
        private static void RegisterNamedServices(
            ContainerBuilder builder, 
            Assembly assembly,
            AppBlocksContainerBuilder appBlocksContainerBuilder)
        {
            // local method to filter in named services 
            bool isNamedService(Type t)
            {
                // Look for AppBlocks service attribute on a type
                var serviceAttributes = t.GetCustomAttributes(typeof(AppBlocksServiceAttribute), true);

                // Attribute was not found, return. 
                if (serviceAttributes.Length == 0) return false;

                // Get reference to attribute
                var serviceAttribute = (AppBlocksServiceAttribute)serviceAttributes[0];

                // return if service is name service registration is not requested
                if (string.IsNullOrEmpty(serviceAttribute.Name) || serviceAttribute.IsKeyed) return false;

                // Exclude live service if current application mode is test
                if (appBlocksContainerBuilder.ApplicationMode == AppBlocksApplicationMode.Test
                    && serviceAttribute.ServiceDependencyType == AppBlocksServiceDependencyType.Live) return false;

                // Ask if AppBlocksContainerBuilder concrete implementation wants to exclude the service from registration
                return appBlocksContainerBuilder.ShouldRegisterService(t, serviceAttribute);
            }

            // Search all assembly types for services to register
            assembly.GetTypes()
                .Where(t => isNamedService(t))
                .Select(t =>
                    // Create anonymous type
                    new
                    {
                        TypeInformation = t,
                        AttributeInformation = (AppBlocksServiceAttribute)t
                            .GetCustomAttribute(typeof(AppBlocksServiceAttribute))
                    })
                .ToList()
                .ForEach(i =>
                {
                    // Perform attribute validations
                    ValidateRegistration(i.AttributeInformation);

                    //Find out what to Interface to use for service registration
                    if (logger.IsDebugEnabled)
                        logger.Debug($"Registering {i.TypeInformation.FullName} as named service {i.AttributeInformation.Name}");

                    // Register type with Autofac container builder. 
                    var registrationType = GetServiceRegistrationType(i.TypeInformation, i.AttributeInformation);
                    var registration = builder.RegisterType(i.TypeInformation).Named(i.AttributeInformation.Name, registrationType);

                    // Set type lifetime scope
                    SetTypeLifetimeScope(i.AttributeInformation, registration);

                    // Add type to interceptors based on attributed information
                    AddTypeInterceptors(i.AttributeInformation, registration);
                });
        }

        /// <summary>
        /// Register types to be registered as Keyed services
        /// </summary>        
        private static void RegisterKeyedServices(
            ContainerBuilder builder, 
            Assembly assembly,            
            AppBlocksContainerBuilder appBlocksContainerBuilder)
        {
            // local method to filter in services to be registered as keyed services
            bool isKeyedService(Type t)
            {
                // Look for AppBlocks service attribute on a type
                var serviceAttributes = t.GetCustomAttributes(typeof(AppBlocksServiceAttribute), true);

                // Attribute was not found, return
                if (serviceAttributes.Length == 0) return false;

                // Get reference to attribute
                var serviceAttribute = (AppBlocksServiceAttribute)serviceAttributes[0];

                // return if service keyed service registration is not requested
                if (string.IsNullOrEmpty(serviceAttribute.Name) || !serviceAttribute.IsKeyed) return false;

                // Exclude live service if current application mode is test
                if (appBlocksContainerBuilder.ApplicationMode == AppBlocksApplicationMode.Test
                    && serviceAttribute.ServiceDependencyType == AppBlocksServiceDependencyType.Live) return false;

                // Ask if AppBlocksContainerBuilder concrete implementation wants to exclude the service from registration
                return appBlocksContainerBuilder.ShouldRegisterService(t, serviceAttribute);
            }

            // Search all assembly types for services to register
            assembly.GetTypes()
                .Where(t => isKeyedService(t))
                .Select(t =>
                    // Create anonymous type
                    new
                    {
                        TypeInformation = t,
                        AttributeInformation = (AppBlocksServiceAttribute)t
                            .GetCustomAttribute(typeof(AppBlocksServiceAttribute))
                    })
                .ToList()
                .ForEach(i =>
                {
                    // Perform attribute validations
                    ValidateRegistration(i.AttributeInformation);

                    //Find out what to Interface to use for service registration. For keyed service, we enforce
                    //having attribute declared with non-null ServiceType. In other words, keyed services 
                    //must explicitly specify service type.  
                    if (logger.IsDebugEnabled)
                        logger.Debug($"Registering {i.TypeInformation.FullName} as keyed service of type {i.AttributeInformation.ServiceType.FullName} " +
                            $"with key {i.AttributeInformation.Name}");

                    // Register type with Autofac container builder
                    var registrationType = GetServiceRegistrationType(i.TypeInformation, i.AttributeInformation);
                    var registration = builder.RegisterType(i.TypeInformation).Keyed(i.AttributeInformation.Name, registrationType);

                    // Set type lifetime scope
                    SetTypeLifetimeScope(i.AttributeInformation, registration);

                    // Add type to interceptors based on attribute information
                    AddTypeInterceptors(i.AttributeInformation, registration);
                });
        }

        /// <summary>
        /// Register types to be registered as MediatR request service 
        /// </summary>        
        private static void RegisterMediatrRequestServices(
            ContainerBuilder builder, 
            Assembly assembly,
            AppBlocksContainerBuilder appBlocksContainerBuilder)
        {
            // local method to filter in services to be registered as MediatR request handlers
            bool isMediatrRequestService(Type t)
            {
                // Look for AppBlocks MediatR request service attribute
                var serviceAttributes = t.GetCustomAttributes(typeof(AppBlocksMediatrRequestServiceAttribute), true);

                // Attribute was not found, return
                if (serviceAttributes.Length == 0) return false;

                // Get reference to attribute
                var serviceAttribute = (AppBlocksMediatrRequestServiceAttribute)serviceAttributes[0];

                // Exclude live service if current application mode is test
                if (appBlocksContainerBuilder.ApplicationMode == AppBlocksApplicationMode.Test
                    && serviceAttribute.ServiceDependencyType == AppBlocksServiceDependencyType.Live) return false;

                // Ask if AppBlocksContainerBuilder concrete implementation wants to exclude the service from registration
                return appBlocksContainerBuilder.ShouldRegisterService(t, serviceAttribute);
            }

            // Search all assembly types for services to register
            assembly.GetTypes()
               .Where(t => isMediatrRequestService(t))
               .Select(t =>
                   new
                   {
                       // Create anonymous type
                       TypeInformation = t,
                       AttributeInformation = (AppBlocksMediatrRequestServiceAttribute)t
                           .GetCustomAttribute(typeof(AppBlocksMediatrRequestServiceAttribute))
                   })
               .ToList()
               .ForEach(i =>
               {
                   // Make sure type is a valid MediatR.IRequestHandler
                   if (!i.TypeInformation.IsClosedTypeOf(typeof(IRequestHandler<,>)))
                   {
                       throw new Exception($"{i.TypeInformation.FullName} must closed type of " +
                           $"Mediatr.IRequestHandler<,>");
                   }

                   if (logger.IsDebugEnabled)
                       logger.Debug($"Registering {i.TypeInformation.FullName} as Mediatr Request Handler");

                   // Register type with interfaces
                   var registration = builder
                    .RegisterType(i.TypeInformation)
                    .AsImplementedInterfaces();

                   // Set type lifetime scope
                   SetTypeLifetimeScope(i.AttributeInformation, registration);
               });
        }

        /// <summary>
        /// Register types to be registered as MediatR notification services
        /// </summary>        
        private static void RegisterMediatrNotificationServices(
            ContainerBuilder builder, 
            Assembly assembly,
            AppBlocksContainerBuilder appBlocksContainerBuilder)
        {
            // local method to filter in services to be registered as MediatR notification handlers
            bool isMediatrRequestService(Type t)
            {
                // Look for AppBlocks MediatR notification service attribute
                var serviceAttributes = t.GetCustomAttributes(typeof(AppBlocksMediatrNotificationServiceAttribute), true);

                // Attribute was not found, return
                if (serviceAttributes.Length == 0) return false;
                
                // Get reference to attribute
                var serviceAttribute = (AppBlocksMediatrNotificationServiceAttribute)serviceAttributes[0];

                // Exclude live service if current application mode is test
                if (appBlocksContainerBuilder.ApplicationMode == AppBlocksApplicationMode.Test
                    && serviceAttribute.ServiceDependencyType == AppBlocksServiceDependencyType.Live) return false;

                // Ask if AppBlocksContainerBuilder concrete implementation wants to exclude the service from registration
                return appBlocksContainerBuilder.ShouldRegisterService(t, serviceAttribute);
            }

            // Search all assembly types for services to register
            assembly.GetTypes()
               .Where(t => isMediatrRequestService(t))
               .Select(t =>
                    // Create anonymous type
                   new
                   {
                       TypeInformation = t,
                       AttributeInformation = (AppBlocksMediatrNotificationServiceAttribute)t
                           .GetCustomAttribute(typeof(AppBlocksMediatrNotificationServiceAttribute))
                   })
               .ToList()
               .ForEach(i =>
               {
                   // Make sure type is a valid MediatR.INotificationHandler
                   if (!i.TypeInformation.IsClosedTypeOf(typeof(INotificationHandler<>)))
                   {
                       throw new Exception($"{i.TypeInformation.FullName} must closed type of " +
                           $"Mediatr.INotificationHandler<>");
                   }

                   if (logger.IsDebugEnabled)
                       logger.Debug($"Registering {i.TypeInformation.FullName} as Mediatr Notification Handler");

                   // Register type with implemented interfaces
                   var registration = builder
                    .RegisterType(i.TypeInformation)
                    .AsImplementedInterfaces();

                   // Set type lifetime scope
                   SetTypeLifetimeScope(i.AttributeInformation, registration);
               });
        }

        /// <summary>
        /// Make sure attributes are valid types
        /// </summary>        
        private static void ValidateRegistration(AppBlocksServiceAttribute attributeInformation)
        {
            // No further checks necessary if ServiceType is not set
            if (attributeInformation.ServiceType == null) return;

            // Make sure that service loggers have the correct service attribute
            if (attributeInformation.ServiceType.GetType() == typeof(IServiceLogger)
                && !attributeInformation.GetType().IsAssignableFrom(typeof(AppBlocksLoggerServiceAttribute)))
            {
                var message = $"Please register {nameof(IServiceLogger)} using attribute {nameof(AppBlocksLoggerServiceAttribute)}. " +
                        $"Registering using {nameof(AppBlocksServiceAttribute)} is not permitted";

                var exception = new InvalidOperationException(message);

                if (logger.IsErrorEnabled)
                    logger.Error("Error registering service", exception);

                throw new InvalidOperationException(message);
            }

            // Make sure that service validators have the correct service attribute
            if (attributeInformation.ServiceType.GetType() == typeof(IServiceValidator)
                && !attributeInformation.GetType().IsAssignableFrom(typeof(AppBlocksValidatorServiceAttribute)))
            {
                var message = $"Please register {nameof(IServiceValidator)} using attribute {nameof(AppBlocksValidatorServiceAttribute)}. " +
                        $"Registering using {nameof(AppBlocksValidatorServiceAttribute)} is not permitted";

                var exception = new InvalidOperationException(message);

                if (logger.IsErrorEnabled)
                    logger.Error("Error registering service", exception);

                throw new InvalidOperationException(message);
            }

            // Make sure that workflow writers have the correct service attribute
            if (attributeInformation.ServiceType.GetType() == typeof(IWorkflowWriter)
                && !attributeInformation.GetType().IsAssignableFrom(typeof(AppBlocksWorkflowWriterServiceAttribute)))
            {
                var message = $"Please register {nameof(IWorkflowWriter)} using attribute {nameof(AppBlocksWorkflowWriterServiceAttribute)}. " +
                        $"Registering using {nameof(AppBlocksWorkflowWriterServiceAttribute)} is not permitted";

                var exception = new InvalidOperationException(message);

                if (logger.IsErrorEnabled)
                    logger.Error("Error registering service", exception);

                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Sets service lifetime scope
        /// </summary>        
        private static void SetTypeLifetimeScope(
            AppBlocksServiceAttributeBase attribute,
            IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> registration)
        {
            //Service is available as all implemented interfaces. 
            switch (attribute.ServiceScope)
            {
                // Register as single instance
                case AppBlocksInstanceLifetime.SingleInstance:
                    registration.SingleInstance();
                    break;

                // Register with instance for each depedency
                case AppBlocksInstanceLifetime.InstancePerDependency:
                    registration.InstancePerDependency();
                    break;

                // Register with instance per lifetime scope
                case AppBlocksInstanceLifetime.InstancePerLifetimeScope:
                    registration.InstancePerLifetimeScope();
                    break;
            }
        }

        /// <summary>
        /// Adds interceptors to AppBlocks services
        /// </summary>        
        private static void AddTypeInterceptors(
            AppBlocksServiceAttribute attribute,
            IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> registration)
        {
            //bool isEnabledForInterception = false;
            // Return if no interceptors are set on service
            if ((attribute.Interceptors?.Count() ?? 0) == 0 &&
                !(attribute.Workflows ?? new string[0]).Any(s => !string.IsNullOrWhiteSpace(s))) return;

            // Add interceptor for service
            registration = registration
                    .EnableInterfaceInterceptors()
                    .InterceptedBy(typeof(AppBlocksServiceInterceptor));

            //var interceptorsSet = attribute.Interceptors.ToHashSet<string>();
            //if (interceptorsSet.Contains(AppBlocksInterceptorConstants.Logging))
            //{
            //    registration = registration
            //        .EnableInterfaceInterceptors().InterceptedBy(typeof(LoggingInterceptor));
            //    isEnabledForInterception = true;
            //}

            //if (interceptorsSet.Contains(AppBlocksInterceptorConstants.Validation))
            //{
            //    if (isEnabledForInterception) registration = registration.InterceptedBy(typeof(ValidationInterceptor));
            //    else registration.EnableInterfaceInterceptors().InterceptedBy(typeof(ValidationInterceptor));
            //    isEnabledForInterception = true;
            //}

            ////A class should only be incercepted by workflow interceptor if 
            ////Worflows fields has at least one non whitespace workflow name
            //if ((attribute.Workflows ?? new string[0]).Any(s => !string.IsNullOrWhiteSpace(s)))
            //{
            //    if (isEnabledForInterception) registration = registration.InterceptedBy(typeof(WorkflowInterceptor));
            //    else registration.EnableInterfaceInterceptors().InterceptedBy(typeof(WorkflowInterceptor));
            //    isEnabledForInterception = true;
            //}
        }

        /// <summary>
        /// Check if service is assignable to service type
        /// </summary>        
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
