// <copyright file="Container.cs" company="Codefarts">
// Copyright (c) Codefarts
// </copyright>

using System.Collections.ObjectModel;
using System.Linq;

namespace Codefarts.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Provides a simple IoC container functions.
    /// </summary>
    public partial class Container
    {
        /// <summary>
        /// The backing field for the <see cref="DefaultInstance"/> property.
        /// </summary>
        private static readonly Container DefaultInstance = new Container();

        /// <summary>
        /// The dictionary containing the registered types and there creation delegate reference.
        /// </summary>
        private readonly SafeDictionary<Type, Creator> typeCreators;

        /// <summary>
        /// Provides a delegate for constructing a type reference.
        /// </summary>
        /// <returns>A instance of a type.</returns>
        public delegate object Creator();

        /// <summary>
        /// Initializes static members of the <see cref="Container"/> class.
        /// </summary>
        static Container()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Container"/> class.
        /// </summary>
        public Container()
        {
            this.typeCreators = new SafeDictionary<Type, Creator>();
        }

        /// <summary>
        /// Gets the types that have been registered with the container.
        /// </summary>
        public IReadOnlyCollection<Type> RegisteredTypes
        {
            get
            {
                return new ReadOnlyCollection<Type>(this.typeCreators.Keys.ToArray());
            }
        }

        /// <summary>
        /// Lazy created Singleton instance of the container for simple scenarios.
        /// </summary>
        public static Container Default
        {
            get
            {
                return DefaultInstance;
            }
        }

        /// <summary>
        /// Creates instance of a specified type.
        /// </summary>
        /// <param name="type">Specifies the type to be instantiated.</param>
        /// <returns>Returns a reference to a instance of <paramref name="type"/>.</returns>
        /// <remarks>Will attempt to resolve the type even if there was no previous type <see cref="Creator"/> delegate specified for the type.</remarks>
        public object Resolve(Type type)
        {
            Creator provider;
            if (this.typeCreators.TryGetValue(type, out provider))
            {
                try
                {
                    return provider();
                }
                catch (Exception ex)
                {
                    throw new ContainerResolutionException(type, ex);
                }
            }

            return this.ResolveByType(type);
        }

        /// <summary>
        /// Registers a <see cref="Creator" /> delegate for a given type.
        /// </summary>
        /// <param name="type">The type to be registered.</param>
        /// <param name="creator">The creator delegate used to create the type.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="creator"/> is null.</exception>
        public void Register(Type type, Creator creator)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (creator == null)
            {
                throw new ArgumentNullException(nameof(creator));
            }

            this.PreviouslyRegisteredCheck(type);
            this.typeCreators[type] = creator;
        }

        /// <summary>
        /// Registers a <see cref="Creator" /> delegate for a given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <exception cref="RegistrationTypeException">Can not register interfaces, abstract classes or value types.</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="creator" /> is null.</exception>
        public void Register(Type type)
        {
            if (type.IsAbstract || type.IsInterface || type.IsValueType)
            {
                throw new RegistrationTypeException(type);
            }

            this.PreviouslyRegisteredCheck(type);
            this.typeCreators[type] = () => this.ResolveByType(type);
        }

        /// <summary>
        /// Registers a type key with a concrete type.
        /// </summary>
        /// <param name="key">The type of the key.</param>
        /// <param name="concrete">The type of the concrete class.</param>
        public void Register(Type key, Type concrete)
        {
            this.PreviouslyRegisteredCheck(key);
            this.typeCreators[key] = () => this.ResolveByType(concrete);
        }

        /// <summary>
        /// Unregister a type from the container.
        /// </summary>
        /// <param name="type">The type to be unregistered.</param>
        /// <returns><c>true</c> if the type was successfully unregistered; otherwise <c>false</c>.</returns>
        public bool Unregister(Type type)
        {
            return this.typeCreators.Remove(type);
        }

        /// <summary>
        /// Determines whether the type can be resolved.
        /// </summary>
        /// <typeparam name="T">The type to check if it can be resolved.</typeparam>
        /// <returns><c>true</c> if the type can be resolved; otherwise, <c>false</c>.</returns>
        public bool CanResolve<T>()
        {
            return this.CanResolve(typeof(T));
        }

        /// <summary>
        /// Determines whether the type can be resolved.
        /// </summary>
        /// <param name="type">The type to check if it can be resolved.</param>
        /// <returns><c>true</c> if the type can be resolved; otherwise, <c>false</c>.</returns>
        public bool CanResolve(Type type)
        {
            Creator value;
            return this.typeCreators.TryGetValue(type, out value);
        }

        /// <summary>
        /// Attempts to retrieve Gets the property value.
        /// </summary>
        /// <param name="value">The object whose property value to retrieve.</param>
        /// <param name="member">The property member information.</param>
        /// <returns>The value of the property.</returns>
        /// <exception cref="NullReferenceException">Property has no accessible get method.</exception>
        private object GetPropertyValue(object value, PropertyInfo member)
        {
            var getMethod = member.GetGetMethod();
            if (getMethod == null)
            {
                throw new NullReferenceException("Property has no accessible get method.");
            }

            var propValue = getMethod.Invoke(value, null);
            return propValue;
        }

        /// <summary>
        /// Private method that acts as a wrapper for the Resolve method.
        /// </summary>
        /// <typeparam name="T">The type to cast to before returning.</typeparam>
        /// <remarks>This is called by the <seealso cref="CreateAction"/> method.</remarks>
        private Func<T> Perform<T>()
        {
            return () => this.Resolve<T>();
        }

        /// <summary>
        /// Called by the <seealso cref="ResolveGenericType"/> method.
        /// </summary>
        private Delegate CreateAction(Type type)
        {
            var methodInfo = this.GetType().GetMethod("Perform", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(type);
            var funcGenericType = typeof(Func<>).MakeGenericType(type);
            return (Delegate)Convert.ChangeType(methodInfo.Invoke(this, null), funcGenericType);
        }

        /// <summary>
        /// Creates a instance of a type.
        /// </summary>
        /// <param name="type">The type that is to be instantiated.</param>
        /// <returns>The reference to the created instance.</returns>
        /// <remarks>Attempts to create the specified <param name="type"/> starting with the most number 
        /// of constructor arguments down to the constructor with the least arguments.</remarks>
        /// <exception cref="TypeLoadException"> Thrown if the type could not be constructed because none 
        /// of the available constructors could be satisfied.
        /// </exception>
        private object ResolveByType(Type type)
        {
            // check if the type if a generic type
            object genericResultValue;
            if (this.ResolveGenericType(type, out genericResultValue))
            {
                return genericResultValue;
            }

            if (type.IsAbstract || type.IsInterface || type.IsValueType)
            {
                throw new ContainerResolutionException(
                    type,
                    string.Format(
                        "The type '{0}' could not be resolved because it is either a interface, abstract class or value type.",
                        type.FullName));
            }

            var constructors = this.GetPublicConstructorWithValidParameters(type);

            // work through each constructor and attempt to instantiate it
            foreach (var constructor in constructors)
            {
                var arguments = this.ResolveParametersFromConstructorInfo(constructor);
                try
                {
                    var value = constructor.Invoke(arguments);
                    return value;
                }
                catch
                {
                    // ignored
                }
            }

            throw new ContainerResolutionException(
                type,
                string.Format(
                    "The type '{0}' could not be instantiated because none of the available constructors could be satisfied.",
                    type.FullName));
        }

        /// <summary>
        /// Tries to resolve a generic type definition.
        /// </summary>
        /// <param name="type">The type to be resolved.</param>
        /// <param name="result">A reference to a new object.</param>
        /// <returns>true if the generic type could be resolved.</returns>
        private bool ResolveGenericType(Type type, out object result)
        {
            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();

                // Just a generic func
                if (genericType == typeof(Func<>))
                {
                    var genericArguments = type.GetGenericArguments();
                    var returnType = genericArguments[0];
                    {
                        result = this.CreateAction(returnType);
                        return true;
                    }
                }

                // Just a generic ienumerable
                if (genericType == typeof(IEnumerable<>))
                {
                    var genericArguments = type.GetGenericArguments();
                    var returnType = genericArguments[0];
                    var defaultConstructor = this.GetDefaultListConstructor(returnType);
                    if (defaultConstructor != null)
                    {
                        result = defaultConstructor.Invoke(null);
                        return true;
                    }
                }
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Checks for previously registered type and if found throws an exception.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <exception cref="Codefarts.IoC.RegistrationException">If type has been registered already.</exception>
        private void PreviouslyRegisteredCheck(Type type)
        {
            Creator creator;
            if (this.typeCreators.TryGetValue(type, out creator) || creator != null)
            {
                throw new RegistrationException();
            }
        }
    }
}
