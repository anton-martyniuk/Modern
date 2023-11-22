using System.Runtime.Serialization;

namespace Modern.Exceptions;

/// <summary>
/// The exception that is thrown when entity is not updated because of concurrent update issue.<br/>
/// For example: see <see href="https://docs.microsoft.com/en-us/ef/core/saving/concurrency"/> for more information
/// </summary>
public class EntityConcurrentUpdateException : Exception
{
    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    public EntityConcurrentUpdateException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="message">Exception message</param>
    public EntityConcurrentUpdateException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="innerException">Inner exception</param>
    public EntityConcurrentUpdateException(string message, Exception innerException) : base(message, innerException)
    {
    }
}