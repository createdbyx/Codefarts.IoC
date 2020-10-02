// <copyright file="Container NET35.cs" company="Codefarts">
// Copyright (c) Codefarts
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
                               where c.IsPublic && !parameters.Any(x => x.ParameterType.IsValueType || typeof(Delegate).IsAssignableFrom(x.ParameterType))
                               orderby parameters.Length descending
                               select c;
            return constructors;
        }
    }
}
#endif