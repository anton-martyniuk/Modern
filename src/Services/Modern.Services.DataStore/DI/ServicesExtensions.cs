using Microsoft.Extensions.DependencyInjection.Extensions;
using Modern.Extensions.Microsoft.DependencyInjection.Models;
using Modern.Services.DataStore;
using Modern.Services.DataStore.Abstractions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Modern.Data.Services extensions for Microsoft.Extensions.DependencyInjection
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Adds services into DI
    /// </summary>
    /// <param name="builder">Modern services builder</param>
    /// <param name="configure">Services configure delegate</param>
    /// <returns>IServiceCollection</returns>
    public static ModernServicesBuilder AddServices(this ModernServicesBuilder builder, Action<ModernServicesOptions> configure)
    {
        var options = new ModernServicesOptions();
        configure(options);

        foreach (var c in options.Services)
        {
            var interfaceType = typeof(IModernService<,,>).MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType);
            var implementationType = typeof(ModernService<,,>).MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType);

            builder.Services.TryAdd(new ServiceDescriptor(interfaceType, implementationType, c.Lifetime));
        }

        foreach (var c in options.ConcreteServices)
        {
            builder.Services.TryAdd(new ServiceDescriptor(c.InterfaceType, c.ImplementationType, c.Lifetime));
        }

        return builder;
    }

    ///// <summary>
    ///// gdfgfdg
    ///// </summary>
    ///// <param name="services"></param>
    ///// <param name="assembliesToScan"></param>
    //public static void AddMediatRClasses(IServiceCollection services, IEnumerable<Assembly> assembliesToScan)
    //{
    //    assembliesToScan = assembliesToScan.Distinct().ToArray();

    //    var serviceInterface = typeof(IModernService<,,>);
    //    var argumentsCount = serviceInterface.GetGenericArguments().Length;

    //    ConnectImplementationsToTypesClosing(services, serviceInterface, assembliesToScan, true);

    //    var concretions = assembliesToScan
    //        .SelectMany(x => x.DefinedTypes)
    //        .Where(x => x.FindInterfacesThatClose(serviceInterface).Any()
    //                    && x.IsConcrete()
    //                    && x.IsOpenGeneric()
    //                    && x.GetGenericArguments().Length == argumentsCount)
    //        .ToList();

    //    // TODO: configure this
    //    foreach (var type in concretions)
    //    {
    //        services.AddTransient(serviceInterface, type);
    //    }
    //}

    //private static void ConnectImplementationsToTypesClosing(IServiceCollection services, Type openRequestInterface,
    //    IEnumerable<Assembly> assembliesToScan, bool addIfAlreadyExists)
    //{
    //    var concretions = new List<Type>();
    //    var interfaces = new List<Type>();

    //    foreach (var type in assembliesToScan.SelectMany(x => x.DefinedTypes).Where(x => !x.IsOpenGeneric()))
    //    {
    //        var interfaceTypes = type.FindInterfacesThatClose(openRequestInterface).ToArray();
    //        if (interfaceTypes.Length == 0)
    //        {
    //            continue;
    //        }

    //        if (type.IsConcrete())
    //        {
    //            concretions.Add(type);
    //        }

    //        foreach (var interfaceType in interfaceTypes)
    //        {
    //            if (!interfaces.Contains(interfaceType))
    //            {
    //                interfaces.Add(interfaceType);
    //            }
    //        }
    //    }

    //    foreach (var @interface in interfaces)
    //    {
    //        var exactMatches = concretions.Where(x => x.CanBeCastTo(@interface)).ToList();
    //        if (addIfAlreadyExists)
    //        {
    //            foreach (var type in exactMatches)
    //            {
    //                services.AddTransient(@interface, type);
    //            }
    //        }
    //        else
    //        {
    //            if (exactMatches.Count > 1)
    //            {
    //                exactMatches.RemoveAll(m => !IsMatchingWithInterface(m, @interface));
    //            }

    //            foreach (var type in exactMatches)
    //            {
    //                services.TryAddTransient(@interface, type);
    //            }
    //        }

    //        if (!@interface.IsOpenGeneric())
    //        {
    //            AddConcretionsThatCouldBeClosed(@interface, concretions, services);
    //        }
    //    }
    //}

    //private static bool IsMatchingWithInterface(Type handlerType, Type handlerInterface)
    //{
    //    if (handlerType == null || handlerInterface == null)
    //    {
    //        return false;
    //    }

    //    if (handlerType.IsInterface)
    //    {
    //        if (handlerType.GenericTypeArguments.SequenceEqual(handlerInterface.GenericTypeArguments))
    //        {
    //            return true;
    //        }
    //    }
    //    else
    //    {
    //        return IsMatchingWithInterface(handlerType.GetInterface(handlerInterface.Name), handlerInterface);
    //    }

    //    return false;
    //}

    //private static void AddConcretionsThatCouldBeClosed(Type @interface, List<Type> concretions, IServiceCollection services)
    //{
    //    foreach (var type in concretions.Where(x => x.IsOpenGeneric() && x.CouldCloseTo(@interface)))
    //    {
    //        try
    //        {
    //            services.TryAddTransient(@interface, type.MakeGenericType(@interface.GenericTypeArguments));
    //        }
    //        catch (Exception)
    //        {
    //        }
    //    }
    //}

    //private static bool CouldCloseTo(this Type openConcretion, Type closedInterface)
    //{
    //    var openInterface = closedInterface.GetGenericTypeDefinition();
    //    var arguments = closedInterface.GenericTypeArguments;

    //    var concreteArguments = openConcretion.GenericTypeArguments;
    //    return arguments.Length == concreteArguments.Length && openConcretion.CanBeCastTo(openInterface);
    //}

    //private static bool CanBeCastTo(this Type pluggedType, Type pluginType)
    //{
    //    if (pluggedType == null) return false;

    //    if (pluggedType == pluginType) return true;

    //    return pluginType.GetTypeInfo().IsAssignableFrom(pluggedType.GetTypeInfo());
    //}

    //private static IEnumerable<Type> FindInterfacesThatClose(this Type pluggedType, Type templateType)
    //{
    //    return FindInterfacesThatClosesCore(pluggedType, templateType).Distinct();
    //}

    //private static IEnumerable<Type> FindInterfacesThatClosesCore(Type? pluggedType, Type templateType)
    //{
    //    if (pluggedType is null)
    //    {
    //        yield break;
    //    }

    //    if (!pluggedType.IsConcrete())
    //    {
    //        yield break;
    //    }

    //    if (templateType.GetTypeInfo().IsInterface)
    //    {
    //        foreach (var interfaceType in pluggedType.GetInterfaces()
    //                .Where(type => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == templateType))
    //        {
    //            yield return interfaceType;
    //        }
    //    }
    //    else if (pluggedType.GetTypeInfo().BaseType.GetTypeInfo().IsGenericType &&
    //             pluggedType.GetTypeInfo().BaseType.GetGenericTypeDefinition() == templateType)
    //    {
    //        yield return pluggedType.GetTypeInfo().BaseType;
    //    }

    //    if (pluggedType.GetTypeInfo().BaseType == typeof(object))
    //    {
    //        yield break;
    //    }

    //    foreach (var interfaceType in FindInterfacesThatClosesCore(pluggedType.GetTypeInfo().BaseType, templateType))
    //    {
    //        yield return interfaceType;
    //    }
    //}

    //private static bool IsOpenGeneric(this Type type)
    //{
    //    return type.GetTypeInfo().IsGenericTypeDefinition || type.GetTypeInfo().ContainsGenericParameters;
    //}

    //private static bool IsConcrete(this Type type)
    //{
    //    return !type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsInterface;
    //}

    //public static void AddRequiredServices(IServiceCollection services, MediatRServiceConfiguration serviceConfiguration)
    //{
    //    // Use TryAdd, so any existing ServiceFactory/IMediator registration doesn't get overriden
    //    services.TryAddTransient<ServiceFactory>(p => p.GetRequiredService);
    //    services.TryAdd(new ServiceDescriptor(typeof(IMediator), serviceConfiguration.MediatorImplementationType, serviceConfiguration.Lifetime));
    //    services.TryAdd(new ServiceDescriptor(typeof(ISender), sp => sp.GetRequiredService<IMediator>(), serviceConfiguration.Lifetime));
    //    services.TryAdd(new ServiceDescriptor(typeof(IPublisher), sp => sp.GetRequiredService<IMediator>(), serviceConfiguration.Lifetime));

    //    // Use TryAddTransientExact (see below), we dó want to register our Pre/Post processor behavior, even if (a more concrete)
    //    // registration for IPipelineBehavior<,> already exists. But only once.
    //    services.TryAddTransientExact(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
    //    services.TryAddTransientExact(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));

    //    if (serviceConfiguration.RequestExceptionActionProcessorStrategy == RequestExceptionActionProcessorStrategy.ApplyForUnhandledExceptions)
    //    {
    //        services.TryAddTransientExact(typeof(IPipelineBehavior<,>), typeof(RequestExceptionActionProcessorBehavior<,>));
    //        services.TryAddTransientExact(typeof(IPipelineBehavior<,>), typeof(RequestExceptionProcessorBehavior<,>));
    //    }
    //    else
    //    {
    //        services.TryAddTransientExact(typeof(IPipelineBehavior<,>), typeof(RequestExceptionProcessorBehavior<,>));
    //        services.TryAddTransientExact(typeof(IPipelineBehavior<,>), typeof(RequestExceptionActionProcessorBehavior<,>));
    //    }
    //}

    ///// <summary>
    ///// Adds a new transient registration to the service collection only when no existing registration of the same service type and implementation type exists.
    ///// In contrast to TryAddTransient, which only checks the service type.
    ///// </summary>
    ///// <param name="services">The service collection</param>
    ///// <param name="serviceType">Service type</param>
    ///// <param name="implementationType">Implementation type</param>
    //private static void TryAddTransientExact(this IServiceCollection services, Type serviceType, Type implementationType)
    //{
    //    if (services.Any(reg => reg.ServiceType == serviceType && reg.ImplementationType == implementationType))
    //    {
    //        return;
    //    }

    //    services.AddTransient(serviceType, implementationType);
    //}
}
