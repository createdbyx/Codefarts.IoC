// <copyright file="ContainerExtensionMethods.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.IoC
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides extension methods for the <see cref="Container"/> class.
    /// </summary>
    public static class ContainerExtensionMethods
    {
        /// <summary>
        /// Tries the resolve the type.
        /// </summary>
        /// <param name="container">The container that will be used to resolve the type.</param>
        /// <param name="defaultValue">The default value to return if the type can't be resolved.</param>
        /// <param name="value">The value that will be returned.</param>
        /// <returns>Returns true if the type was resolved; otherwise false;</returns>
        public static bool TryResolve(this Container container, Type type, object defaultValue, out object value)
        {
            value = defaultValue;
            try
            {
                value = container.Resolve(type);
                return true;
            }
            catch
            {
                return false;
            }
        }

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
        /// Determines whether the type can be resolved.
        /// </summary>
        /// <typeparam name="T">The type to check if it can be resolved.</typeparam>
        /// <param name="container">The container that will be used to resolve the type.</param>
        /// <returns><c>true</c> if the type can be resolved; otherwise, <c>false</c>.</returns>
        public static bool CanResolve<T>(this Container container)
        {
            return container.CanResolve(typeof(T));
        }

        /// <summary>
        /// Tries the resolve the type.
        /// </summary>
        /// <typeparam name="T">The type to be resolved.</typeparam>
        /// <param name="container">The container that will be used to resolve the type.</param>
        /// <param name="defaultValue">The default value to return if the type can't be resolved.</param>
        /// <returns>Returns the resolved object if successful, otherwise returns provided default value.</returns>
        public static T Resolve<T>(this Container container, T defaultValue)
        {
            T value;
            return TryResolve(container, defaultValue, out value) ? value : defaultValue;
        }

        /// <summary>
        /// Tries the resolve the type.
        /// </summary>
        /// <param name="container">The container that will be used to resolve the type.</param>
        /// <param name="defaultValue">The default value to return if the type can't be resolved.</param>
        /// <returns>Returns true if the type was resolved; otherwise false;</returns>
        public static object Resolve(this Container container, Type type, object defaultValue)
        {
            object value;
            return TryResolve(container, defaultValue, out value) ? value : defaultValue;
        }

        public static void ResolveMembers(this Container container, object value)
        {
            ResolveMembers(container, value, false);
        }

        public static void ResolveMembers(this Container container, object value, bool suppressExceptions)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var type = value.GetType();
            var memberInfos = type.GetMembers();
            var propertyMembers = from member in memberInfos
                                  where member.MemberType == MemberTypes.Property
                                  let info = (PropertyInfo)member
                                  where info.GetGetMethod() != null && info.GetSetMethod() != null && !info.PropertyType.IsValueType &&
                                        info.PropertyType != typeof(string)
                                  let getMeth = info.GetGetMethod()
                                  let setMeth = info.GetSetMethod()
                                  where getMeth.IsPublic && setMeth.IsPublic
                                  select info;

            var fieldMembers = from member in memberInfos
                               where member.MemberType == MemberTypes.Field
                               let info = (FieldInfo)member
                               where info.IsPublic && !info.IsInitOnly && !info.FieldType.IsValueType && info.FieldType != typeof(string)
                               select info;

            foreach (var member in fieldMembers)
            {
                var fieldValue = member.GetValue(value);

                // don't set a field if it already has a value
                if (fieldValue != null)
                {
                    continue;
                }

                if (suppressExceptions)
                {
                    try
                    {
                        var resolvedValue = container.Resolve(member.FieldType);
                        member.SetValue(value, resolvedValue);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    var resolvedValue = container.Resolve(member.FieldType);
                    member.SetValue(value, resolvedValue);
                }
            }

            foreach (var member in propertyMembers)
            {
                var getMethod = member.GetGetMethod();

                var propValue = getMethod.Invoke(value, null);

                // don't set a property if it already has a value
                if (propValue != null)
                {
                    continue;
                }

                if (suppressExceptions)
                {
                    try
                    {
                        var resolvedValue = container.Resolve(member.PropertyType);
                        member.GetSetMethod().Invoke(value, new[] { resolvedValue });
                    }
                    catch
                    {
                    }
                }
                else
                {
                    var resolvedValue = container.Resolve(member.PropertyType);
                    member.GetSetMethod().Invoke(value, new[] { resolvedValue });
                }
            }
        }
    }
}