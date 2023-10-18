using Compendium.Logging;
using Compendium.Pooling.Pools;
using Compendium.Utilities.Reflection;

using MonoMod.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Compendium.Attributes
{
    public static class AttributeResolver<TAttribute> where TAttribute : Attribute
    {
        private static readonly List<AttributeInfo<TAttribute>> _attributes = new List<AttributeInfo<TAttribute>>();

        public static event Action<AttributeInfo<TAttribute>> OnResolved;
        public static event Action<AttributeInfo<TAttribute>> OnRemoved;

        public static IReadOnlyList<AttributeInfo<TAttribute>> Attributes => _attributes;

        public static void ForEach(
            Predicate<AttributeInfo<TAttribute>> predicate, 
            
            Action<List<AttributeInfo<TAttribute>>> listAction, 
            Action<AttributeInfo<TAttribute>> action)
        {
            var list = ListPool<AttributeInfo<TAttribute>>.Shared.Rent();

            if (predicate != null)
                list.AddRange(_attributes.Where(a => predicate.SafeCall(a)));
            else
                list.AddRange(_attributes);

            listAction.SafeCall(list);

            for (int i = 0; i < list.Count; i++)
                action.SafeCall(list[i]);

            ListPool<AttributeInfo<TAttribute>>.Shared.Return(list);
        }

        public static List<AttributeInfo<TAttribute>> ResolveAttributes(Assembly assembly, AttributeResolveFlags flags = AttributeResolveFlags.Field | AttributeResolveFlags.Method | AttributeResolveFlags.Property)
        {
            var list = new List<AttributeInfo<TAttribute>>();

            foreach (var type in assembly.GetTypes())
                list.AddRange(ResolveAttributes(type, null, flags));

            return list;
        }

        public static List<AttributeInfo<TAttribute>> ResolveAttributes(Type type, object handle = null, AttributeResolveFlags attributeResolveFlags = AttributeResolveFlags.Field | AttributeResolveFlags.Property | AttributeResolveFlags.Method | AttributeResolveFlags.Type)
        {
            var list = new List<AttributeInfo<TAttribute>>();

            try
            {
                if (attributeResolveFlags.Has(AttributeResolveFlags.Type) && type.HasAttribute<TAttribute>(out var typeAttribute))
                    ResolveAttribute(new AttributeInfo<TAttribute>(type, typeAttribute), list);

                if (attributeResolveFlags.Has(AttributeResolveFlags.Method))
                {
                    var methods = type.GetAllMethods();

                    for (int i = 0; i < methods.Length; i++)
                    {
                        if (methods[i].HasAttribute<TAttribute>(out var methodAttribute))
                            ResolveAttribute(new AttributeInfo<TAttribute>(methods[i], methodAttribute, handle), list);
                    }
                }

                if (attributeResolveFlags.Has(AttributeResolveFlags.Field))
                {
                    var fields = type.GetAllFields();

                    for (int i = 0; i < fields.Length; i++)
                    {
                        if (fields[i].HasAttribute<TAttribute>(out var fieldAttribute))
                            ResolveAttribute(new AttributeInfo<TAttribute>(fields[i], fieldAttribute, handle), list);
                    }
                }


                if (attributeResolveFlags.Has(AttributeResolveFlags.Property))
                {
                    var properties = type.GetAllProperties();

                    for (int i = 0; i < properties.Length; i++)
                    {
                        if (properties[i].HasAttribute<TAttribute>(out var propertyAttribute))
                            ResolveAttribute(new AttributeInfo<TAttribute>(properties[i], propertyAttribute, handle), list);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Critical("Attribute Resolver", $"Failed while resolving attributes in '{type.ToName()}' due to an exception.", ex);
            }

            return list;
        }

        public static void ResolveAttribute(AttributeInfo<TAttribute> attribute, List<AttributeInfo<TAttribute>> list)
        {
            list?.Add(attribute);

            if (_attributes.Any(a => a.Equals(attribute)))
                return;

            if (attribute.Attribute is ResolveableAttribute<TAttribute> resolveable)
            {
                resolveable.SetInfo(attribute);
                resolveable.OnResolved(attribute);
            }    

            _attributes.Add(attribute);
            OnResolved.SafeCall(attribute);
        }

        public static void RemoveAttributes(Type type, object handle = null)
        {
            try
            {
                var methods = type.GetAllMethods();
                var fields = type.GetAllFields();
                var properties = type.GetAllProperties();

                if (type.HasAttribute<TAttribute>(out var typeAttribute))
                    RemoveAttribute(new AttributeInfo<TAttribute>(type, typeAttribute));

                for (int i = 0; i < methods.Length; i++)
                {
                    if (methods[i].HasAttribute<TAttribute>(out var methodAttribute))
                        RemoveAttribute(new AttributeInfo<TAttribute>(methods[i], methodAttribute, handle));
                }

                for (int i = 0; i < fields.Length; i++)
                {
                    if (fields[i].HasAttribute<TAttribute>(out var fieldAttribute))
                        RemoveAttribute(new AttributeInfo<TAttribute>(fields[i], fieldAttribute, handle));
                }

                for (int i = 0; i < properties.Length; i++)
                {
                    if (properties[i].HasAttribute<TAttribute>(out var propertyAttribute))
                        RemoveAttribute(new AttributeInfo<TAttribute>(properties[i], propertyAttribute, handle));
                }
            }
            catch (Exception ex)
            {
                Log.Critical("Attribute Resolver", $"Failed while removing attributes from '{type.ToName()}' due to an exception.", ex);
            }
        }

        public static void RemoveAttribute(AttributeInfo<TAttribute> attribute)
        {
            var removingAttributes = _attributes.Where(a => a.Equals(attribute));

            if (removingAttributes.Any())
            {
                foreach (var attr in removingAttributes)
                {
                    _attributes.Remove(attr);
                    OnRemoved.SafeCall(attr);
                }
            }
        }
    }
}