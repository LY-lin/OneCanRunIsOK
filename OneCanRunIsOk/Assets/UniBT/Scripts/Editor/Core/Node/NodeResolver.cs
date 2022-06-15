using System;

namespace UniBT.Editor
{
    public class NodeResolver
    {
        public BehaviorTreeNode CreateNodeInstance(Type type)
        {
            BehaviorTreeNode node;
            if (type.IsSubclassOf(typeof(Composite)))
            {
                node = new CompositeNode();
            } else if (type.IsSubclassOf(typeof(Conditional)))
            {
                node = new ConditionalNode();
            } else if (type == typeof(Root))
            {
                node = new RootNode();
            }
            else
            {
                node = new ActionNode();
            }
            node.SetBehavior(type);
            return node;
        }
    }
}