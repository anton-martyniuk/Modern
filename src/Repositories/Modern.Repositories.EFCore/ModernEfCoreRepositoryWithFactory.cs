﻿using Microsoft.EntityFrameworkCore;
using Modern.Repositories.EFCore.Configuration;
using System.Data;
using System.Linq.Expressions;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Options;
using Modern.Data.Paging;
using Modern.Exceptions;
using Modern.Repositories.Abstractions;
using Modern.Repositories.Abstractions.Infrastracture;
using Modern.Repositories.Abstractions.Specifications;
using Modern.Repositories.EFCore.Query;
using Modern.Repositories.EFCore.Specifications;

namespace Modern.Repositories.EFCore;

/// <summary>
/// Represents an <see cref="IModernCrudRepository{TEntity, TId}"/> and <see cref="IModernQueryRepository{TEntity, TId}"/> implementation using EFCore
/// and <see cref="IDbContextFactory{TDbContext}"/>
/// </summary>
/// <typeparam name="TDbContext">The type of EF Core DbContext</typeparam>
/// <typeparam name="TEntity">The type of entity</typeparam>
/// <typeparam name="TId">The type of entity identifier</typeparam>
public class ModernEfCoreRepositoryWithFactory<TDbContext, TEntity, TId> : IModernRepository<TEntity, TId>
    where TDbContext : DbContext
    where TEntity : class
    where TId : IEquatable<TId>
{
    private readonly string _entityName = typeof(TEntity).Name;

    private readonly EfCoreRepositoryConfiguration? _configuration;

    /// <summary>
    /// The <typeparamref name="TDbContext"/> factory
    /// </summary>
    protected IDbContextFactory<TDbContext> DbContextFactory { get; }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="dbContextFactory">The <see cref="IDbContextFactory{TDbContext}"/> implementation</param>
    /// <param name="configuration">Repository configuration</param>
    public ModernEfCoreRepositoryWithFactory(IDbContextFactory<TDbContext> dbContextFactory, IOptions<EfCoreRepositoryConfiguration> configuration)
    {
        DbContextFactory = dbContextFactory;
        _configuration = configuration.Value;
    }

    /// <summary>
    /// Returns entity id of type <typeparamref name="TId"/>
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>Entity id</returns>
    protected virtual TId GetEntityId(TEntity entity) => (TId)(entity.GetType().GetProperty("Id")?.GetValue(entity, null) ?? 0);

    /// <summary>
    /// Returns entity include query for all repository methods. <see cref="EntityIncludeQuery{TEntity}"/>
    /// </summary>
    /// <returns>Expression that describes included entities</returns>
    protected virtual EntityIncludeQuery<TEntity>? GetEntityIncludeQuery() =>
        null;

    /// <summary>
    /// Returns standardized repository exception
    /// </summary>
    /// <param name="ex">Original exception</param>
    /// <returns>Repository exception which holds original exception as InnerException</returns>
    protected virtual Exception CreateProperException(Exception ex)
        => ex switch
        {
            ArgumentException _ => ex,
            DbUpdateConcurrencyException _ => new EntityConcurrentUpdateException(ex.Message, ex),
            EntityAlreadyExistsException _ => ex,
            EntityNotFoundException _ => ex,
            EntityNotModifiedException _ => ex,
            _ => new RepositoryErrorException(ex.Message, ex)
        };

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity, TId}.CreateAsync(TEntity,CancellationToken)"/>
    /// </summary>
    public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            cancellationToken.ThrowIfCancellationRequested();

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            if (_configuration?.CreateConfiguration?.NeedExecuteInTransaction is not true)
            {
                await context.Set<TEntity>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return entity;
            }

            var isolationLevel = _configuration?.CreateConfiguration?.TransactionIsolationLevel ?? IsolationLevel.Unspecified;
            await using var transaction = await context.Database.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);

            await context.Set<TEntity>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            return entity;
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity, TId}.CreateAsync(List{TEntity},CancellationToken)"/>
    /// </summary>
    public virtual async Task<List<TEntity>> CreateAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entities, nameof(entities));
            Guard.Against.NegativeOrZero(entities.Count, nameof(entities));
            cancellationToken.ThrowIfCancellationRequested();

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            if (_configuration?.CreateConfiguration?.NeedExecuteInTransaction is not true)
            {
                await context.Set<TEntity>().AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return entities;
            }

            var isolationLevel = _configuration?.CreateConfiguration?.TransactionIsolationLevel ?? IsolationLevel.Unspecified;
            await using var transaction = await context.Database.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);

            await context.Set<TEntity>().AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            return entities;
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity, TId}.UpdateAsync(TId,TEntity,CancellationToken)"/>
    /// </summary>
    public virtual async Task<TEntity> UpdateAsync(TId id, TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            cancellationToken.ThrowIfCancellationRequested();

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            if (_configuration?.UpdateConfiguration?.NeedExecuteInTransaction is not true)
            {
                return await PerformUpdateAsync(context, id, entity, cancellationToken).ConfigureAwait(false);
            }

            var isolationLevel = _configuration?.UpdateConfiguration?.TransactionIsolationLevel ?? IsolationLevel.Unspecified;
            await using var transaction = await context.Database.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);

            entity = await PerformUpdateAsync(context, id, entity, cancellationToken).ConfigureAwait(false);
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            return entity;
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity, TId}.UpdateAsync(List{TEntity},CancellationToken)"/>
    /// </summary>
    public virtual async Task<List<TEntity>> UpdateAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entities, nameof(entities));
            Guard.Against.NegativeOrZero(entities.Count, nameof(entities));
            cancellationToken.ThrowIfCancellationRequested();

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            if (_configuration?.UpdateConfiguration?.NeedExecuteInTransaction is not true)
            {
                return await PerformUpdateAsync(context, entities, cancellationToken).ConfigureAwait(false);
            }

            var isolationLevel = _configuration?.UpdateConfiguration?.TransactionIsolationLevel ?? IsolationLevel.Unspecified;
            await using var transaction = await context.Database.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);

            entities = await PerformUpdateAsync(context, entities, cancellationToken).ConfigureAwait(false);
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            return entities;
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity, TId}.UpdateAsync(TId, Action{TEntity}, CancellationToken)"/>
    /// </summary>
    public virtual async Task<TEntity> UpdateAsync(TId id, Action<TEntity> update, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            cancellationToken.ThrowIfCancellationRequested();

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            if (_configuration?.UpdateConfiguration?.NeedExecuteInTransaction is not true)
            {
                return await PerformUpdateAsync(context, id, update, cancellationToken).ConfigureAwait(false);
            }

            var isolationLevel = _configuration?.UpdateConfiguration?.TransactionIsolationLevel ?? IsolationLevel.Unspecified;
            await using var transaction = await context.Database.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);

            var entity = await PerformUpdateAsync(context, id, update, cancellationToken).ConfigureAwait(false);
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            return entity;
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity, TId}.DeleteAsync(TId,CancellationToken)"/>
    /// </summary>
    public virtual async Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            cancellationToken.ThrowIfCancellationRequested();

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            if (_configuration?.DeleteConfiguration?.NeedExecuteInTransaction != true)
            {
                return await PerformDeleteAsync(context, id, cancellationToken).ConfigureAwait(false);
            }

            var isolationLevel = _configuration?.DeleteConfiguration?.TransactionIsolationLevel ?? IsolationLevel.Unspecified;
            await using var transaction = await context.Database.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);

            var result = await PerformDeleteAsync(context, id, cancellationToken).ConfigureAwait(false);
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity, TId}.DeleteAsync(List{TId},CancellationToken)"/>
    /// </summary>
    public virtual async Task<bool> DeleteAsync(List<TId> ids, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(ids, nameof(ids));
            Guard.Against.NegativeOrZero(ids.Count, nameof(ids));
            cancellationToken.ThrowIfCancellationRequested();

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            if (_configuration?.DeleteConfiguration?.NeedExecuteInTransaction != true)
            {
                return await PerformDeleteAsync(context, ids, cancellationToken).ConfigureAwait(false);
            }

            var isolationLevel = _configuration?.DeleteConfiguration?.TransactionIsolationLevel ?? IsolationLevel.Unspecified;
            await using var transaction = await context.Database.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);

            var result = await PerformDeleteAsync(context, ids, cancellationToken).ConfigureAwait(false);
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <inheritdoc cref="IModernCrudRepository{TEntity, TId}.DeleteAndReturnAsync"/>
    public virtual async Task<TEntity> DeleteAndReturnAsync(TId id, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            cancellationToken.ThrowIfCancellationRequested();

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            if (_configuration?.DeleteConfiguration?.NeedExecuteInTransaction != true)
            {
                return await PerformDeleteAndReturnAsync(context, id, cancellationToken).ConfigureAwait(false);
            }

            var isolationLevel = _configuration?.DeleteConfiguration?.TransactionIsolationLevel ?? IsolationLevel.Unspecified;
            await using var transaction = await context.Database.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);

            var entity = await PerformDeleteAndReturnAsync(context, id, cancellationToken).ConfigureAwait(false);
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            return entity;
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity, TId}.GetByIdAsync"/>
    /// </summary>
    public virtual async Task<TEntity> GetByIdAsync(TId id, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            cancellationToken.ThrowIfCancellationRequested();

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

            var entity = await GetEntityByIdImpl(context, id, includeQuery ?? GetEntityIncludeQuery(), cancellationToken).ConfigureAwait(false);
            if (entity is null)
            {
                throw new EntityNotFoundException($"{_entityName} entity with id '{id}' not found");
            }
            return entity;
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity, TId}.TryGetByIdAsync"/>
    /// </summary>
    public virtual async Task<TEntity?> TryGetByIdAsync(TId id, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            cancellationToken.ThrowIfCancellationRequested();

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken);
            return await GetEntityByIdImpl(context, id, includeQuery ?? GetEntityIncludeQuery(), cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity, TId}.GetAllAsync"/>
    /// </summary>
    public virtual async Task<List<TEntity>> GetAllAsync(EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

            includeQuery ??= GetEntityIncludeQuery();
            if (includeQuery is null)
            {
                return await context.Set<TEntity>().ToListAsync(cancellationToken).ConfigureAwait(false);
            }

            var query = context.Set<TEntity>().AsNoTracking();
            query = includeQuery.GetExpression(query);
            return await query.ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity, TId}.CountAsync(CancellationToken)"/>
    /// </summary>
    public virtual async Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Set<TEntity>().AsNoTracking().CountAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity, TId}.CountAsync(Expression{Func{TEntity, bool}},EntityIncludeQuery{TEntity},CancellationToken)"/>
    /// </summary>
    public virtual async Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate,
        EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
            cancellationToken.ThrowIfCancellationRequested();

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken);

            includeQuery ??= GetEntityIncludeQuery();
            if (includeQuery is null)
            {
                return await context.Set<TEntity>().AsNoTracking().CountAsync(predicate, cancellationToken).ConfigureAwait(false);
            }

            var query = context.Set<TEntity>().AsNoTracking();
            query = includeQuery.GetExpression(query);
            return await query.CountAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity, TId}.ExistsAsync(Expression{Func{TEntity, bool}},EntityIncludeQuery{TEntity},CancellationToken)"/>
    /// </summary>
    public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
            cancellationToken.ThrowIfCancellationRequested();

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken);

            includeQuery ??= GetEntityIncludeQuery();
            if (includeQuery is null)
            {
                return await context.Set<TEntity>().AsNoTracking().AnyAsync(predicate, cancellationToken).ConfigureAwait(false);
            }

            var query = context.Set<TEntity>().AsNoTracking();
            query = includeQuery.GetExpression(query);
            return await query.AnyAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity, TId}.FirstOrDefaultAsync(Expression{Func{TEntity, bool}},EntityIncludeQuery{TEntity},CancellationToken)"/>
    /// </summary>
    public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
            cancellationToken.ThrowIfCancellationRequested();

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken);

            includeQuery ??= GetEntityIncludeQuery();
            if (includeQuery is null)
            {
                return await context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
            }

            var query = context.Set<TEntity>().AsNoTracking();
            query = includeQuery.GetExpression(query);
            return await query.FirstOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity, TId}.SingleOrDefaultAsync(Expression{Func{TEntity, bool}},EntityIncludeQuery{TEntity},CancellationToken)"/>
    /// </summary>
    public virtual async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
            cancellationToken.ThrowIfCancellationRequested();

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken);

            includeQuery ??= GetEntityIncludeQuery();
            if (includeQuery is null)
            {
                return await context.Set<TEntity>().AsNoTracking().SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
            }

            var query = context.Set<TEntity>().AsNoTracking();
            query = includeQuery.GetExpression(query);
            return await query.SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity,TId}.WhereAsync(Expression{Func{TEntity, bool}},EntityIncludeQuery{TEntity},CancellationToken)"/>
    /// </summary>
    public virtual async Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
            cancellationToken.ThrowIfCancellationRequested();

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken);

            includeQuery ??= GetEntityIncludeQuery();
            if (includeQuery is null)
            {
                return await context.Set<TEntity>().AsNoTracking().Where(predicate).ToListAsync(cancellationToken).ConfigureAwait(false);
            }

            var query = context.Set<TEntity>().AsNoTracking();
            query = includeQuery.GetExpression(query);
            return await query.Where(predicate).ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity, TId}.WhereAsync(Expression{Func{TEntity, bool}},int,int,EntityIncludeQuery{TEntity},CancellationToken)"/>
    /// </summary>
    public virtual async Task<PagedResult<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize, EntityIncludeQuery<TEntity>? includeQuery = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
            cancellationToken.ThrowIfCancellationRequested();

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken);

            var query = context.Set<TEntity>().AsNoTracking();
            includeQuery ??= GetEntityIncludeQuery();
            query = includeQuery is null ? query : includeQuery.GetExpression(query);

            var countTask = context.Set<TEntity>().AsNoTracking().CountAsync(predicate, cancellationToken);

            var queryTask = query
                .Where(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var pagedResult = new PagedResult<TEntity>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = await countTask.ConfigureAwait(false),
                Items = await queryTask.ConfigureAwait(false)
            };

            return pagedResult;
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }
    
    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity,TId}.WhereAsync(ISpecification{TEntity},System.Threading.CancellationToken)"/>
    /// </summary>
    public async Task<List<TEntity>> WhereAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(specification, nameof(specification));
            cancellationToken.ThrowIfCancellationRequested();

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken);
            
            var efCoreSpecification = new EfCoreSpecification<TEntity>(specification);

            var query = context.Set<TEntity>().AsNoTracking();
            query = efCoreSpecification.Apply(query);
            
            return await query.ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity, TId}.AsQueryable"/>
    /// </summary>
    public virtual IQueryable<TEntity> AsQueryable()
    {
        try
        {
            return new EfCoreQueryable<TEntity>(new EfCoreQueryProviderWithFactory<TDbContext, TEntity>(DbContextFactory));
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// Performs update operation
    /// </summary>
    /// <param name="context">DbContext</param>
    /// <param name="id">Entity id</param>
    /// <param name="entity">Entity which should be updated</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <returns>Updated entity</returns>
    private async Task<TEntity> PerformUpdateAsync(TDbContext context, TId id, TEntity entity, CancellationToken cancellationToken)
    {
        var existingEntity = await context.Set<TEntity>().FindAsync(new object?[] { id }, cancellationToken).ConfigureAwait(false);
        if (existingEntity is null)
        {
            throw new EntityNotFoundException($"{_entityName} entity with id '{id}' not found");
        }

        context.Entry(existingEntity).CurrentValues.SetValues(entity);

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return entity;
    }

    /// <summary>
    /// Performs update operation
    /// </summary>
    /// <param name="context">DbContext</param>
    /// <param name="entities">List of entities which should be updated</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <returns>Updated entity</returns>
    /// <exception cref="ArgumentException">Thrown when entity does not have any primary key defined</exception>
    private async Task<List<TEntity>> PerformUpdateAsync(TDbContext context, List<TEntity> entities, CancellationToken cancellationToken)
    {
        var idName = GetEntityIdColumnOrThrow(context);

        // TODO: no reflection here if possible
        var idProperty = typeof(TEntity).GetProperty(idName);

        var list = new List<TEntity>();

        // ChunkBy 200 entities. Databases have limitation of performing WHERE IN clause
        var entityIdChunks = entities.Select(x => GetEntityId(x)).Chunk(200).ToList();
        foreach (var entityIdChunk in entityIdChunks)
        {
            var entityDbos = await context.Set<TEntity>()
                .Where(x => entityIdChunk.Contains(EF.Property<TId>(x, idName)))
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            list.AddRange(entityDbos);
        }

        if (list.Count != entities.Count)
        {
            throw new EntityNotFoundException($"Not all {_entityName} entities were found!");
        }

        foreach (var entity in list)
        {
            var entityId = GetEntityId(entity);
            var updatedEntity = entities.Single(x => entityId.Equals(idProperty?.GetValue(x, null)));
            context.Entry(entity).CurrentValues.SetValues(updatedEntity);
        }

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return entities;
    }

    /// <summary>
    /// Performs update operation
    /// </summary>
    /// <param name="context">DbContext</param>
    /// <param name="id">Entity id</param>
    /// <param name="update">Update entity action</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <exception cref="ArgumentException">Thrown when entity does not have any primary key defined</exception>
    /// <returns>Updated entity</returns>
    private static async Task<TEntity> PerformUpdateAsync(TDbContext context, TId id, Action<TEntity> update, CancellationToken cancellationToken)
    {
        var entity = await context.Set<TEntity>().FindAsync(new object?[] { id }, cancellationToken).ConfigureAwait(false);
        if (entity is null)
        {
            throw new EntityNotFoundException($"Entity with id '{id}' not found");
        }

        // Perform update action
        update(entity);

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return entity;
    }

    /// <summary>
    /// Performs delete operation
    /// </summary>
    /// <param name="context">DbContext</param>
    /// <param name="id">Entity id</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <returns>Deleted entity</returns>
    private async Task<bool> PerformDeleteAsync(TDbContext context, TId id, CancellationToken cancellationToken)
    {
        var idName = GetEntityIdColumnOrThrow(context);
        var result = await context.Set<TEntity>().Where(x => id.Equals(EF.Property<TId>(x, idName))).ExecuteDeleteAsync(cancellationToken).ConfigureAwait(false);
        return result == 1;
    }

    /// <summary>
    /// Performs delete operation
    /// </summary>
    /// <param name="context">DbContext</param>
    /// <param name="ids">List of entity ids</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <exception cref="ArgumentException">Thrown when entity does not have any primary key defined</exception>
    /// <returns>Deleted entity</returns>
    private async Task<bool> PerformDeleteAsync(TDbContext context, List<TId> ids, CancellationToken cancellationToken)
    {
        var idName = GetEntityIdColumnOrThrow(context);

        var isAllDeleted = true;

        // ChunkBy 200 entities. Databases have limitation of performing WHERE IN clause
        var entityIdChunks = ids.Chunk(200).ToList();
        foreach (var entityIdChunk in entityIdChunks)
        {
            var result = await context.Set<TEntity>().Where(x => entityIdChunk.Contains(EF.Property<TId>(x, idName))).ExecuteDeleteAsync(cancellationToken).ConfigureAwait(false);
            isAllDeleted &= result == ids.Count;
        }

        return isAllDeleted;
    }

    /// <summary>
    /// Performs delete operation
    /// </summary>
    /// <param name="context">DbContext</param>
    /// <param name="id">Entity id</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <returns>Deleted entity</returns>
    private async Task<TEntity> PerformDeleteAndReturnAsync(TDbContext context, TId id, CancellationToken cancellationToken)
    {
        var entity = await context.Set<TEntity>().FindAsync(new object?[] { id }, cancellationToken).ConfigureAwait(false);
        if (entity is null)
        {
            throw new EntityNotFoundException($"{_entityName} entity with id '{id}' not found");
        }

        context.Set<TEntity>().Remove(entity);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return entity;
    }

    /// <summary>
    /// Returns entity by id
    /// </summary>
    /// <param name="context">DbContext</param>
    /// <param name="id">Entity id</param>
    /// <param name="includeQuery">Expression that describes included entities</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <returns>Entity</returns>
    private async Task<TEntity?> GetEntityByIdImpl(TDbContext context, TId id, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default)
    {
        if (includeQuery is null)
        {
            return await context.Set<TEntity>().FindAsync(new object?[] { id }, cancellationToken).ConfigureAwait(false);
        }

        var query = context.Set<TEntity>().AsQueryable();
        query = includeQuery.GetExpression(query);

        var idName = GetEntityIdColumnOrThrow(context);
        return await query.SingleOrDefaultAsync(x => id.Equals(EF.Property<TId>(x, idName)), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Return entity id column name or throws exception if entity does not have any primary key defined
    /// </summary>
    /// <param name="context">DbContext</param>
    /// <exception cref="ArgumentException">Thrown when entity does not have any primary key defined</exception>
    /// <returns>Entity id column name</returns>
    private string GetEntityIdColumnOrThrow(TDbContext context)
    {
        var idName = context.Model.FindEntityType(typeof(TEntity))?.FindPrimaryKey()?.Properties.SingleOrDefault()?.Name;
        if (idName is null)
        {
            throw new ArgumentException($"{_entityName} entity does not have any primary key defined", _entityName);
        }

        return idName;
    }
}