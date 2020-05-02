// <copyright file="ContainerExtensionMethods.cs" company="Codefarts">
// Copyright (c) Codefarts
// </copyright>

using System;

namespace Codefarts.IoC
{
    /// <summary>
    /// Provides extension methods for the <see cref="Container"/> class.
    /// </summary>
    public static class ContainerExtensionMethods
    {
        /// <summary>
        /// Tries the resolve the type.
        /// </summary>
        /// <typeparam name="T">The type to be resolved.</typeparam>
        /// <param name="container">The container that will be used to resolve the type.</param>
        /// <param name="defaultValue">The default value to return if the type can't be resolved.</param>
        /// <param name="value">The value that will be returned.</param>
        /// <returns>Returns true if the type was resolved; otherwise false;</returns>
        public static bool TryResolve<T>(this Container container, T defaultValue, out T value)
        {
            value = defaultValue;
            try
            {
                value = container.Resolve<T>();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Creates instance of a specified type.
        /// </summary>
        /// <typeparam name="T">Specifies the type to be instantiated.</typeparam>
        /// <param name="container">The container that will be used to resolve the type.</param>
        /// <returns>Returns a reference to a instance of <see cref="T"/>.</returns>
        public static T Resolve<T>(this Container container)
        {
            return (T)container.Resolve(typeof(T));
        }

        /// <summary>
        /// Registers a <see cref="Container.Creator" /> delegate for a given type.
        /// </summary>
        /// <typeparam name="T">The type that is to be registered.</typeparam>
        /// <param name="container">The container that will be used to resolve the type.</param>
        /// <param name="creator">The creator delegate.</param>
        public static void Register<T>(this Container container, Container.Creator creator)
        {
            container.Register(typeof(T), creator);
        }

        /// <summary>
        /// Registers a type within the container.
        /// </summary>
        /// <typeparam name="T">The type of the concrete class.</typeparam>
        /// <param name="container">The container that will be used to resolve the type.</param>
        public static void Register<T>(this Container container)
            where T : class
        {
            container.Register(typeof(T));
        }

        /// <summary>
        /// Registers a type key with a concrete type.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TConcrete">The type of the concrete class.</typeparam>
        /// <param name="container">The container that will be used to resolve the type.</param>
        public static void Register<TKey, TConcrete>(this Container container)
            where TConcrete : TKey
        {
            container.Register(typeof(TKey), typeof(TConcrete));
        }

        /// <summary>
        /// Unregisters a type from the container.
        /// </summary>
        /// <typeparam name="T">The type to be unregistered.</typeparam>
        /// <param name="container">The container that will be used to resolve the type.</param>
        /// <returns><c>true</c> if the type was successfully unregistered; otherwise <c>false</c>.</returns>
        public static bool Unregister<T>(this Container container)
        {
            return container.Unregister(typeof(T));
        }

        /// <summary>
        /// Tries the resolve the type.
        /// </summary>
        /// <typeparam name="T">The type to be resolved.</typeparam>
        /// <param name="container">The container that will be used to resolve the type.</param>
        /// <param name="defaultValue">The default value to return if the type can't be resolved.</param>
        /// <returns>Returns true if the type was resolved; otherwise false;</returns>
        public static T Resolve<T>(this Container container, T defaultValue)
        {
            T value;
            return TryResolve(container, defaultValue, out value) ? value : defaultValue;
        }
    }
}