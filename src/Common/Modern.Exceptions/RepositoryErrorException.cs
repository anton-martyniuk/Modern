﻿namespace Modern.Exceptions;

/// <summary>
/// The exception that is thrown when an unhandled error occurs in repository
/// </summary>
public class RepositoryErrorException : Exception
{
    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    public RepositoryErrorException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="message">Exception message</param>
    public RepositoryErrorException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="innerException">Inner exception</param>
    public RepositoryErrorException(string message, Exception innerException) : base(message, innerException)
    {
    }
}