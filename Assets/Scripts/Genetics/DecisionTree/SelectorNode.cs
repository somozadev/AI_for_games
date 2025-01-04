using System.Linq;

namespace Genetics.DecisionTree
{
    public class SelectorNode : Node
    {
        private readonly Node[] _children;

        public SelectorNode(params Node[] children)
        {
            _children = children;
        }

        public override bool Evaluate(Creature creature)
        {
            return _children.Any(child => child.Evaluate(creature));
        }
    }
}