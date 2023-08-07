namespace Domain.Exceptions
{
    public class AppException : Exception
    {
        public AppException(string message, string[]? errors = null) : base(message)
        {
            Errors = errors;
        }

        public IEnumerable<string>? Errors { get; private set; }
    }
}
