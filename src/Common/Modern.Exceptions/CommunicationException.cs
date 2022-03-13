using System.Runtime.Serialization;

namespace Modern.Exceptions;

/// <summary>
/// Represents a base class for service communication errors
/// </summary>
[Serializable]
public class CommunicationException : Exception
{
    public CommunicationException()
    {
    }

    public CommunicationException(string message) : base(message)
    {
    }

    public CommunicationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected CommunicationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}