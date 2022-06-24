namespace SwaggerMerge.Exceptions;

/// <summary>
/// Defines an exception thrown when an error occurs with the Swagger merge tool.
/// </summary>
public class SwaggerMergeException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SwaggerMergeException"/> class with a message.
    /// </summary>
    /// <param name="message">The detail of the exception thrown.</param>
    public SwaggerMergeException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SwaggerMergeException"/> class with a message and inner exception.
    /// </summary>
    /// <param name="message">The detail of the exception thrown.</param>
    /// <param name="innerException">The exception that caused this exception to be thrown.</param>
    public SwaggerMergeException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}