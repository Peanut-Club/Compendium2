namespace Compendium.Parenting
{
    public class ChildObject
    {
        public ParentObject Parent { get; internal set; }

        public virtual void OnAddedToParent() { }
        public virtual void OnRemovedFromParent() { }
    }
}