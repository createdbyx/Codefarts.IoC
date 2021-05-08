// <copyright file="Container NET35.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

#if !NET20
namespace Codefarts.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides a simple IoC container functions.
    /// </summary>
    public partial class Container
    {
        private ConstructorInfo GetDefaultListConstructor(Type returnType)
        {
            var funcGenericType = typeof(List<>).MakeGenericType(returnType);
            return funcGenericType.GetConstructors().FirstOrDefault(x => !x.GetParameters().Any());
        }

        private IEnumerable<ConstructorInfo> GetPublicConstructorWithValidParameters(Type type)
        {
            // get public constructors ordered by most arguments first
            var constructors = from c in type.GetConstructors()
                               let parameters = c.GetParameters()
                               where c.IsPublic && !parameters.Any(x => x.ParameterType.IsValueType ||
                                                                        typeof(Delegate).IsAssignableFrom(x.ParameterType) ||
                                                                        type == typeof(string))
                               orderby parameters.Length descending
                               select c;
            return constructors;
        }

        private IEnumerable<ConstructorInfo> GetPublicConstructorWithMatchingParameters(Type type, object[] args)
        {
            var constructorCandidates = new List<ConstructorInfo>();

            // can't resolve argument types
            if (args.Any(x => x == null))
            {
                return constructorCandidates;
            }

            var constructors = type.GetConstructors();
            var argTypes = args.Select(x => x.GetType()).ToArray();
            foreach (var info in constructors.Where(x => x.IsPublic))
            {
                var constructorParamTypes = info.GetParameters().Select(x => x.ParameterType);

                if (constructorParamTypes.SequenceEqual(argTypes))
                {
                    constructorCandidates.Add(info);
                }
            }

            return constructorCandidates;
        }

        private bool IsInvalidInstantiationType(Type type)
        {
            return type.IsAbstract ||
                   type.IsInterface ||
                   type.IsValueType ||
                   typeof(Delegate).IsAssignableFrom(type) ||
                   type == typeof(string);
        }
    }
}
#endif