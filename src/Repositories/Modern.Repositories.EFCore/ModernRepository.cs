using Microsoft.EntityFrameworkCore;
using Modern.Repositories.EFCore.Configuration;
using System.Data;
using System.Linq.Expressions;
using Ardalis.GuardClauses;
using Modern.Exceptions;
using Modern.Repositories.Abstractions;
using Modern.Repositories.Abstractions.Exceptions;
using Modern.Repositories.EFCore.Extensions;
using Modern.Repositories.EFCore.Query;

namespace Modern.Repositories.EFCore;

/// <summary>
/// Represents an <see cref="IModernCrudRepository{TEntity,TId}"/> and <see cref="IModernQueryRepository{TEntity,TId}"/> implementation using EFCore
/// </summary>
/// <typeparam name="TDbContext">The type of EF Core DbContext</typeparam>
/// <typeparam name="TEntity">The type of entity</typeparam>
/// <typeparam name="TId">The type of entity identifier</typeparam>
public class ModernRepository<TDbContext, TEntity, TId> : IModernCrudRepository<TEntity, TId>, IModernQueryRepository<TEntity, TId>
    where TDbContext : DbContext
    where TEntity : class
    where TId : IEquatable<TId>
{
    private readonly EfCoreRepositoryConfiguration? _configuration;

    /// <summary>
    /// The <typeparamref name="TDbContext"/> factory
    /// </summary>
    protected IDbContextFactory<TDbContext> DbContextFactory { get; }

    ///// <summary>
    ///// Initializes a new instance of the class
    ///// </summary>
    ///// <param name="createDbContext">The DbContext creation delegate</param>
    ///// <param name="configuration">Repository configuration</param>
    //public ModernEfCoreRepository(Func<TDbContext> createDbContext, EfCoreRepositoryConfiguration? configuration = null)
    //    : base(createDbContext)
    //{
    //    _configuration = configuration;
    //}

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="dbContextFactory">The <see cref="IDbContextFactory{TDbContext}"/> implementation</param>
    /// <param name="configuration">Repository configuration</param>
    public ModernRepository(IDbContextFactory<TDbContext> dbContextFactory, EfCoreRepositoryConfiguration? configuration = null)
    {
        // TODO: use IOptions or Snapshot
        DbContextFactory = dbContextFactory;
        _configuration = configuration;
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity,TId}.CreateAsync(TEntity,CancellationToken)"/>
    /// </summary>
    public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));

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
    /// <inheritdoc cref="IModernCrudRepository{TEntity,TId}.CreateAsync(List{TEntity},CancellationToken)"/>
    /// </summary>
    public virtual async Task<List<TEntity>> CreateAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entities, nameof(entities));
            Guard.Against.NegativeOrZero(entities.Count, nameof(entities));

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
    /// <inheritdoc cref="IModernCrudRepository{TEntity,TId}.UpdateAsync(TId,TEntity,CancellationToken)"/>
    /// </summary>
    public virtual async Task<TEntity> UpdateAsync(TId id, TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));

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
    /// <inheritdoc cref="IModernCrudRepository{TEntity,TId}.UpdateAsync(List{TEntity},CancellationToken)"/>
    /// </summary>
    public virtual async Task<List<TEntity>> UpdateAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entities, nameof(entities));
            Guard.Against.NegativeOrZero(entities.Count, nameof(entities));

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
    /// <inheritdoc cref="IModernCrudRepository{TEntity,TId}.UpdateAsync(TId, Action{TEntity}, CancellationToken)"/>
    /// </summary>
    public virtual async Task<TEntity> UpdateAsync(TId id, Action<TEntity> update, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));

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
    /// <inheritdoc cref="IModernCrudRepository{TEntity,TId}.DeleteAsync(TId,CancellationToken)"/>
    /// </summary>
    public virtual async Task DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            if (_configuration?.DeleteConfiguration?.NeedExecuteInTransaction != true)
            {
                await PerformDeleteAsync(context, id, cancellationToken).ConfigureAwait(false);
                return;
            }

            var isolationLevel = _configuration?.DeleteConfiguration?.TransactionIsolationLevel ?? IsolationLevel.Unspecified;
            await using var transaction = await context.Database.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);

            await PerformDeleteAsync(context, id, cancellationToken).ConfigureAwait(false);
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernCrudRepository{TEntity,TId}.DeleteAsync(List{TId},CancellationToken)"/>
    /// </summary>
    public virtual async Task DeleteAsync(List<TId> ids, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(ids, nameof(ids));
            Guard.Against.NegativeOrZero(ids.Count, nameof(ids));

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            if (_configuration?.DeleteConfiguration?.NeedExecuteInTransaction != true)
            {
                await PerformDeleteAsync(context, ids, cancellationToken).ConfigureAwait(false);
                return;
            }

            var isolationLevel = _configuration?.DeleteConfiguration?.TransactionIsolationLevel ?? IsolationLevel.Unspecified;
            await using var transaction = await context.Database.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);

            await PerformDeleteAsync(context, ids, cancellationToken).ConfigureAwait(false);
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <inheritdoc cref="IModernCrudRepository{TEntity,TId}.DeleteAndReturnAsync"/>
    public virtual async Task<TEntity> DeleteAndReturnAsync(TId id, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));

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
    /// <inheritdoc cref="IModernQueryRepository{TEntity,TId}.GetByIdAsync"/>
    /// </summary>
    public virtual async Task<TEntity> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            var entity = await context.Set<TEntity>().FindAsync(new object?[] { id }, cancellationToken).ConfigureAwait(false);
            if (entity is null)
            {
                throw new EntityNotFoundException($"Entity with id '{id}' not found");
            }

            return entity;
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity,TId}.TryGetByIdAsync"/>
    /// </summary>
    public virtual async Task<TEntity?> TryGetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Set<TEntity>().FindAsync(new object?[] { id }, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity,TId}.GetAllAsync"/>
    /// </summary>
    public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            return await context.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity,TId}.CountAsync(CancellationToken)"/>
    /// </summary>
    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Set<TEntity>().AsNoTracking().CountAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity,TId}.CountAsync(Expression{Func{TEntity, bool}},CancellationToken)"/>
    /// </summary>
    public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Set<TEntity>().AsNoTracking().CountAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity,TId}.ExistsAsync(Expression{Func{TEntity, bool}},CancellationToken)"/>
    /// </summary>
    public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Set<TEntity>().AsNoTracking().AnyAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity,TId}.FirstOrDefaultAsync(Expression{Func{TEntity, bool}},CancellationToken)"/>
    /// </summary>
    public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity,TId}.SingleOrDefaultAsync(Expression{Func{TEntity, bool}},CancellationToken)"/>
    /// </summary>
    public virtual async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Set<TEntity>().AsNoTracking().SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity,TId}.WhereAsync"/>
    /// </summary>
    public virtual async Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Set<TEntity>().AsNoTracking().Where(predicate).ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity,TId}.AsQueryable"/>
    /// </summary>
    public virtual IQueryable<TEntity> AsQueryable()
    {
        try
        {
            return new EfCoreQueryable<TEntity>(new EfCoreQueryProvider<TDbContext, TEntity>(DbContextFactory));
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// Returns standardized repository exception
    /// </summary>
    /// <param name="ex">Original exception</param>
    /// <returns>Repository exception which holds original exception as InnerException</returns>
    protected virtual Exception CreateProperException(Exception ex)
    {
        return ex switch
        {
            ArgumentException _ => ex,
            DbUpdateConcurrencyException _ => new EntityConcurrentUpdateException(ex.Message, ex),
            EntityAlreadyExistsException _ => ex,
            EntityNotFoundException _ => ex,
            EntityNotModifiedException _ => ex,
            _ => new RepositoryErrorException(ex.Message, ex)
        };
    }

    /// <summary>
    /// Performs update operation
    /// </summary>
    /// <param name="context">DbContext</param>
    /// <param name="id">Entity id</param>
    /// <param name="entity">Entity which should be updated</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <returns>Updated entity</returns>
    private static async Task<TEntity> PerformUpdateAsync(TDbContext context, TId id, TEntity entity, CancellationToken cancellationToken)
    {
        var existingEntity = await context.Set<TEntity>().FindAsync(new object?[] { id }, cancellationToken).ConfigureAwait(false);
        if (existingEntity is null)
        {
            throw new EntityNotFoundException($"Entity with id '{id}' not found");
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
    private static async Task<List<TEntity>> PerformUpdateAsync(TDbContext context, List<TEntity> entities, CancellationToken cancellationToken)
    {
        var entityType = context.Set<TEntity>().EntityType;
        var keyName = entityType.FindPrimaryKey()?.Properties.Select(x => x.Name).SingleOrDefault();
        if (keyName is null)
        {
            throw new ArgumentException("Entity does not have any primary key defined", nameof(entityType));
        }

        foreach (var entity in entities)
        {
            var id = entity.GetType().GetProperty(keyName)?.GetValue(entity, null);
            await context.Set<TEntity>().WhereIdEquals(entityType, id).UpdateFromQueryAsync(_ => entity, cancellationToken).ConfigureAwait(false);
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
    private static async Task PerformDeleteAsync(TDbContext context, TId id, CancellationToken cancellationToken)
    {
        var entityType = context.Set<TEntity>().EntityType;
        await context.Set<TEntity>().WhereIdEquals(entityType, id).DeleteFromQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Performs delete operation
    /// </summary>
    /// <param name="context">DbContext</param>
    /// <param name="ids">List of entity ids</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <exception cref="ArgumentException">Thrown when entity does not have any primary key defined</exception>
    /// <returns>Deleted entity</returns>
    private static async Task PerformDeleteAsync(TDbContext context, List<TId> ids, CancellationToken cancellationToken)
    {
        var entityType = context.Set<TEntity>().EntityType;
        await context.Set<TEntity>().WhereIdEquals(entityType, ids).DeleteFromQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Performs delete operation
    /// </summary>
    /// <param name="context">DbContext</param>
    /// <param name="id">Entity id</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete</param>
    /// <returns>Deleted entity</returns>
    private static async Task<TEntity> PerformDeleteAndReturnAsync(TDbContext context, TId id, CancellationToken cancellationToken)
    {
        var entity = await context.Set<TEntity>().FindAsync(new object?[] { id }, cancellationToken).ConfigureAwait(false);
        if (entity is null)
        {
            throw new EntityNotFoundException($"Entity with id '{id}' not found");
        }

        context.Set<TEntity>().Remove(entity);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return entity;
    }
}