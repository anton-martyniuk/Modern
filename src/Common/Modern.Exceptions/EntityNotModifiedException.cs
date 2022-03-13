using System.Runtime.Serialization;

namespace Modern.Exceptions;

/// <summary>
/// The exception that is thrown when entity is not modified by update operation
/// </summary>
[Serializable]
public class EntityNotModifiedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    public EntityNotModifiedException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="message">Exception message</param>
    public EntityNotModifiedException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="innerException">Inner exception</param>
    public EntityNotModifiedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="info">Serialization info</param>
    /// <param name="context">Streaming context</param>
    protected EntityNotModifiedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}