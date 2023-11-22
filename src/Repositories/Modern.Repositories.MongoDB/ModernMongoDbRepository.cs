using Ardalis.GuardClauses;
using Modern.Data.Paging;
using Modern.Exceptions;
using System.Linq.Expressions;
using Modern.Repositories.Abstractions;
using Modern.Repositories.Abstractions.Infrastracture;
using Modern.Repositories.Abstractions.Specifications;
using Modern.Repositories.MongoDB.Specifications;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Bson;

namespace Modern.Repositories.MongoDB;

/// <summary>
/// Represents an <see cref="IModernCrudRepository{TEntity,TId}"/> and <see cref="IModernQueryRepository{TEntity, TId}"/> implementation using NO SQL MongoDb
/// </summary>
/// <typeparam name="TEntity">The type of entity</typeparam>
/// <typeparam name="TId">The type of entity identifier</typeparam>
public class ModernMongoDbRepository<TEntity, TId> : IModernRepository<TEntity, TId>
    where TEntity : class
    where TId : IEquatable<TId>
{
    private readonly string _entityName = typeof(TEntity).Name;

    /// <summary>
    /// The MongoDB collection of type <typeparamref name="TEntity"/>
    /// </summary>
    protected IMongoCollection<TEntity> MongoCollection { get; set; }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="mongoClient">MongoDb client</param>
    /// <param name="databaseName">Name of the database</param>
    /// <param name="collectionName">Name of the collection</param>
    public ModernMongoDbRepository(IMongoClient mongoClient, string databaseName, string collectionName)
    {
        var database = mongoClient.GetDatabase(databaseName);
        MongoCollection = database.GetCollection<TEntity>(collectionName);
    }

    /// <summary>
    /// Returns entity id of type <typeparamref name="TId"/>
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>Entity id</returns>
    protected virtual TId GetEntityId(TEntity entity) => (TId)(entity.GetType().GetProperty("Id")?.GetValue(entity, null) ?? 0);

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

            await MongoCollection.InsertOneAsync(entity, cancellationToken: cancellationToken).ConfigureAwait(false);
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

            await MongoCollection.InsertManyAsync(entities, cancellationToken: cancellationToken).ConfigureAwait(false);
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

            var replaceOptions = new ReplaceOptions
            {
                IsUpsert = false
            };

            var result = await MongoCollection.ReplaceOneAsync(GetIdFilter(id), entity, replaceOptions, cancellationToken).ConfigureAwait(false);
            if (result.MatchedCount != 1)
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
    /// <inheritdoc cref="IModernCrudRepository{TEntity, TId}.UpdateAsync(List{TEntity},CancellationToken)"/>
    /// </summary>
    public virtual async Task<List<TEntity>> UpdateAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entities, nameof(entities));
            Guard.Against.NegativeOrZero(entities.Count, nameof(entities));
            cancellationToken.ThrowIfCancellationRequested();

            var updates = new List<WriteModel<TEntity>>();

            foreach (var entity in entities)
            {
                var id = GetEntityId(entity);
                updates.Add(new ReplaceOneModel<TEntity>(GetIdFilter(id), entity));
            }

            var bulkOptions = new BulkWriteOptions { IsOrdered = false };
            var result = await MongoCollection.BulkWriteAsync(updates, bulkOptions, cancellationToken).ConfigureAwait(false);
            if (result.ModifiedCount != entities.Count)
            {
                throw new EntityNotFoundException($"Not all {_entityName} entities were found!");
            }

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

            var idFilter = GetIdFilter(id);

            var entity = await MongoCollection.Find(GetIdFilter(id)).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            if (entity is null)
            {
                throw new EntityNotFoundException($"{_entityName} entity with id '{id}' not found");
            }

            // Perform update action
            update(entity);

            var replaceOptions = new ReplaceOptions
            {
                IsUpsert = false
            };

            var result = await MongoCollection.ReplaceOneAsync(idFilter, entity, replaceOptions, cancellationToken).ConfigureAwait(false);
            if (result.MatchedCount != 1)
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
    /// <inheritdoc cref="IModernCrudRepository{TEntity, TId}.DeleteAsync(TId,CancellationToken)"/>
    /// </summary>
    public virtual async Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            cancellationToken.ThrowIfCancellationRequested();

            var result = await MongoCollection.DeleteOneAsync(GetIdFilter(id), cancellationToken).ConfigureAwait(false);
            return result.DeletedCount == 1;
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

            var result = await MongoCollection.DeleteManyAsync(GetIdsFilter(ids), cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.DeletedCount == ids.Count;
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

            var entity = await MongoCollection.FindOneAndDeleteAsync(GetIdFilter(id), cancellationToken: cancellationToken).ConfigureAwait(false);
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
    /// <inheritdoc cref="IModernQueryRepository{TEntity, TId}.GetByIdAsync"/>
    /// </summary>
    public virtual async Task<TEntity> GetByIdAsync(TId id, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            cancellationToken.ThrowIfCancellationRequested();

            var entity = await MongoCollection.Find(GetIdFilter(id)).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
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

            return await MongoCollection.Find(GetIdFilter(id)).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
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

            return await MongoCollection.AsQueryable().ToListAsync(cancellationToken).ConfigureAwait(false);
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

            return await MongoCollection.CountDocumentsAsync(FilterDefinition<TEntity>.Empty, cancellationToken: cancellationToken);
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

            return await MongoCollection.AsQueryable().LongCountAsync(predicate, cancellationToken).ConfigureAwait(false);
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

            return await MongoCollection.AsQueryable().AnyAsync(predicate, cancellationToken).ConfigureAwait(false);
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

            return await MongoCollection.AsQueryable().FirstOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
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

            return await MongoCollection.AsQueryable().SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
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

            return await MongoCollection.AsQueryable().Where(predicate).ToListAsync(cancellationToken).ConfigureAwait(false);
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

            var countTask = MongoCollection.AsQueryable().CountAsync(predicate, cancellationToken);

            var queryTask = MongoCollection.AsQueryable()
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
            
            var mongoDbSpecification = new MongoDbSpecification<TEntity>(specification);

            var query = MongoCollection.AsQueryable();
            query = mongoDbSpecification.Apply(query);
            
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
            return MongoCollection.AsQueryable();
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// Returns modern collection for internal usage inside MongoDb libraries
    /// </summary>
    public virtual IMongoCollection<TEntity> GetMongoCollection() => MongoCollection;

    /// <summary>
    /// Returns filter by "_id" field
    /// </summary>
    /// <param name="id">Id to search for</param>
    /// <returns>Filter by "_id"</returns>
    private static FilterDefinition<TEntity> GetIdFilter(TId id) => Builders<TEntity>.Filter.Eq("_id", ObjectId.Parse(id.ToString()));

    /// <summary>
    /// Returns filter by list of "_id" fields
    /// </summary>
    /// <param name="ids">List of ids to search for</param>
    /// <returns>Filter by list of "_id"s</returns>
    private static FilterDefinition<TEntity> GetIdsFilter(IEnumerable<TId> ids) => Builders<TEntity>.Filter.In("_id", ids.Select(x => ObjectId.Parse(x.ToString())));
}
