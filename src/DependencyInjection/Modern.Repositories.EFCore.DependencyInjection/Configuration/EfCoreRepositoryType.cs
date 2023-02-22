namespace Modern.Repositories.EFCore.DependencyInjection.Configuration;

/// <summary>
/// The ef core repository type enum
/// </summary>
internal enum EfCoreRepositoryType
{
    /// <summary>
    /// When using DbContext repository shares the same database connection during its lifetime
    /// </summary>
    DbContext,
    
    /// <summary>
    /// When using DbContextFactory every repository creates and closes a database connection in each method.
    /// </summary>
    DbContextFactory,
    
    /// <summary>
    /// When using UnitOfWork repository shares the same database connection during its lifetime.<br/>
    /// But SaveChanges is not called in the Create, Update and Delete methods.<br/>
    /// SaveChanges is called in the UnitOfWork.<br/>
    /// Also a transaction is not created in the repository but UnitOfWork has methods to create and manipulate the transaction.
    /// </summary>
    UnitOfWork
}