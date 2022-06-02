using MediatR;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Modern.CQRS.DataStore.Abstractions.Queries;
using Modern.CQRS.DataStore.DependencyInjection.Configuration;
using Modern.CQRS.DataStore.QueryHandlers;
using Modern.CQRS.DependencyInjection.Definitions.Configuration;
using Modern.Extensions.Microsoft.DependencyInjection.Models;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Modern.Data.Services extensions for Microsoft.Extensions.DependencyInjection
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Adds CQRS queries, commands and their handlers into DI
    /// </summary>
    /// <param name="builder">Modern services builder</param>
    /// <param name="configure">Services configure delegate</param>
    /// <returns>IServiceCollection</returns>
    public static ModernServicesBuilder AddServices(this ModernServicesBuilder builder, Action<ModernCqrsOptions> configure)
    {
        //services.AddScoped<IRequestHandler<GetByIdQuery<Airplane, long>, Airplane>,
        //    GetByIdQueryHandler<Airplane, Airplane, long, IAirplaneRepository>>();

        var options = new ModernCqrsOptions();
        configure(options);

        foreach (var c in options.Services)
        {
            builder.Services.AddGetByIdQuery(c);
            builder.Services.AddTryGetByIdQuery(c);
            builder.Services.AddGetAllQuery(c);
            builder.Services.AddGetCountAllQuery(c);
            builder.Services.AddGetCountQuery(c);
            builder.Services.AddGetExistsQuery(c);
            builder.Services.AddGetFirstOrDefaultQuery(c);
            builder.Services.AddGetSingleOrDefaultQuery(c);
            builder.Services.AddGetWhereQuery(c);
            builder.Services.AddGetWherePagedQuery(c);

            // TODO: add commands
        }

        foreach (var c in options.ConcreteServices)
        {
            builder.Services.TryAdd(new ServiceDescriptor(c.InterfaceType, c.ImplementationType, c.Lifetime));
        }

        return builder;
    }

    private static void AddGetByIdQuery(this IServiceCollection services, ModernCqrsSpecification c)
    {
        var queryInterfaceType = typeof(GetByIdQuery<,>).MakeGenericType(c.EntityDtoType, c.EntityIdType);
        var requestHandlerInterfaceType = typeof(IRequestHandler<>).MakeGenericType(queryInterfaceType);
        var requestHandlerImplementationType = typeof(GetByIdQueryHandler<,,,>).MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType, c.RepositoryType);

        services.TryAdd(new ServiceDescriptor(requestHandlerInterfaceType, requestHandlerImplementationType, c.Lifetime));
    }

    private static void AddTryGetByIdQuery(this IServiceCollection services, ModernCqrsSpecification c)
    {
        var queryInterfaceType = typeof(TryGetByIdQuery<,>).MakeGenericType(c.EntityDtoType, c.EntityIdType);
        var requestHandlerInterfaceType = typeof(IRequestHandler<>).MakeGenericType(queryInterfaceType);
        var requestHandlerImplementationType = typeof(TryGetByIdQueryHandler<,,,>).MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType, c.RepositoryType);

        services.TryAdd(new ServiceDescriptor(requestHandlerInterfaceType, requestHandlerImplementationType, c.Lifetime));
    }

    private static void AddGetAllQuery(this IServiceCollection services, ModernCqrsSpecification c)
    {
        var queryInterfaceType = typeof(GetAllQuery<,>).MakeGenericType(c.EntityDtoType, c.EntityIdType);
        var requestHandlerInterfaceType = typeof(IRequestHandler<>).MakeGenericType(queryInterfaceType);
        var requestHandlerImplementationType = typeof(GetAllQueryHandler<,,,>).MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType, c.RepositoryType);

        services.TryAdd(new ServiceDescriptor(requestHandlerInterfaceType, requestHandlerImplementationType, c.Lifetime));
    }

    private static void AddGetCountAllQuery(this IServiceCollection services, ModernCqrsSpecification c)
    {
        var queryInterfaceType = typeof(GetCountAllQuery<,>).MakeGenericType(c.EntityDtoType, c.EntityIdType);
        var requestHandlerInterfaceType = typeof(IRequestHandler<>).MakeGenericType(queryInterfaceType);
        var requestHandlerImplementationType = typeof(GetCountAllQueryHandler<,,,>).MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType, c.RepositoryType);

        services.TryAdd(new ServiceDescriptor(requestHandlerInterfaceType, requestHandlerImplementationType, c.Lifetime));
    }

    private static void AddGetCountQuery(this IServiceCollection services, ModernCqrsSpecification c)
    {
        var queryInterfaceType = typeof(GetCountQuery<,>).MakeGenericType(c.EntityDtoType, c.EntityIdType);
        var requestHandlerInterfaceType = typeof(IRequestHandler<>).MakeGenericType(queryInterfaceType);
        var requestHandlerImplementationType = typeof(GetCountQueryHandler<,,,>).MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType, c.RepositoryType);

        services.TryAdd(new ServiceDescriptor(requestHandlerInterfaceType, requestHandlerImplementationType, c.Lifetime));
    }

    private static void AddGetExistsQuery(this IServiceCollection services, ModernCqrsSpecification c)
    {
        var queryInterfaceType = typeof(GetExistsQuery<,>).MakeGenericType(c.EntityDtoType, c.EntityIdType);
        var requestHandlerInterfaceType = typeof(IRequestHandler<>).MakeGenericType(queryInterfaceType);
        var requestHandlerImplementationType = typeof(GetExistsQueryHandler<,,,>).MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType, c.RepositoryType);

        services.TryAdd(new ServiceDescriptor(requestHandlerInterfaceType, requestHandlerImplementationType, c.Lifetime));
    }

    private static void AddGetFirstOrDefaultQuery(this IServiceCollection services, ModernCqrsSpecification c)
    {
        var queryInterfaceType = typeof(GetFirstOrDefaultQuery<,,>).MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType);
        var requestHandlerInterfaceType = typeof(IRequestHandler<>).MakeGenericType(queryInterfaceType);
        var requestHandlerImplementationType = typeof(GetFirstOrDefaultQueryHandler<,,,>).MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType, c.RepositoryType);

        services.TryAdd(new ServiceDescriptor(requestHandlerInterfaceType, requestHandlerImplementationType, c.Lifetime));
    }

    private static void AddGetSingleOrDefaultQuery(this IServiceCollection services, ModernCqrsSpecification c)
    {
        var queryInterfaceType = typeof(GetSingleOrDefaultQuery<,,>).MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType);
        var requestHandlerInterfaceType = typeof(IRequestHandler<>).MakeGenericType(queryInterfaceType);
        var requestHandlerImplementationType = typeof(GetSingleOrDefaultQueryHandler<,,,>).MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType, c.RepositoryType);

        services.TryAdd(new ServiceDescriptor(requestHandlerInterfaceType, requestHandlerImplementationType, c.Lifetime));
    }

    private static void AddGetWhereQuery(this IServiceCollection services, ModernCqrsSpecification c)
    {
        var queryInterfaceType = typeof(GetWhereQuery<,,>).MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType);
        var requestHandlerInterfaceType = typeof(IRequestHandler<>).MakeGenericType(queryInterfaceType);
        var requestHandlerImplementationType = typeof(GetWhereQueryHandler<,,,>).MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType, c.RepositoryType);

        services.TryAdd(new ServiceDescriptor(requestHandlerInterfaceType, requestHandlerImplementationType, c.Lifetime));
    }

    private static void AddGetWherePagedQuery(this IServiceCollection services, ModernCqrsSpecification c)
    {
        var queryInterfaceType = typeof(GetWherePagedQuery<,,>).MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType);
        var requestHandlerInterfaceType = typeof(IRequestHandler<>).MakeGenericType(queryInterfaceType);
        var requestHandlerImplementationType = typeof(GetWherePagedQueryHandler<,,,>).MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType, c.RepositoryType);

        services.TryAdd(new ServiceDescriptor(requestHandlerInterfaceType, requestHandlerImplementationType, c.Lifetime));
    }
}
