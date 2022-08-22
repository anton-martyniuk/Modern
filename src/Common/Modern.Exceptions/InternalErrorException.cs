using System.Runtime.Serialization;

namespace Modern.Exceptions;

/// <summary>
/// The exception that is thrown when an internal error occurs
/// </summary>
[Serializable]
public class InternalErrorException : Exception
{
    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    public InternalErrorException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="message">Exception message</param>
    public InternalErrorException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="innerException">Inner exception</param>
    public InternalErrorException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="info">Serialization info</param>
    /// <param name="context">Streaming context</param>
    protected InternalErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}