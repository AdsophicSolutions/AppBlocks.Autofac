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
    internal static class RegistrationUtils
    {
        private static readonly ILog logger =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        internal static void RegisterAssembly(Assembly assembly,
            ContainerBuilder builder,
            AppBlocksContainerBuilder appBlocksContainerBuilder)
        {
            RegisterAsInterfaces(builder, assembly, appBlocksContainerBuilder);
            RegisterNamedServices(builder, assembly, appBlocksContainerBuilder);
            RegisterKeyedServices(builder, assembly, appBlocksContainerBuilder);
            RegisterMediatrRequestServices(builder, assembly, appBlocksContainerBuilder);
            RegisterMediatrNotificationServices(builder, assembly, appBlocksContainerBuilder);
        }

        /// <summary>
        /// Registers types attributed with AutofacServiceAttribute as interfaces. 
        /// </summary>
        /// <param name="builder">Services Container builder</param>
        /// <param name="assembly">Assembly to process</param>
        private static void RegisterAsInterfaces(
            ContainerBuilder builder, 
            Assembly assembly,
            AppBlocksContainerBuilder appBlocksContainerBuilder)
        {
            bool isAnonymousService(Type t)
            {
                var serviceAttributes = t.GetCustomAttributes(typeof(AppBlocksServiceAttribute), true);
                if (serviceAttributes.Length == 0) return false;

                var serviceAttribute = (AppBlocksServiceAttribute)serviceAttributes[0];

                if (!string.IsNullOrEmpty(serviceAttribute.Name) || serviceAttribute.IsKeyed) return false;

                if (appBlocksContainerBuilder.ApplicationMode == AppBlocksApplicationMode.Test
                    && serviceAttribute.ServiceDependencyType == AppBlocksServiceDependencyType.Live) return false;

                return appBlocksContainerBuilder.ShouldRegisterService(t, serviceAttribute);
            }

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
                    ValidateRegistration(i.AttributeInformation);

                    if (logger.IsDebugEnabled)
                        logger.Debug($"Registering {i.TypeInformation.FullName} as implemented interfaces");

                    var registration = builder.RegisterType(i.TypeInformation).AsImplementedInterfaces();
                    SetTypeScope(i.AttributeInformation, registration);
                    AddTypeInterceptors(i.AttributeInformation, registration);
                });
        }

        private static void RegisterNamedServices(
            ContainerBuilder builder, 
            Assembly assembly,
            AppBlocksContainerBuilder appBlocksContainerBuilder)
        {
            bool isNamedService(Type t)
            {
                var serviceAttributes = t.GetCustomAttributes(typeof(AppBlocksServiceAttribute), true);
                if (serviceAttributes.Length == 0) return false;

                var serviceAttribute = (AppBlocksServiceAttribute)serviceAttributes[0];

                if (string.IsNullOrEmpty(serviceAttribute.Name) || serviceAttribute.IsKeyed) return false;

                if (appBlocksContainerBuilder.ApplicationMode == AppBlocksApplicationMode.Test
                    && serviceAttribute.ServiceDependencyType == AppBlocksServiceDependencyType.Live) return false;

                return appBlocksContainerBuilder.ShouldRegisterService(t, serviceAttribute);
            }

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
                    ValidateRegistration(i.AttributeInformation);

                    //Find out what to Interface to use for service registration
                    if (logger.IsDebugEnabled)
                        logger.Debug($"Registering {i.TypeInformation.FullName} as named service {i.AttributeInformation.Name}");

                    var registrationType = GetServiceRegistrationType(i.TypeInformation, i.AttributeInformation);
                    var registration = builder.RegisterType(i.TypeInformation).Named(i.AttributeInformation.Name, registrationType);
                    SetTypeScope(i.AttributeInformation, registration);
                    AddTypeInterceptors(i.AttributeInformation, registration);
                });
        }

        private static void RegisterKeyedServices(
            ContainerBuilder builder, 
            Assembly assembly,            
            AppBlocksContainerBuilder appBlocksContainerBuilder)
        {
            bool isKeyedService(Type t)
            {
                var serviceAttributes = t.GetCustomAttributes(typeof(AppBlocksServiceAttribute), true);
                if (serviceAttributes.Length == 0) return false;

                var serviceAttribute = (AppBlocksServiceAttribute)serviceAttributes[0];

                if (string.IsNullOrEmpty(serviceAttribute.Name) || !serviceAttribute.IsKeyed) return false;

                if (appBlocksContainerBuilder.ApplicationMode == AppBlocksApplicationMode.Test
                    && serviceAttribute.ServiceDependencyType == AppBlocksServiceDependencyType.Live) return false;

                return appBlocksContainerBuilder.ShouldRegisterService(t, serviceAttribute);
            }

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
                    ValidateRegistration(i.AttributeInformation);

                    //Find out what to Interface to use for service registration. For keyed service, we enforce
                    //having attribute declared with non-null ServiceType. In other words, keyed services 
                    //must explicitly specify service type.  
                    if (logger.IsDebugEnabled)
                        logger.Debug($"Registering {i.TypeInformation.FullName} as keyed service of type {i.AttributeInformation.ServiceType.FullName} " +
                            $"with key {i.AttributeInformation.Name}");

                    var registrationType = GetServiceRegistrationType(i.TypeInformation, i.AttributeInformation);
                    var registration = builder.RegisterType(i.TypeInformation).Keyed(i.AttributeInformation.Name, registrationType);
                    SetTypeScope(i.AttributeInformation, registration);
                    AddTypeInterceptors(i.AttributeInformation, registration);
                });
        }

        private static void RegisterMediatrRequestServices(
            ContainerBuilder builder, 
            Assembly assembly,
            AppBlocksContainerBuilder appBlocksContainerBuilder)
        {
            bool isMediatrRequestService(Type t)
            {
                var serviceAttributes = t.GetCustomAttributes(typeof(AppBlocksMediatrRequestServiceAttribute), true);
                if (serviceAttributes.Length == 0) return false;

                var serviceAttribute = (AppBlocksMediatrRequestServiceAttribute)serviceAttributes[0];

                if (appBlocksContainerBuilder.ApplicationMode == AppBlocksApplicationMode.Test
                    && serviceAttribute.ServiceDependencyType == AppBlocksServiceDependencyType.Live) return false;

                return appBlocksContainerBuilder.ShouldRegisterService(t, serviceAttribute);
            }

            assembly.GetTypes()
               .Where(t => isMediatrRequestService(t))
               .Select(t =>
                   new
                   {
                       TypeInformation = t,
                       AttributeInformation = (AppBlocksMediatrRequestServiceAttribute)t
                           .GetCustomAttribute(typeof(AppBlocksMediatrRequestServiceAttribute))
                   })
               .ToList()
               .ForEach(i =>
               {
                   if (!i.TypeInformation.IsClosedTypeOf(typeof(IRequestHandler<,>)))
                   {
                       throw new Exception($"{i.TypeInformation.FullName} must closed type of " +
                           $"Mediatr.IRequestHandler<,>");
                   }

                   if (logger.IsDebugEnabled)
                       logger.Debug($"Registering {i.TypeInformation.FullName} as Mediatr Request Handler");

                   var registration = builder
                    .RegisterType(i.TypeInformation)
                    .AsImplementedInterfaces();

                   SetTypeScope(i.AttributeInformation, registration);
               });
        }

        private static void RegisterMediatrNotificationServices(
            ContainerBuilder builder, 
            Assembly assembly,
            AppBlocksContainerBuilder appBlocksContainerBuilder)
        {
            bool isMediatrRequestService(Type t)
            {
                var serviceAttributes = t.GetCustomAttributes(typeof(AppBlocksMediatrNotificationServiceAttribute), true);
                if (serviceAttributes.Length == 0) return false;

                var serviceAttribute = (AppBlocksMediatrNotificationServiceAttribute)serviceAttributes[0];

                if (appBlocksContainerBuilder.ApplicationMode == AppBlocksApplicationMode.Test
                    && serviceAttribute.ServiceDependencyType == AppBlocksServiceDependencyType.Live) return false;

                return appBlocksContainerBuilder.ShouldRegisterService(t, serviceAttribute);
            }

            assembly.GetTypes()
               .Where(t => isMediatrRequestService(t))
               .Select(t =>
                   new
                   {
                       TypeInformation = t,
                       AttributeInformation = (AppBlocksMediatrNotificationServiceAttribute)t
                           .GetCustomAttribute(typeof(AppBlocksMediatrNotificationServiceAttribute))
                   })
               .ToList()
               .ForEach(i =>
               {
                   if (!i.TypeInformation.IsClosedTypeOf(typeof(INotificationHandler<>)))
                   {
                       throw new Exception($"{i.TypeInformation.FullName} must closed type of " +
                           $"Mediatr.INotificationHandler<>");
                   }

                   if (logger.IsDebugEnabled)
                       logger.Debug($"Registering {i.TypeInformation.FullName} as Mediatr Notification Handler");

                   var registration = builder
                    .RegisterType(i.TypeInformation)
                    .AsImplementedInterfaces();

                   SetTypeScope(i.AttributeInformation, registration);
               });
        }

        private static void ValidateRegistration(AppBlocksServiceAttribute attributeInformation)
        {
            // No further checks necessary if ServiceType is not set
            if (attributeInformation.ServiceType == null) return;

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

        private static void SetTypeScope(
            AppBlocksServiceAttributeBase attribute,
            IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> registration)
        {
            //Service is available as all implemented interfaces. 
            switch (attribute.ServiceScope)
            {
                case AppBlocksInstanceLifetime.SingleInstance:
                    registration.SingleInstance();
                    break;

                case AppBlocksInstanceLifetime.InstancePerDependency:
                    registration.InstancePerDependency();
                    break;

                case AppBlocksInstanceLifetime.InstancePerLifetimeScope:
                    registration.InstancePerLifetimeScope();
                    break;
            }
        }

        private static void AddTypeInterceptors(
            AppBlocksServiceAttribute attribute,
            IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> registration)
        {
            //bool isEnabledForInterception = false;
            if ((attribute.Interceptors?.Count() ?? 0) == 0 &&
                !(attribute.Workflows ?? new string[0]).Any(s => !string.IsNullOrWhiteSpace(s))) return;

            registration = registration
                    .EnableInterfaceInterceptors().InterceptedBy(typeof(AppBlocksServiceInterceptor));

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
