using Compendium.Utilities.Reflection;

using System;
using System.Reflection;

namespace Compendium.Attributes
{
    public struct AttributeInfo<TAttribute> where TAttribute : Attribute
    {
        public readonly MethodInfo Method;
        public readonly PropertyInfo Property;
        public readonly FieldInfo Field;
        public readonly Assembly Assembly;
        public readonly Type Type;

        public readonly object Handle;

        public readonly TAttribute Attribute;

        public readonly AttributeLocation Location;

        public AttributeInfo(MethodInfo method, TAttribute attribute, object handle)
        {
            Method = method;
            Type = method.DeclaringType;
            Assembly = method.DeclaringType.Assembly;
            Attribute = attribute;
            Handle = handle;
            Location = AttributeLocation.Method;
        }

        public AttributeInfo(FieldInfo field, TAttribute attribute, object handle)
        {
            Field = field;
            Type = field.DeclaringType;
            Assembly = field.DeclaringType.Assembly;
            Attribute = attribute;
            Handle = handle;
            Location = AttributeLocation.Field;
        }

        public AttributeInfo(PropertyInfo property, TAttribute attribute, object handle)
        {
            Property = property;
            Type = property.DeclaringType;
            Assembly = property.DeclaringType.Assembly;
            Attribute = attribute;
            Handle = handle;
            Location = AttributeLocation.Property;
        }

        public AttributeInfo(Type type, TAttribute attribute)
        {
            Type = type;
            Assembly = type.Assembly;
            Attribute = attribute;
            Location = AttributeLocation.Type;
        }

        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is AttributeInfo<TAttribute> other))
                return false;

            if (other.Location != Location)
                return false;

            switch (Location)
            {
                case AttributeLocation.Field:
                    return other.Field == Field && ObjectUtilities.IsInstance(other.Handle, Handle);

                case AttributeLocation.Property:
                    return other.Property == Property && ObjectUtilities.IsInstance(other.Handle, Handle);

                case AttributeLocation.Type:
                    return other.Type == Type;

                case AttributeLocation.Method:
                    return other.Method == Method && ObjectUtilities.IsInstance(other.Handle, Handle);

                default:
                    return other.Assembly == Assembly;
            }
        }
    }
}