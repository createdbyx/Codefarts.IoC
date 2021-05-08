﻿// <copyright file="Container.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System.Threading;

namespace Codefarts.IoC
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

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
            var callCount = new Dictionary<int, int>();
            this.typeCreators[key] = () =>
            {
                // NOTE: the fallowing code smells and I should come back to it at some point
                // I feel like the overall architecture is not elegant enough.
                var threadId = Thread.CurrentThread.ManagedThreadId;
                int count;
                lock (callCount)
                {
                    callCount.TryGetValue(threadId, out count);

                    count++;
                    callCount[threadId] = count;
                }

                object result;
                try
                {
                    result = this.ResolveByType(count, concrete);
                }
                finally
                {
                    lock (callCount)
                    {
                        count--;
                        if (count <= 0)
                        {
                            callCount.Remove(threadId);
                        }
                        else
                        {
                            callCount[threadId] = count;
                        }
                    }
                }

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
            if (depth > this.MaxInstantiationDepth)
            {
                throw new ExceededMaxInstantiationDepthException(Resources.ERR_ExceededMaxInstantiationDepth);
            }

            // can't resolve abstract classes, interfaces, value types, delegates, or strings
            if (this.IsInvalidInstantiationType(type))
            {
                throw new ContainerResolutionException(type, string.Format(Resources.ERR_IsInvalidInstantiationType, type.FullName));
            }

            var hasSpecifiedArgs = args != null && args.Length > 0;
            var constructors = hasSpecifiedArgs ?
                this.GetPublicConstructorWithMatchingParameters(type, args) :
                this.GetPublicConstructorWithValidParameters(type);

            // get constructor with the most parameters and attempt to instantiate it
            var constructor = constructors.OrderBy(x => x.GetParameters().Length).LastOrDefault();

            try
            {
                object[] arguments;
                if (hasSpecifiedArgs)
                {
                    arguments = args;
                }
                else
                {
                    var parameters = constructor.GetParameters();
                    arguments = new object[parameters.Length];
                    for (var i = 0; i < arguments.Length; i++)
                    {
                        var paramType = parameters[i].ParameterType;
                        Creator provider;
                        if (this.typeCreators.TryGetValue(paramType, out provider))
                        {
                            arguments[i] = provider();
                        }
                        else
                        {
                            arguments[i] = this.ResolveByType(depth + 1, paramType, null);
                        }
                    }
                }

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