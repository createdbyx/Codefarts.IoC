// <copyright file="RegistrationTypeException.cs" company="Codefarts">
// Copyright (c) Codefarts
// </copyright>

namespace Codefarts.IoC
{
    using System;

    public class RegistrationTypeException : Exception
    {
        private const string RegisterErrorText = "Cannot register type {0} - abstract classes or interfaces are not valid implementation.";

        public RegistrationTypeException()
            : base()
        {
        }

        public RegistrationTypeException(Type type)
            : base(string.Format(RegisterErrorText, type.FullName))
        {
        }

        public RegistrationTypeException(Type type, string message)
            : base(string.Format(RegisterErrorText, type.FullName) + (string.IsNullOrEmpty(message) ? string.Empty : "\r\n" + message))
        {
        }

        public RegistrationTypeException(Type type, string message, Exception innerException)
            : base(string.Format(RegisterErrorText, type.FullName) + (string.IsNullOrEmpty(message) ? string.Empty : "\r\n" + message), innerException)
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