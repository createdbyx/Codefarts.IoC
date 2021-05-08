// <copyright file="Container.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System.Diagnostics;
using System.Threading;

namespace Codefarts.IoC
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Provides a simple IoC container functions.
    /// </summary>
    public partial class Container : INotifyPropertyChanged
    {
        /// <summary>
        /// The default value for <see cref="MaxInstantiationDepth"/>.
        /// </summary>
        public const uint DefaultMaxInstantiationDepth = 25;

        /// <summary>
        /// The backing field for the <see cref="DefaultInstance"/> property.
        /// </summary>
        private static readonly Container DefaultInstance = new Container();

        /// <summary>
        /// The dictionary containing the registered types and there creation delegate reference.
        /// </summary>
        private readonly SafeDictionary<Type, Creator> typeCreators;

        /// <summary>
        /// Backing field for the <see cref="MaxInstantiationDepth"/> property.
        /// </summary>
        private uint maxInstantiationDepth = DefaultMaxInstantiationDepth;

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
        /// Provides a delegate for constructing a type reference.
        /// </summary>
        /// <returns>A instance of a type.</returns>
        public delegate object Creator();

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets a Lazy created Singleton instance of the container for simple scenarios.
        /// </summary>
        public static Container Default
        {
            get
            {
                return DefaultInstance;
            }
        }

        /// <summary>
        /// Gets or sets the max instantiation depth.
        /// </summary>
        /// <remarks>Max instantiation depth prevents the <see cref="Resolve"/> method from making circular instantiation references.</remarks>
        public uint MaxInstantiationDepth
        {
            get
            {
                return this.maxInstantiationDepth;
            }

            set
            {
                var currentValue = this.maxInstantiationDepth;
                if (currentValue != value)
                {
                    this.maxInstantiationDepth = value;
                    this.OnPropertyChanged(nameof(this.MaxInstantiationDepth));
                }
            }
        }

        /// <summary>
        /// Gets the types that have been registered with the container.
        /// </summary>
        public IEnumerable<Type> RegisteredTypes
        {
            get
            {
                return this.typeCreators.Keys;
            }
        }

        /// <summary>
        /// Creates instance of a specified type.
        /// </summary>
        /// <param name="type">Specifies the type to be instantiated.</param>
        /// <param name="args">Arguments to be passed to the type constructor.</param>
        /// <returns>Returns a reference to a instance of <paramref name="type"/>.</returns>
        /// <remarks>
        /// <p>Will attempt to resolve the type even if there was no previous type <see cref="Creator"/> delegate specified for the type.</p>
        /// <p>If no <paramref name="args"/> specified, will attempt to satisfy args automatically.</p>
        /// </remarks>
        public object Resolve(Type type, params object[] args)
        {
            return this.DoResolve(0, true, type, args);
        }

        /// <summary>
        /// Creates instance of a specified type.
        /// </summary>
        /// <param name="depth">Specified the instantiation depth.</param>
        /// <param name="type">Specifies the type to be instantiated.</param>
        /// <param name="args">Arguments to be passed to the type constructor.</param>
        /// <returns>Returns a reference to a instance of <paramref name="type"/>.</returns>
        /// <remarks>
        /// <p>Will attempt to resolve the type even if there was no previous type <see cref="Creator"/> delegate specified for the type.</p>
        /// <p>If no <paramref name="args"/> specified, will attempt to satisfy args automatically.</p>
        /// </remarks>
        private object DoResolve(int depth, bool captureExceptions, Type type, params object[] args)
        {
            Creator provider;
            if (this.typeCreators.TryGetValue(type, out provider))
            {
                if (captureExceptions)
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

                return provider();
            }

            return this.ResolveByType(depth, type, args);
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
        /// Registers a type key with a concrete type.
        /// </summary>
        /// <param name="key">The type of the key.</param>
        /// <param name="concrete">The type of the concrete class.</param>
        public void Register(Type key, Type concrete)
        {
            this.PreviouslyRegisteredCheck(key);
            var count = 0;
            this.typeCreators[key] = () =>
            {
                Interlocked.Increment(ref count);
                //  Debug.WriteLine("count: " + count);
                var result = this.ResolveByType(count, concrete);
                Interlocked.Decrement(ref count);
                return result;
            };
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
        /// <param name="type">The type to check if it can be resolved.</param>
        /// <returns><c>true</c> if the type can be resolved; otherwise, <c>false</c>.</returns>
        /// <remarks><see cref="CanResolve"/> checks <see cref="RegisteredTypes"/> property to see if a type has previously been registered with a <see cref="Register(System.Type,Codefarts.IoC.Container.Creator)"/>.</remarks>
        public bool CanResolve(Type type)
        {
            Creator value;
            return this.typeCreators.TryGetValue(type, out value);
        }

        ///// <summary>
        ///// Private method that acts as a wrapper for the Resolve method.
        ///// </summary>
        ///// <typeparam name="T">The type to cast to before returning.</typeparam>
        ///// <remarks>This is called by the <seealso cref="CreateAction"/> method.</remarks>
        //private Func<T> Perform<T>()
        //{
        //    return () => this.Resolve<T>();
        //}

        ///// <summary>
        ///// Called by the <seealso cref="ResolveGenericType"/> method.
        ///// </summary>
        //private Delegate CreateAction(Type type)
        //{
        //    var methodInfo = this.GetType().GetMethod(nameof(Perform), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(type);
        //    var funcGenericType = typeof(Func<>).MakeGenericType(type);
        //    return (Delegate)Convert.ChangeType(methodInfo.Invoke(this, null), funcGenericType);
        //}

        /// <summary>
        /// Creates a instance of a type.
        /// </summary>
        /// <param name="depth">Specified the instantiation depth.</param>
        /// <param name="type">The type that is to be instantiated.</param>
        /// <param name="args">Arguments to be passed to the type constructor.</param>
        /// <returns>The reference to the created instance.</returns>
        /// <remarks><p>Attempts to create the specified <param name="type"/> with the most number
        /// of constructor arguments that can be satisfied.</p>
        /// <p>Constructors with value types are ignored and only public constructors are considered.</p></remarks>
        /// <exception cref="TypeLoadException"> Thrown if the type could not be constructed because none
        /// of the available constructors could be satisfied.
        /// </exception>
        private object ResolveByType(int depth, Type type, params object[] args)
        {
            Debug.WriteLine("Depth: " + depth);
            if (depth > this.MaxInstantiationDepth)
            {
                throw new ExceededMaxInstantiationDepthException(Resources.ERR_ExceededMaxInstantiationDepth);
            }

            // can't resolve abstract classes, interfaces, value types, delegates, or strings
            if (this.IsInvalidInstantiationType(type))
            {
                throw new ContainerResolutionException(type, string.Format(Resources.ERR_IsInvalidInstantiationType, type.FullName));
            }

            //// check if the type if a generic type
            //object genericResultValue;
            //if (this.ResolveGenericType(type, out genericResultValue))
            //{
            //    return genericResultValue;
            //}

            var hasSpecifiedArgs = args != null && args.Length > 0;
            var constructors = (hasSpecifiedArgs ? this.GetPublicConstructorWithMatchingParameters(type, args) : this.GetPublicConstructorWithValidParameters(type)).ToArray();

            // get constructor with the most parameters and attempt to instantiate it
            var constructor = constructors.OrderBy(x => x.GetParameters().Length).LastOrDefault();

            try
            {
                var arguments = hasSpecifiedArgs ? args : constructor.GetParameters().Select(p => this.DoResolve(depth + 1, false, p.ParameterType)).ToArray();
                var value = constructor.Invoke(arguments);
                return value;
            }
            catch (ExceededMaxInstantiationDepthException mde)
            {
                throw mde;
            }
            catch (Exception ex)
            {
                throw new ContainerResolutionException(type, string.Format(Resources.ERR_NoAvailableConstructors, type.FullName), ex);
            }
        }

        ///// <summary>
        ///// Tries to resolve a generic type definition.
        ///// </summary>
        ///// <param name="type">The type to be resolved.</param>
        ///// <param name="result">A reference to a new object.</param>
        ///// <returns>true if the generic type could be resolved.</returns>
        //private bool ResolveGenericType(Type type, out object result)
        //{
        //    if (type.IsGenericType)
        //    {
        //        var genericType = type.GetGenericTypeDefinition();

        //        //// Just a generic func
        //        //if (genericType == typeof(Func<>))
        //        //{
        //        //    var genericArguments = type.GetGenericArguments();
        //        //    var returnType = genericArguments[0];
        //        //    {
        //        //        result = this.CreateAction(returnType);
        //        //        return true;
        //        //    }
        //        //}

        //        // Just a generic IEnumerable
        //        if (genericType == typeof(IEnumerable<>))
        //        {
        //            var genericArguments = type.GetGenericArguments();
        //            var returnType = genericArguments[0];
        //            var defaultConstructor = this.GetDefaultListConstructor(returnType);
        //            if (defaultConstructor != null)
        //            {
        //                result = defaultConstructor.Invoke(null);
        //                return true;
        //            }
        //        }
        //    }

        //    result = null;
        //    return false;
        //}

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
                throw new RegistrationException(string.Format(Resources.ERR_AlreadyRegistered, type.FullName));
            }
        }

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="propertyName">The property name that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
