using System.Runtime.Serialization;

namespace Modern.Exceptions;

/// <summary>
/// The exception that is thrown when entity is not modified by update operation
/// </summary>
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
}