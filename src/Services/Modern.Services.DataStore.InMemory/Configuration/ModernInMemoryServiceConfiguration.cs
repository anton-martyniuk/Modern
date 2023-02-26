namespace Modern.Services.DataStore.InMemory.Configuration;

/// <summary>
/// The modern in-memory service configuration model
/// </summary>
public class ModernInMemoryServiceConfiguration
{
    /// <summary>
    /// Indicates whether to add an entity into the in-memory cache when entity is created in the datastore.<br/>
    /// Default value is <see langword="true"/>
    /// </summary>
    public bool AddToCacheWhenEntityCreated { get; set; } = true;

    /// <summary>
    /// Indicates whether to add or update an entity in the in-memory cache when entity is updated in the data store.<br/>
    /// Default value is <see langword="true"/>
    /// </summary>
    public bool AddOrUpdateInCacheWhenEntityIsUpdated { get; set; } = true;
}