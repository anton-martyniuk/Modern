using System.Runtime.Serialization;

namespace Modern.Exceptions;

/// <summary>
/// Represents a service communication timeout error
/// </summary>
[Serializable]
public class CommunicationTimeoutException : Exception
{
    public CommunicationTimeoutException()
    {
    }

    public CommunicationTimeoutException(string message) : base(message)
    {
    }

    public CommunicationTimeoutException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected CommunicationTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}