namespace Codefarts.IoC
{
    using System;

    public class ContainerResolutionException : Exception
    {
        private const string ERROR_TEXT = "Unable to resolve type: {0}";

        public ContainerResolutionException(Type type)
            : base(string.Format(ERROR_TEXT, type.FullName))
        {
        }

        public ContainerResolutionException(Type type, Exception innerException)
            : base(string.Format(ERROR_TEXT, type.FullName), innerException)
        {
        }

        public ContainerResolutionException(Type type, string message)
            : base(string.Format(ERROR_TEXT, type.FullName) + "\r\n" + message)
        {
        }

        public ContainerResolutionException(Type type, string message, Exception innerException)
            : base(string.Format(ERROR_TEXT, type.FullName) + "\r\n" + message, innerException)
        {
        }
    }
}