// <copyright file="ExceededMaxInstantiationDepthException.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.IoC
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The exceeded max instantiation depth exception.
    /// </summary>
    public class ExceededMaxInstantiationDepthException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceededMaxInstantiationDepthException"/> class.
        /// </summary>
        public ExceededMaxInstantiationDepthException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceededMaxInstantiationDepthException"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected ExceededMaxInstantiationDepthException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceededMaxInstantiationDepthException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ExceededMaxInstantiationDepthException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceededMaxInstantiationDepthException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ExceededMaxInstantiationDepthException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}