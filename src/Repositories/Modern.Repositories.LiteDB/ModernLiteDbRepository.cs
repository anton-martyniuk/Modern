﻿using System.Linq.Expressions;
using Ardalis.GuardClauses;
using LiteDB;
using Modern.Data.Paging;
using Modern.Exceptions;
using Modern.Repositories.Abstractions;
using Modern.Repositories.Abstractions.Infrastracture;
using Modern.Repositories.Abstractions.Specifications;
using Modern.Repositories.LiteDB.Specifications;

namespace Modern.Repositories.LiteDB;

/// <summary>
/// Represents an <see cref="IModernCrudRepository{TEntity,TId}"/> and <see cref="IModernQueryRepository{TEntity, TId}"/> implementation using NO SQL LiteDB
/// </summary>
/// <typeparam name="TEntity">The type of entity</typeparam>
/// <typeparam name="TId">The type of entity identifier</typeparam>
public class ModernLiteDbRepository<TEntity, TId> : IModernRepository<TEntity, TId>
    where TEntity : class
    where TId : IEquatable<TId>
{
    private readonly string _entityName = typeof(TEntity).Name;
    private readonly string _connectionString;
    private readonly string _collectionName;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="connectionString">Connection string to LiteDB</param>
    /// <param name="collectionName">Name of the collection</param>
    public ModernLiteDbRepository(string connectionString, string collectionName)
    {
        _connectionString = connectionString;
        _collectionName = collectionName;
    }

    /// <summary>
    /// Returns standardized repository exception
    /// </summary>
    /// <param name="ex">Original exception</param>
    /// <returns>Repository exception which holds original exception as InnerException</returns>
    protected virtual Exception CreateProperException(Exception ex)
        => ex switch
        {
            ArgumentException _ => ex,
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

            using var db = new LiteDatabase(_connectionString);
            var collection = db.GetCollection<TEntity>(_collectionName);
            collection.Insert(entity);

            await Task.CompletedTask;
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

            using var db = new LiteDatabase(_connectionString);
            var collection = db.GetCollection<TEntity>(_collectionName);
            collection.Insert(entities);

            await Task.CompletedTask;
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

            using var db = new LiteDatabase(_connectionString);
            var collection = db.GetCollection<TEntity>(_collectionName);

            var result = collection.Update(new BsonValue(id), entity);
            if (!result)
            {
                throw new EntityNotFoundException($"{_entityName} entity with id '{id}' not found");
            }

            await Task.CompletedTask;
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

            using var db = new LiteDatabase(_connectionString);
            var collection = db.GetCollection<TEntity>(_collectionName);

            var result = collection.Update(entities);
            if (result != entities.Count)
            {
                throw new EntityNotFoundException($"Not all {_entityName} entities were found!");
            }

            await Task.CompletedTask;
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

            using var db = new LiteDatabase(_connectionString);
            var collection = db.GetCollection<TEntity>(_collectionName);

            var entity = collection.FindById(new BsonValue(id));
            if (entity is null)
            {
                throw new EntityNotFoundException($"{_entityName} entity with id '{id}' not found");
            }

            // Perform update action
            update(entity);

            var result = collection.Update(entity);
            if (!result)
            {
                throw new EntityNotFoundException($"{_entityName} entity with id '{id}' not found");
            }

            await Task.CompletedTask;
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

            using var db = new LiteDatabase(_connectionString);
            var collection = db.GetCollection<TEntity>(_collectionName);

            await Task.CompletedTask;
            return collection.Delete(new BsonValue(id));
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

            using var db = new LiteDatabase(_connectionString);
            var collection = db.GetCollection<TEntity>(_collectionName);

            await Task.CompletedTask;
            return ids.Aggregate(true, (current, id) => current && collection.Delete(new BsonValue(id)));
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

            using var db = new LiteDatabase(_connectionString);
            var collection = db.GetCollection<TEntity>(_collectionName);

            var entity = collection.FindById(new BsonValue(id));
            if (entity is null)
            {
                throw new EntityNotFoundException($"{_entityName} entity with id '{id}' not found");
            }

            var result = collection.Delete(new BsonValue(id));
            if (!result)
            {
                throw new EntityNotFoundException($"{_entityName} entity with id '{id}' not found");
            }

            await Task.CompletedTask;
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

            using var db = new LiteDatabase(_connectionString);
            var collection = db.GetCollection<TEntity>(_collectionName);

            var entity = collection.FindById(new BsonValue(id));
            if (entity is null)
            {
                throw new EntityNotFoundException($"{_entityName} entity with id '{id}' not found");
            }

            await Task.CompletedTask;
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

            using var db = new LiteDatabase(_connectionString);
            var collection = db.GetCollection<TEntity>(_collectionName);

            await Task.CompletedTask;
            return collection.FindById(new BsonValue(id));
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

            using var db = new LiteDatabase(_connectionString);
            var collection = db.GetCollection<TEntity>(_collectionName);

            await Task.CompletedTask;
            return collection.FindAll().ToList();
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

            using var db = new LiteDatabase(_connectionString);
            var collection = db.GetCollection<TEntity>(_collectionName);

            await Task.CompletedTask;
            return collection.LongCount();
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

            using var db = new LiteDatabase(_connectionString);
            var collection = db.GetCollection<TEntity>(_collectionName);

            await Task.CompletedTask;
            return collection.Count(predicate);
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

            using var db = new LiteDatabase(_connectionString);
            var collection = db.GetCollection<TEntity>(_collectionName);

            await Task.CompletedTask;
            return collection.Exists(predicate);
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

            using var db = new LiteDatabase(_connectionString);
            var collection = db.GetCollection<TEntity>(_collectionName);

            await Task.CompletedTask;
            return collection.FindOne(predicate);
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

            using var db = new LiteDatabase(_connectionString);
            var collection = db.GetCollection<TEntity>(_collectionName);

            await Task.CompletedTask;
            return collection.FindOne(predicate);
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

            using var db = new LiteDatabase(_connectionString);
            var collection = db.GetCollection<TEntity>(_collectionName);

            await Task.CompletedTask;
            return collection.Query().Where(predicate).ToList();
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

            using var db = new LiteDatabase(_connectionString);
            var collection = db.GetCollection<TEntity>(_collectionName);

            var count = collection.Count(predicate);

            var items = collection.Query()
                .Where(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToList();

            await Task.CompletedTask;

            return new PagedResult<TEntity>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = count,
                Items = items
            };
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
            
            using var db = new LiteDatabase(_connectionString);
            var collection = db.GetCollection<TEntity>(_collectionName);

            var liteDbSpecification = new LiteDbSpecification<TEntity>(specification);

            var query = collection.Query();
            query = liteDbSpecification.Apply(query);
            
            await Task.CompletedTask;
            
            return query.ToList();
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
        throw new NotSupportedException("Current operation is not supported by ModernLiteDbRepository");
    }
}
