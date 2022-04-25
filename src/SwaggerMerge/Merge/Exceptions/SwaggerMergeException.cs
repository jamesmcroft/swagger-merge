namespace SwaggerMerge.Merge.Exceptions
{
    internal class SwaggerMergeException : Exception
    {
        public SwaggerMergeException(string message) : base(message)
        {
        }

        public SwaggerMergeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}