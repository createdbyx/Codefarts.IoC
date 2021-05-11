// <copyright file="Container NET20.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

#if NET20
namespace Codefarts.IoC
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Provides a simple IoC container functions.
    /// </summary>
    public partial class Container
    {
        public void ResolveMembers(object value)
        {
            throw new NotImplementedException();
            /*
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
            */
        }
    }
} 
#endif