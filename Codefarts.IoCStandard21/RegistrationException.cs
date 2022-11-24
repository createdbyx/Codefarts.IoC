// <copyright file="RegistrationException.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.IoC
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides an exception for registration errors.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class RegistrationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationException"/> class.
        /// </summary>
        public RegistrationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public RegistrationException(string message)
            : base(message)
        {
        }

        /// <inheritdoc />
        public RegistrationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <inheritdoc />
        protected RegistrationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}