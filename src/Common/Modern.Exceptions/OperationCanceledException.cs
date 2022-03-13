using System.Runtime.Serialization;

namespace Modern.Exceptions;

/// <summary>
/// The exception that is thrown in a thread upon cancellation of an operation that the thread was executing
/// </summary>
[Serializable]
public class OperationCanceledException : Exception
{
    public OperationCanceledException()
    {
    }

    public OperationCanceledException(string message) : base(message)
    {
    }

    public OperationCanceledException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected OperationCanceledException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}