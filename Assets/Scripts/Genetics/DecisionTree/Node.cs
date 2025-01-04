namespace Genetics.DecisionTree
{
    [System.Serializable]
    public abstract class Node
    {
        public abstract bool Evaluate(Creature creature);
    }
}