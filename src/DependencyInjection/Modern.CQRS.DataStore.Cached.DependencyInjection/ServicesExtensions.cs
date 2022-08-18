using MediatR;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Modern.CQRS.DataStore.Abstractions.Commands;
using Modern.CQRS.DataStore.Abstractions.Queries;
using Modern.CQRS.DataStore.Cached.CommandHandlers;
using Modern.CQRS.DataStore.Cached.QueryHandlers;
using Modern.CQRS.DependencyInjection.Definitions.Configuration;
using Modern.Data.Paging;
using Modern.Extensions.Microsoft.DependencyInjection.Models;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Modern.CQRS.DataStore extensions for Microsoft.Extensions.DependencyInjection
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Adds CQRS queries, commands and their handlers into DI
    /// </summary>
    /// <param name="builder">Modern services builder</param>
    /// <param name="configure">Services configure delegate</param>
    /// <returns>IServiceCollection</returns>
    public static ModernServicesBuilder AddCachedCqrs(this ModernServicesBuilder builder, Action<ModernCqrsOptions> configure)
    {
        var options = new ModernCqrsOptions();
        configure(options);

        foreach (var c in options.CqrsRequests)
        {
            var nullableType = GetNullableType(c.EntityDtoType);
            var listType = typeof(List<>).MakeGenericType(c.EntityDtoType);
            var pagedType = typeof(PagedResult<>).MakeGenericType(c.EntityDtoType);

            builder.Services.AddDoubleArityRequestAndHandler(typeof(GetByIdQuery<,>), typeof(GetByIdQueryHandler<,,,>), c.EntityDtoType, c);
            builder.Services.AddDoubleArityRequestAndHandler(typeof(TryGetByIdQuery<,>), typeof(TryGetByIdQueryHandler<,,,>), nullableType, c);
            builder.Services.AddDoubleArityRequestAndHandler(typeof(GetAllQuery<,>), typeof(GetAllQueryHandler<,,,>), listType, c);
            builder.Services.AddDoubleArityRequestAndHandler(typeof(GetCountAllQuery<,>), typeof(GetCountAllQueryHandler<,,,>), typeof(long), c);
            builder.Services.AddDoubleArityRequestAndHandler(typeof(GetCountQuery<,>), typeof(GetCountQueryHandler<,,,>), typeof(long), c);
            builder.Services.AddDoubleArityRequestAndHandler(typeof(GetExistsQuery<,>), typeof(GetExistsQueryHandler<,,,>), typeof(bool), c);
            builder.Services.AddTripleArityRequestAndHandler(typeof(GetFirstOrDefaultQuery<,,>), typeof(GetFirstOrDefaultQueryHandler<,,,>), nullableType, c);
            builder.Services.AddTripleArityRequestAndHandler(typeof(GetSingleOrDefaultQuery<,,>), typeof(GetSingleOrDefaultQueryHandler<,,,>), nullableType, c); 
            builder.Services.AddTripleArityRequestAndHandler(typeof(GetWhereQuery<,,>), typeof(GetWhereQueryHandler<,,,>), listType, c);
            builder.Services.AddTripleArityRequestAndHandler(typeof(GetWherePagedQuery<,,>), typeof(GetWherePagedQueryHandler<,,,>), pagedType, c);

            builder.Services.AddSingleArityRequestAndHandler(typeof(CreateEntityCommand<>), typeof(CreateEntityCommandHandler<,,,>), c.EntityDtoType, c);
            builder.Services.AddSingleArityRequestAndHandler(typeof(CreateEntitiesCommand<>), typeof(CreateEntitiesCommandHandler<,,,>), listType, c);
            builder.Services.AddDoubleArityRequestAndHandler(typeof(UpdateEntityCommand<,>), typeof(UpdateEntityCommandHandler<,,,>), c.EntityDtoType, c);
            builder.Services.AddTripleArityRequestAndHandler(typeof(UpdateEntityByActionCommand<,,>), typeof(UpdateEntityByActionCommandHandler<,,,>), c.EntityDtoType, c);
            builder.Services.AddSingleArityRequestAndHandler(typeof(UpdateEntitiesCommand<>), typeof(UpdateEntitiesCommandHandler<,,,>), listType, c);
            builder.Services.AddByIdRequestAndHandler(typeof(DeleteEntityCommand<>), typeof(DeleteEntityCommandHandler<,,,>), typeof(bool), c);
            builder.Services.AddByIdRequestAndHandler(typeof(DeleteEntitiesCommand<>), typeof(DeleteEntitiesCommandHandler<,,,>), typeof(bool), c);
            builder.Services.AddDoubleArityRequestAndHandler(typeof(DeleteAndReturnEntityCommand<,>), typeof(DeleteAndReturnEntityCommandHandler<,,,>), c.EntityDtoType, c);
        }

        return builder;
    }

    private static void AddSingleArityRequestAndHandler(this IServiceCollection services, Type reguestType, Type requestHandlerType, Type returnType, ModernCqrsSpecification c)
    {
        var requestInterfaceType = reguestType.MakeGenericType(c.EntityDtoType);
        var requestHandlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(requestInterfaceType, returnType);
        var requestHandlerImplementationType = requestHandlerType.MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType, c.RepositoryType);

        services.TryAdd(new ServiceDescriptor(requestHandlerInterfaceType, requestHandlerImplementationType, c.Lifetime));
    }

    private static void AddDoubleArityRequestAndHandler(this IServiceCollection services, Type reguestType, Type requestHandlerType, Type returnType, ModernCqrsSpecification c)
    {
        var requestInterfaceType = reguestType.MakeGenericType(c.EntityDtoType, c.EntityIdType);
        var requestHandlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(requestInterfaceType, returnType);
        var requestHandlerImplementationType = requestHandlerType.MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType, c.RepositoryType);

        services.TryAdd(new ServiceDescriptor(requestHandlerInterfaceType, requestHandlerImplementationType, c.Lifetime));
    }

    private static void AddTripleArityRequestAndHandler(this IServiceCollection services, Type reguestType, Type requestHandlerType, Type returnType, ModernCqrsSpecification c)
    {
        var requestInterfaceType = reguestType.MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType);
        var requestHandlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(requestInterfaceType, returnType);
        var requestHandlerImplementationType = requestHandlerType.MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType, c.RepositoryType);

        services.TryAdd(new ServiceDescriptor(requestHandlerInterfaceType, requestHandlerImplementationType, c.Lifetime));
    }

    private static void AddByIdRequestAndHandler(this IServiceCollection services, Type reguestType, Type requestHandlerType, Type returnType, ModernCqrsSpecification c)
    {
        var requestInterfaceType = reguestType.MakeGenericType(c.EntityIdType);
        var requestHandlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(requestInterfaceType, returnType);
        var requestHandlerImplementationType = requestHandlerType.MakeGenericType(c.EntityDtoType, c.EntityDboType, c.EntityIdType, c.RepositoryType);

        services.TryAdd(new ServiceDescriptor(requestHandlerInterfaceType, requestHandlerImplementationType, c.Lifetime));
    }

    private static Type GetNullableType(Type type)
    {
        var resultType = Nullable.GetUnderlyingType(type) ?? type;
        return resultType.IsValueType ? typeof(Nullable<>).MakeGenericType(resultType) : resultType;
    }
}
