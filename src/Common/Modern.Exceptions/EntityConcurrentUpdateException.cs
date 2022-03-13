using System.Runtime.Serialization;

namespace Modern.Exceptions;

/// <summary>
/// The exception that is thrown when entity is not updated because of concurrent update issue.<br/>
/// See <see href="https://docs.microsoft.com/en-us/ef/core/saving/concurrency"/> for more information
/// </summary>
[Serializable]
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

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="info">Serialization info</param>
    /// <param name="context">Streaming context</param>
    protected EntityConcurrentUpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}