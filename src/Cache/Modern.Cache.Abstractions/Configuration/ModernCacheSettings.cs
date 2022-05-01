namespace Modern.Cache.Abstractions.Configuration;

/// <summary>
/// The cache settings model
/// </summary>
public class ModernCacheSettings
{
    /// <summary>
    /// Time after which an item is deleted from cache.<br/>
    /// If an item is retrieved from cache before the time expires - the timer is reset.<br/>
    /// Set <see langword="null"/> to set items to never expire.
    /// </summary>
    public TimeSpan? ExpiresIn { get; set; }
}
