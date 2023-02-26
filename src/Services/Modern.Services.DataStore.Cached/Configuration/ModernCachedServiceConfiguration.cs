namespace Modern.Services.DataStore.Cached.Configuration;

/// <summary>
/// The modern cached service configuration model
/// </summary>
public class ModernCachedServiceConfiguration
{
    /// <summary>
    /// Indicates whether to add an entity into the cache when entity is created in the data store.<br/>
    /// Default value is <see langword="true"/>
    /// </summary>
    public bool AddToCacheWhenEntityCreated { get; set; } = true;
    
    /// <summary>
    /// Indicates whether to add or update an entity in the cache when entity is updated in the data store.<br/>
    /// Default value is <see langword="true"/>
    /// </summary>
    public bool AddOrUpdateInCacheWhenEntityIsUpdated { get; set; } = true;
}