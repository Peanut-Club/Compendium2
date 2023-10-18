using System;

namespace Compendium.Attributes
{
    public class ResolveableAttribute<TAttribute> : Attribute where TAttribute : Attribute
    {
        public AttributeInfo<TAttribute> AttributeInfo { get; private set; }

        public virtual void OnResolved(AttributeInfo<TAttribute> attributeInfo) { }

        internal void SetInfo(AttributeInfo<TAttribute> attributeInfo)
            => AttributeInfo = attributeInfo;
    }
}
