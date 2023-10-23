using YamlDotNet.Core;
using YamlDotNet.Core.Events;

using YamlDotNet.Serialization.ObjectGraphVisitors;
using YamlDotNet.Serialization;

namespace Compendium.Configuration.YamlApi
{
    public sealed class CommentsObjectGraphVisitor : ChainedObjectGraphVisitor
    {
        public CommentsObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor) : base(nextVisitor) { }

        public override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context)
        {
            if (value is CommentsObjectDescriptor commentsDescriptor && commentsDescriptor.Comment is not null)
                context.Emit(new Comment(commentsDescriptor.Comment, false));

            return base.EnterMapping(key, value, context);
        }
    }
}