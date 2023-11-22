using System.Runtime.Serialization;

namespace Modern.Exceptions;

/// <summary>
/// The exception that is thrown when entity is missing in the data store
/// </summary>
public class EntityNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    public EntityNotFoundException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="message">Exception message</param>
    public EntityNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="innerException">Inner exception</param>
    public EntityNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}