using System.Data;
using System.Linq.Expressions;
using System.Text;
using Ardalis.GuardClauses;
using Dapper;
using Modern.Data.Paging;
using Modern.Exceptions;
using Modern.Repositories.Abstractions;
using Modern.Repositories.Abstractions.Infrastracture;
using Modern.Repositories.Dapper.Adapters;
using Modern.Repositories.Dapper.Extensions;
using Modern.Repositories.Dapper.Mapping;
using static Dapper.SqlMapper;

namespace Modern.Repositories.Dapper;

/// <summary>
/// Represents an <see cref="IModernCrudRepository{TEntity, TId}"/> and <see cref="IModernQueryRepository{TEntity, TId}"/> implementation using Dapper
/// </summary>
/// <typeparam name="TEntityMapping">The type of entity mapping</typeparam>
/// <typeparam name="TEntity">The type of entity</typeparam>
/// <typeparam name="TId">The type of entity identifier</typeparam>
public class ModernDapperRepository<TEntityMapping, TEntity, TId> : IModernRepository<TEntity, TId>
    where TEntityMapping : DapperEntityMapping<TEntity>
    where TEntity : class
    where TId : IEquatable<TId>
{
    private readonly string _entityName = typeof(TEntity).Name;

    /// <summary>
    /// The database connection
    /// </summary>
    public IDbConnection DbConnection { get; }

    /// <summary>
    /// Dapper entity mapping
    /// </summary>
    public TEntityMapping Mapping { get; }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="dbConnection">The database connection</param>
    /// <param name="mapping">The Dapper entity mapping</param>
    public ModernDapperRepository(IDbConnection dbConnection, TEntityMapping mapping)
    {
        DbConnection = dbConnection;
        Mapping = mapping;
        Mapping.Build();
    }

    /// <summary>
    /// Returns entity id of type <typeparamref name="TId"/>
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>Entity id</returns>
    protected virtual TId GetEntityId(TEntity entity) => (TId)(entity.GetType().GetProperty(Mapping.IdColumn.Value)?.GetValue(entity, null) ?? 0);

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

            var insertQuery = GetInsertQuery();
            var createdEntity = await DbConnection.QueryFirstWithTokenAsync<TEntity>(insertQuery, entity, cancellationToken: cancellationToken).ConfigureAwait(false);
            return createdEntity;
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

            var createdEntities = new List<TEntity>(entities.Count);
            foreach (var entity in entities)
            {
                var insertQuery = GetInsertQuery();
                var createdEntity = await DbConnection.QueryFirstWithTokenAsync<TEntity>(insertQuery, entity, cancellationToken: cancellationToken).ConfigureAwait(false);
                createdEntities.Add(createdEntity);
            }

            return createdEntities;
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

            var updateQuery = GetUpdateQuery();
            var result = await DbConnection.ExecuteWithTokenAsync(updateQuery, entity, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (result != 1)
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

            foreach (var entity in entities)
            {
                var updateQuery = GetUpdateQuery();
                var result = await DbConnection.ExecuteWithTokenAsync(updateQuery, entity, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (result != 1)
                {
                    throw new EntityNotFoundException($"{_entityName} entity with id '{GetEntityId(entity)}' not found");
                }
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

            var whereQuery = GetWhereByIdQuery();
            var columnsQuery = GetSelectColumnsQuery();

            var entity = await DbConnection.QueryFirstOrDefaultWithTokenAsync<TEntity>($"SELECT {columnsQuery} FROM {Mapping.TableName} {whereQuery}", new { Id = id }, cancellationToken: cancellationToken);
            if (entity is null)
            {
                throw new EntityNotFoundException($"{_entityName} entity with id '{id}' not found");
            }

            // Perform update action
            update(entity);

            var updateQuery = GetUpdateQuery();
            var result = await DbConnection.ExecuteWithTokenAsync(updateQuery, entity, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (result != 1)
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

            var whereQuery = GetWhereByIdQuery();
            var result = await DbConnection.ExecuteWithTokenAsync($"DELETE FROM {Mapping.TableName} {whereQuery}", new { Id = id }, cancellationToken: cancellationToken);
            return result == 1;
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

            var isAllDeleted = true;

            // ChunkBy 200 entities. Databases have limitation of performing WHERE IN clause
            var entityIdChunks = ids.Chunk(200).ToList();
            foreach (var entityIdChunk in entityIdChunks)
            {
                var result = await DbConnection.ExecuteWithTokenAsync($"DELETE FROM {Mapping.TableName} WHERE {Mapping.IdColumnName} IN ({string.Join(",", entityIdChunk)})", cancellationToken: cancellationToken);
                isAllDeleted &= result == ids.Count;
            }

            return isAllDeleted;
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

            var whereQuery = GetWhereByIdQuery();
            var columnsQuery = GetSelectColumnsQuery();

            var entity = await DbConnection.QueryFirstOrDefaultWithTokenAsync<TEntity>($"SELECT {columnsQuery} FROM {Mapping.TableName} {whereQuery}", new { Id = id }, cancellationToken: cancellationToken);
            if (entity is null)
            {
                throw new EntityNotFoundException($"{_entityName} entity with id '{id}' not found");
            }

            await DbConnection.ExecuteWithTokenAsync($"DELETE FROM {Mapping.TableName} {whereQuery}", new { Id = id }, cancellationToken: cancellationToken);
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

            var whereQuery = GetWhereByIdQuery();
            var columnsQuery = GetSelectColumnsQuery();

            var entity = await DbConnection.QueryFirstOrDefaultWithTokenAsync<TEntity>($"SELECT {columnsQuery} FROM {Mapping.TableName} {whereQuery}", new { Id = id }, cancellationToken: cancellationToken);
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

            var whereQuery = GetWhereByIdQuery();
            var columnsQuery = GetSelectColumnsQuery();

            var entity = await DbConnection.QueryFirstOrDefaultWithTokenAsync<TEntity>($"SELECT {columnsQuery} FROM {Mapping.TableName} {whereQuery}", new { Id = id }, cancellationToken: cancellationToken);
            return entity;
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

            var columnsQuery = GetSelectColumnsQuery();
            var enumerable = await DbConnection.QueryAsync<TEntity>($"SELECT {columnsQuery} FROM {Mapping.TableName}");
            return enumerable.ToList();
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
            return await DbConnection.QueryFirstAsync<int>($"SELECT COUNT(*) FROM {Mapping.TableName}");
        }
        catch (Exception ex)
        {
            throw CreateProperException(ex);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity, TId}.CountAsync(Expression{Func{TEntity, bool}},EntityIncludeQuery{TEntity},CancellationToken)"/>
    /// </summary>
    public virtual Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Current operation is not supported by ModernDapperRepository");
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity, TId}.ExistsAsync(Expression{Func{TEntity, bool}},EntityIncludeQuery{TEntity},CancellationToken)"/>
    /// </summary>
    public virtual Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Current operation is not supported by ModernDapperRepository");
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity, TId}.FirstOrDefaultAsync(Expression{Func{TEntity, bool}},EntityIncludeQuery{TEntity},CancellationToken)"/>
    /// </summary>
    public virtual Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Current operation is not supported by ModernDapperRepository");
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity, TId}.SingleOrDefaultAsync(Expression{Func{TEntity, bool}},EntityIncludeQuery{TEntity},CancellationToken)"/>
    /// </summary>
    public virtual Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Current operation is not supported by ModernDapperRepository");
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity,TId}.WhereAsync(Expression{Func{TEntity, bool}},EntityIncludeQuery{TEntity},CancellationToken)"/>
    /// </summary>
    public virtual Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, EntityIncludeQuery<TEntity>? includeQuery = null, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Current operation is not supported by ModernDapperRepository");
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity, TId}.WhereAsync(Expression{Func{TEntity, bool}},int,int,EntityIncludeQuery{TEntity},CancellationToken)"/>
    /// </summary>
    public virtual Task<PagedResult<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize, EntityIncludeQuery<TEntity>? includeQuery = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Current operation is not supported by ModernDapperRepository");
    }

    /// <summary>
    /// <inheritdoc cref="IModernQueryRepository{TEntity, TId}.AsQueryable"/>
    /// </summary>
    public virtual IQueryable<TEntity> AsQueryable()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns part of SQL query which performs WHERE clause on id column
    /// </summary>
    /// <returns>Where query</returns>
    private string GetWhereByIdQuery()
        => $"WHERE {Mapping.IdColumn.Key}=@{Mapping.IdColumn.Value}";

    /// <summary>
    /// Returns SQL query that selects all columns of the entity table
    /// </summary>
    /// <returns>Select query</returns>
    private string GetSelectColumnsQuery()
        => Mapping.ColumnMappingsWithId.Aggregate(new StringBuilder(), (builder, mapping) => builder.AppendFormat("{0}\"{1}\" as {2}",
            builder.Length > 0 ? ", " : "", mapping.Key, mapping.Value), builder => builder.ToString());

    /// <summary>
    /// Returns an insert SQL query for a single entity
    /// </summary>
    /// <returns>Insert SQL query</returns>
    private string GetInsertQuery()
    {
        // TODO: different providers
        return new PostgresSqlQueryProvider().GetInsertWithOutputCommand(Mapping);
    }
    
    /// <summary>
    /// Returns update SQL query for a single entity
    /// </summary>
    /// <returns>Insert sql query</returns>
    private string GetUpdateQuery()
    {
        var columns = Mapping.ColumnMappings.Aggregate(new StringBuilder(), (builder, mapping) => builder.AppendFormat("{0}\"{1}\"=@{2}",
            builder.Length > 0 ? ", " : "", mapping.Key, mapping.Value), builder => builder.ToString());

        var queryBuilder = new StringBuilder();
        queryBuilder.AppendFormat("update {0} SET {1}", Mapping.TableName, columns);
        queryBuilder.AppendFormat(" WHERE {0}=@{1}", Mapping.IdColumn.Key, Mapping.IdColumn.Value);

        return queryBuilder.ToString();
    }

    /// <summary>
    /// Returns insert query for multiple entities
    /// </summary>
    /// <returns>Insert sql query</returns>
    private string GetInsertManyQuery(int count)
    {
        var columns = Mapping.ColumnMappings.Aggregate(new StringBuilder(), (builder, mapping) => builder.AppendFormat("{0}\"{1}\"",
            builder.Length > 0 ? ", " : "", mapping.Key), builder => builder.ToString());

        //var values = Mapping.ColumnMappings.Aggregate(new StringBuilder(), (builder, mapping) => builder.AppendFormat("{0}@{1}",
        //    builder.Length > 0 ? ", " : "", mapping.Value), builder => builder.ToString());

        var valueBuilder = new StringBuilder();

        for (var i = 0; i < count; i++)
        {
            valueBuilder.Append(valueBuilder.Length > 0 ? ", " : "");

            var values = Mapping.ColumnMappings.Aggregate(new StringBuilder(), (builder, mapping) => builder.AppendFormat("{0}@{1}{2}",
                builder.Length > 0 ? ", " : "", mapping.Value, i), builder => builder.ToString());

            valueBuilder.AppendFormat("({0})", values);
        }

        var outputString = Mapping.ColumnMappingsWithId.Aggregate(new StringBuilder(), (builder, mapping) => builder.AppendFormat("{0}\"{1}\" as {2}",
            builder.Length > 0 ? ", " : "", mapping.Key, mapping.Value), builder => builder.ToString());

        var queryBuilder = new StringBuilder();
        queryBuilder.AppendFormat("insert into {0} ({1}) values ({2})", Mapping.TableName, columns, valueBuilder);
        queryBuilder.AppendFormat(" RETURNING {0}", outputString);

        return queryBuilder.ToString();
    }
}