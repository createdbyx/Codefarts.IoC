namespace Codefarts.IoC
{
    using System;

    public class RegistrationTypeException : Exception
    {
        private const string REGISTER_ERROR_TEXT = "Cannot register type {0} - abstract classes or interfaces are not valid implementation.";

        public RegistrationTypeException(Type type)
            : base(string.Format(REGISTER_ERROR_TEXT, type.FullName))
        {
        }

        public RegistrationTypeException(Type type, string message)
            : base(string.Format(REGISTER_ERROR_TEXT, type.FullName) + (string.IsNullOrEmpty(message) ? string.Empty : "\r\n" + message))
        {
        }

        public RegistrationTypeException(Type type, string message, Exception innerException)
            : base(string.Format(REGISTER_ERROR_TEXT, type.FullName) + (string.IsNullOrEmpty(message) ? string.Empty : "\r\n" + message), innerException)
        {
        }

#if SERIALIZABLE
        protected RegistrationTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}