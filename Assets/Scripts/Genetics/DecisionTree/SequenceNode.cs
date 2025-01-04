using System.Linq;

namespace Genetics.DecisionTree
{
    public class SequenceNode : Node
    {
        private readonly Node[] _children;

        public SequenceNode(params Node[] children)
        {
            _children = children;
        }

        public override bool Evaluate(Creature creature)
        {
            return _children.All(child => child.Evaluate(creature));
        }
    }
}