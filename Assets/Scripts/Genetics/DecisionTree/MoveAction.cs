using UnityEngine;

namespace Genetics.DecisionTree
{
    public class MoveAction : Node
    {
        private Vector3 targetPosition;

        public MoveAction(Vector3 targetPosition)
        {
            this.targetPosition = targetPosition;
        }

        public override bool Evaluate(Creature creature)
        {
            // float step = creature.Chromosome.BasicStats.speed * Time.deltaTime;
            // // creature.transform.position = Vector3.MoveTowards(creature.transform.position, targetPosition, step);
            // if (Vector3.Distance(creature.transform.position, targetPosition) < 0.1f)
            // {
            //     creature.OnMovePerformed();
            //     return true; // Acción completada
            // }

            return false;

            // corr to move to target (?)

            return true;
        }
    }
}