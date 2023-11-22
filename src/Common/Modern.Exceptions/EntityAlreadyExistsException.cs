using System.Runtime.Serialization;

namespace Modern.Exceptions;

/// <summary>
/// The exception that is thrown when entity is not created because of the existence of a similar entity
/// </summary>
public class EntityAlreadyExistsException : Exception
{
    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    public EntityAlreadyExistsException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="message">Exception message</param>
    public EntityAlreadyExistsException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="innerException">Inner exception</param>
    public EntityAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
    {
    }
}