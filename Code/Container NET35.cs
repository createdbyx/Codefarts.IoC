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

        private object[] ResolveParametersFromConstructorInfo(ConstructorInfo constructor)
        {
            var arguments = from p in constructor.GetParameters()
                            select this.Resolve(p.ParameterType);
            return arguments.ToArray();
        }

        private IEnumerable<ConstructorInfo> GetPublicConstructorWithValidParameters(Type type)
        {
            // get public constructors ordered by most arguments first
            var constructors = from c in type.GetConstructors()
                               let parameters = c.GetParameters()
                               where c.IsPublic && !parameters.Any(x => x.ParameterType.IsValueType)
                               orderby parameters.Length descending
                               select c;
            return constructors;
        }

        public void ResolveMembers(object value)
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
                                  where info.GetGetMethod() != null && info.GetSetMethod() != null && this.CanResolve(info.PropertyType)
                                  select info;

            var fieldMembers = from member in memberInfos
                               where member.MemberType == MemberTypes.Field
                               let info = (FieldInfo)member
                               where !info.IsInitOnly && this.CanResolve(info.FieldType)
                               select info;

            foreach (var member in fieldMembers)
            {
                var fieldValue = member.GetValue(value);

                // don't set a field if it already has a value
                if (fieldValue != null)
                {
                    continue;
                }

                var resolvedValue = this.Resolve(member.FieldType);
                member.SetValue(value, resolvedValue);
            }

            foreach (var member in propertyMembers)
            {
                var propValue = this.GetPropertyValue(value, member);

                // don't set a property if it already has a value
                if (propValue != null)
                {
                    continue;
                }

                var resolvedValue = this.Resolve(member.PropertyType);
                member.GetSetMethod().Invoke(value, new[] { resolvedValue });
            }
        }
    }
} 
#endif