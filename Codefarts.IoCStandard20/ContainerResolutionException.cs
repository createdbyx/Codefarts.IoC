// <copyright file="ContainerResolutionException.cs" company="Codefarts">
// Copyright (c) Codefarts
// </copyright>

namespace Codefarts.IoC
{
    using System;

    /// <summary>Provides an exception for type resolution failures.</summary>
    /// <seealso cref="System.Exception" />
    public class ContainerResolutionException : Exception
    {
        private const string ErrorText = "Unable to resolve type: {0}";

        /// <summary>
        ///  Initializes a new instance of the <see cref="ContainerResolutionException"/> class.
        /// </summary>
        /// <param name="type">The type that could not be resolved.</param>
        public ContainerResolutionException(Type type)
            : base(string.Format(ErrorText, type.FullName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerResolutionException"/> class.
        /// </summary>
        /// <param name="type">The type that could not be resolved.</param>
        /// <param name="innerException">Gets the Exception instance that caused the current exception.</param>
        public ContainerResolutionException(Type type, Exception innerException)
            : base(string.Format(ErrorText, type.FullName), innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerResolutionException"/> class.
        /// </summary>
        /// <param name="type">The type that could not be resolved.</param>
        /// <param name="message">The error message to report.</param>
        public ContainerResolutionException(Type type, string message)
            : base(string.Format(ErrorText, type.FullName) + "\r\n" + message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerResolutionException"/> class.
        /// </summary>
        /// <param name="type">The type that could not be resolved.</param>
        /// <param name="message">The error message to report.</param>
        /// <param name="innerException">Gets the Exception instance that caused the current exception.</param>
        public ContainerResolutionException(Type type, string message, Exception innerException)
            : base(string.Format(ErrorText, type.FullName) + "\r\n" + message, innerException)
        {
        }
    }
}