// <copyright file="Container.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System.Collections;

namespace Codefarts.IoC
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Threading;

    /// <summary>
    /// Provides a simple IoC container functions.
    /// </summary>
    public class Container : INotifyPropertyChanged
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
        private readonly ConcurrentDictionary<Type, CreatorData> typeCreators;

        /// <summary>
        /// Backing field for the <see cref="MaxInstantiationDepth"/> property.
        /// </summary>
        private uint maxInstantiationDepth = DefaultMaxInstantiationDepth;

        private class CreatorData
        {
            public Creator Creator { get; set; }
            public Type ConcreteType { get; set; }
            public bool InternalCreator { get; set; }
        }

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
            this.typeCreators = new ConcurrentDictionary<Type, CreatorData>();
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
        /// <returns>Returns a reference to a instance of <paramref name="type"/>.</returns>
        /// <remarks>
        /// <p>Will attempt to resolve the type even if there was no previous type <see cref="Creator"/> delegate specified for the type.</p>
        /// </remarks>
        public object Resolve(Type type)
        {
            CreatorData provider;
            if (this.typeCreators.TryGetValue(type, out provider))
            {
                try
                {
                    return provider.Creator();
                }
                catch (Exception ex)
                {
                    throw new ContainerResolutionException(type, ex);
                }
            }

            return this.ResolveByType(0, type);
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
            this.typeCreators[type] = new CreatorData() { Creator = creator };
        }

        /// <summary>
        /// Registers a type key with a concrete type.
        /// </summary>
        /// <param name="key">The type of the key.</param>
        /// <param name="concrete">The type of the concrete class.</param>
        public void Register(Type key, Type concrete)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (concrete == null)
            {
                throw new ArgumentNullException(nameof(concrete));
            }

            // make sure not already registered
            this.PreviouslyRegisteredCheck(key);

            // can't register same type for key and concrete.
            if (key.Equals(concrete))
            {
                throw new RegistrationException(string.Format(Resources.ERR_KeyAndConcreteTypeCannotBeTheSame, key.FullName, concrete.FullName));
            }

            // validate the concrete type.
            if (concrete.IsAbstract ||
                concrete.IsInterface ||
                concrete.IsValueType ||
                typeof(Delegate).IsAssignableFrom(concrete) ||
                concrete == typeof(string) ||
                !key.IsAssignableFrom(concrete))
            {
                throw new RegistrationException(string.Format(Resources.ERR_InvalidConcreteType, concrete.FullName));
            }

            // The purpose of tracking call counts to to prevent stack overflow exceptions and
            // stay within the MaxInstantiationDepth value for call depths.
            //   var callCount = new Dictionary<int, int>();
            this.typeCreators[key] = new CreatorData()
            {
                InternalCreator = true, ConcreteType = concrete, Creator = () =>
                {
                    // // NOTE: the fallowing code smells and I should come back to it at some point
                    // // I feel like the overall architecture approach is not elegant enough.
                    // var threadId = Thread.CurrentThread.ManagedThreadId;
                    // int count;
                    // lock (callCount)
                    // {
                    //     callCount.TryGetValue(threadId, out count);
                    //
                    //     count++;
                    //     callCount[threadId] = count;
                    // }

                    // object result;
                    // // try
                    // // {
                    // Creator provider;
                    // if (this.typeCreators.TryGetValue(concrete, out provider))
                    // {
                    //     try
                    //     {
                    //         return provider();
                    //     }
                    //     catch (Exception ex)
                    //     {
                    //         throw new ContainerResolutionException(concrete, ex);
                    //     }
                    // }
                    //
                    // result = this.ResolveByType(0, concrete);
                    // }
                    // finally
                    // {
                    //     lock (callCount)
                    //     {
                    //         count--;
                    //         if (count <= 0)
                    //         {
                    //             callCount.Remove(threadId);
                    //         }
                    //         else
                    //         {
                    //             callCount[threadId] = count;
                    //         }
                    //     }
                    // }

                    //return result;

                    return ResolveByType(0, concrete);
                }
            };
        }

        /// <summary>
        /// Unregister a type from the container.
        /// </summary>
        /// <param name="type">The type to be unregistered.</param>
        /// <returns><c>true</c> if the type was successfully unregistered; otherwise <c>false</c>.</returns>
        public bool Unregister(Type type)
        {
            CreatorData value;
            return this.typeCreators.Remove(type, out value);
        }

        /// <summary>
        /// Determines whether the type can be resolved.
        /// </summary>
        /// <param name="type">The type to check if it can be resolved.</param>
        /// <returns><c>true</c> if the type can be resolved; otherwise, <c>false</c>.</returns>
        /// <remarks><see cref="CanResolve"/> checks <see cref="RegisteredTypes"/> property to see if a type has previously been registered with a
        /// <see cref="Register(System.Type,Codefarts.IoC.Container.Creator)"/>.</remarks>
        public bool CanResolve(Type type)
        {
            CreatorData value;
            if (this.typeCreators.TryGetValue(type, out value))
            {
                return true;
            }

            // can't resolve abstract classes, interfaces, value types, delegates, or strings
            if (type.IsAbstract ||
                type.IsInterface ||
                type.IsValueType ||
                typeof(Delegate).IsAssignableFrom(type) ||
                type == typeof(string))
            {
                return false;
            }

            return type.IsPublic;
        }

        /// <summary>
        /// Creates a instance of a type.
        /// </summary>
        /// <param name="depth">Specified the instantiation depth.</param>
        /// <param name="type">The type that is to be instantiated.</param>
        /// <exception cref="ExceededMaxInstantiationDepthException">Raised if the depth argument is greater then
        /// the <see cref="MaxInstantiationDepth"/> property.</exception>
        /// <returns>The reference to the created instance.</returns>
        /// <remarks><p>Attempts to create the specified <param name="type"/> with the most number
        /// of constructor arguments that can be satisfied.</p>
        /// <p>Constructors with value types are ignored and only public constructors are considered.</p></remarks>
        /// <exception cref="ContainerResolutionException"> Thrown if the type could not be constructed because 
        /// the chosen constructor could be satisfied.
        /// </exception>
        private object ResolveByType(int depth, Type type)
        {
            // if (depth > this.MaxInstantiationDepth)
            // {
            //     throw new ExceededMaxInstantiationDepthException(Resources.ERR_ExceededMaxInstantiationDepth);
            // }

            // can't resolve abstract classes, interfaces, value types, delegates, or strings
            if (type.IsAbstract ||
                type.IsInterface ||
                type.IsValueType ||
                typeof(Delegate).IsAssignableFrom(type) ||
                type == typeof(string))
            {
                throw new ContainerResolutionException(type, string.Format(Resources.ERR_IsInvalidInstantiationType, type.FullName));
            }

            // if the type implements IEnumerable 
            if (type.IsAssignableTo(typeof(ICollection)))
            {
                return CreateEmptyCollection(type);
                // if (type.IsAssignableTo(typeof(IDictionary)))
                // {
                // }
                // else
                // {
                //     return Enumerable.Empty<Type>();
                // }
                //
                // ConstructorInfo? c = null;
                // if (type.IsArray)
                // {
                //     c = type.GetConstructors()[0];
                //     return c.Invoke(new object[] { 0 });
                // }
                //
                // foreach (var x in type.GetConstructors())
                // {
                //     if (x.GetParameters().Length == 0)
                //     {
                //         c = x;
                //         break;
                //     }
                // }
                //
                // return c.Invoke(new object[0]);
            }

            try
            {
                var list = new List<InfoContainer>();

                var constructor = GetBestConstructorInfo(type);
                var info = new InfoContainer(constructor);
                list.Add(info);
                var listIndex = 0;

                // mapping
                while (listIndex < list.Count)
                {
                    // if (list.Count > this.MaxInstantiationDepth)
                    // {
                    //     throw new ExceededMaxInstantiationDepthException(Resources.ERR_ExceededMaxInstantiationDepth);
                    // }

                    // get last
                    var current = list[listIndex];

                    // map constructor tree
                    var pList = new List<InfoContainer>();
                    if (current.Constructor != null && !current.Constructor.DeclaringType.IsArray)
                    {
                        var parameters = current.Constructor.GetParameters();
                        for (var pIndex = 0; pIndex < parameters.Length; pIndex++)
                        {
                            var parameterInfo = parameters[pIndex];
                            var pCon = this.GetBestConstructorInfo(parameterInfo.ParameterType);
                            if (pCon != null)
                            {
                                pList.Add(new InfoContainer(pCon));
                            }
                            else
                            {
                                CreatorData provider;
                                //object value = null;
                                if (this.typeCreators.TryGetValue(parameterInfo.ParameterType, out provider))
                                {
                                    var cInfo = this.GetBestConstructorInfo(provider.ConcreteType);
                                    if (cInfo != null)
                                    {
                                        var pc = new InfoContainer(cInfo);
                                        pList.Add(pc);
                                    }
                                    else
                                    {
                                        var pc = new InfoContainer() { ObjectReference = provider.Creator() };
                                        pList.Add(pc);
                                    }
                                }
                                else
                                {
                                    throw new ContainerResolutionException(parameterInfo.ParameterType,
                                                                           string.Format(Resources.ERR_IsInvalidInstantiationType,
                                                                                         parameterInfo.ParameterType));
                                }
                            }

                            // info = new InfoContainer(pCon, true);
                            // var infoParams = pCon.GetParameters();
                            // if (infoParams.Length > 0)
                            // {
                            //     var args = new InfoContainer[infoParams.Length];
                            //     for (var i = 0; i < infoParams.Length; i++)
                            //     {
                            //         var param = infoParams[i];
                            //         args[i] = new InfoContainer(this.GetBestConstructorInfo(param.ParameterType));
                            //     }
                            //
                            //     info.Parameters = args;
                            //     list.Insert(listIndex + 1, info);
                            // }
                        }
                    }

                    current.Parameters = pList.Count == 0 ? null : pList;
                    list.AddRange(pList);

                    listIndex++;
                    //
                    // var last = stack.Pop();
                    // if (last.Parameters!=null)
                    // {
                    //     while (last.ObjectReference == null)
                    //     {
                    //         last.ObjectReference = last.Constructor.Invoke(null);
                    //     }
                    // }
                    // else
                    // {
                    //     stack.Push(last);
                    // }
                }

                // building
                while (list.Count > 0)
                {
                    var last = list[^1];
                    //  var args = last.Parameters == null ? null : last.Parameters;
                    var argRefs = last.Parameters == null ? null : new object[last.Parameters.Count];
                    if (last.Parameters != null)
                    {
                        for (var i = 0; i < last.Parameters.Count; i++)
                        {
                            var item = last.Parameters[i];
                            argRefs[i] = item.ObjectReference; //.Parameters[i].Constructor.Invoke(null);
                        }
                    }

                    if (last.ObjectReference == null && last.Constructor != null)
                    {
                        if (last.Constructor.DeclaringType.IsArray)
                        {
                            last.ObjectReference = last.Constructor.Invoke(new object[] { 0 });
                        }
                        else
                        {
                            last.ObjectReference = last.Constructor.Invoke(argRefs);
                        }
                    }

                    list.RemoveAt(list.Count - 1);
                }

                // var arguments = this.BuildConstructorArguments(constructor);
                // var value = constructor.Invoke(arguments);
                return info.ObjectReference;
            }
            catch
                (ExceededMaxInstantiationDepthException mde)
            {
                throw mde;
            }
            catch (ContainerResolutionException cre)
            {
                throw cre;
            }
            catch (Exception ex)
            {
                throw new ContainerResolutionException(type, string.Format(Resources.ERR_NoAvailableConstructors, type.FullName), ex);
            }
        }

        private ConstructorInfo GetBestConstructorInfo(Type type)
        {
            if (type == null)
            {
                return null;
            }

            // get all valid public constructors
            var constructors = type.GetConstructors();
            ConstructorInfo constructor = null;
            var lastParameterLength = 0;

            if (type.IsAssignableTo(typeof(ICollection)) || type.IsAssignableTo(typeof(IEnumerable)))
            {
                CreatorData provider;
                object value = null;
                if (this.typeCreators.TryGetValue(type, out provider))
                {
                    if (provider.ConcreteType != null)
                    {
                        type = provider.ConcreteType;
                    }
                    else
                    {
                        return null;
                    }
                    // try
                    // {
                    //     value = provider();
                    // }
                    // catch (Exception ex)
                    // {
                    //     throw new ContainerResolutionException(type, ex);
                    // }
                }

                //type = value == null ? type : value.GetType(); //.GetConstructors()[0];

                if (type.IsArray)
                {
                    return type.GetConstructors()[0];
                }
                else
                {
                    if (type.IsAssignableTo(typeof(IDictionary)))
                    {
                        return this.GetDictionaryConstructor(type);
                    }

                    return this.GetListConstructor(type.GetGenericArguments()[0]);
                }
            }

            // search for best constructor
            foreach (var c in constructors)
            {
                if (!c.IsPublic)
                {
                    continue;
                }

                var parameters = c.GetParameters();
                var invalidParameters = false;
                foreach (var x in parameters)
                {
                    invalidParameters = x.ParameterType.IsValueType ||
                                        typeof(Delegate).IsAssignableFrom(x.ParameterType) ||
                                        type == typeof(string);
                    if (invalidParameters)
                    {
                        break;
                    }
                }

                // parameters are not invalid
                if (!invalidParameters && parameters.Length >= lastParameterLength)
                {
                    lastParameterLength = parameters.Length;
                    constructor = c;
                }
            }

            // return constructor if found
            return constructor;
        }

        private IEnumerable<T> GetIEnumerable<T>()
        {
            return Enumerable.Empty<T>();
        }

        private ConstructorInfo GetArrayConstructor<T>()
        {
            var type = typeof(T[]);
            return type.GetConstructors()[0];
        }

        private ConstructorInfo GetListConstructor(Type type)
        {
            var t = typeof(List<>).MakeGenericType(type);
            var ctors = t.GetConstructors();
            // find default ctor
            foreach (var c in ctors)
            {
                if (c.GetParameters().Length == 0)
                {
                    return c;
                }
            }

            return null;
        }

        private ConstructorInfo GetDictionaryConstructor(Type type)
        {
            var t = typeof(Dictionary<,>).MakeGenericType(type.GetGenericArguments());
            var ctors = t.GetConstructors();
            // find default ctor
            foreach (var c in ctors)
            {
                if (c.GetParameters().Length == 0)
                {
                    return c;
                }
            }

            return null;
        }

        private IEnumerable CreateEmptyCollection(Type type)
        {
            if (type.IsArray)
            {
                return (IEnumerable)type.GetConstructors()[0].Invoke(new object[] { 0 });
            }
            else
            {
                if (type.IsAssignableTo(typeof(IDictionary)))
                {
                    return (IEnumerable)this.GetDictionaryConstructor(type).Invoke(null);
                }
                else
                {
                    return (IEnumerable)this.GetListConstructor(type.GetGenericArguments()[0]).Invoke(null);
                }
            }

            // var methodInfo = this.GetType().GetMethod("GetIEnumerable", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(type);
            // var result = methodInfo.Invoke(this, null);
            // return (IEnumerable)result;
        }

        /*
        /// <summary>
        /// Private method that acts as a wrapper for the Resolve method.
        /// </summary>
        /// <typeparam name="T">The type to cast to before returning.</typeparam>
        /// <remarks>This is called by the <seealso cref="CreateAction"/> method.</remarks>
        private Func<IEnumerable<T>> Perform<T>()
        {
            return () => Enumerable.Empty<T>();
        }

        /// <summary>
        /// Called by the <seealso cref="ResolveGenericType"/> method.
        /// </summary>
        private Delegate CreateCollection(Type type)
        {
            var methodInfo = this.GetType().GetMethod("Perform", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(type);
            var funcGenericType = typeof(Func<>).MakeGenericType(type);
            return (Delegate)Convert.ChangeType(methodInfo.Invoke(this, null), funcGenericType);
        } */


        private class InfoContainer
        {
            public ConstructorInfo Constructor;

            public InfoContainer(ConstructorInfo constructor)
            {
                this.Constructor = constructor;
            }

            public InfoContainer(ConstructorInfo constructor, bool isParameter)
            {
                this.Constructor = constructor;
                this.IsParameter = isParameter;
            }

            public object ObjectReference;
            public bool IsParameter;
            public List<InfoContainer> Parameters;

            public InfoContainer()
            {
            }
        }

        private object[] BuildConstructorArguments(ConstructorInfo constructor)
        {
            var parameters = constructor.GetParameters();
            var arguments = new object[parameters.Length];
            for (var i = 0; i < arguments.Length; i++)
            {
                var paramType = parameters[i].ParameterType;
                arguments[i] = this.Resolve(paramType);
            }

            return arguments;
        }

        /// <summary>
        /// Checks for previously registered type and if found throws an exception.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <exception cref="Codefarts.IoC.RegistrationException">If type has been registered already.</exception>
        private void PreviouslyRegisteredCheck(Type type)
        {
            CreatorData creator;
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